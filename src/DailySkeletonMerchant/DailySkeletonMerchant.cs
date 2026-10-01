using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace DailySkeletonMerchant;

/// <summary>
/// 每日骷髅商人：每个游戏日上午在出生点附近固定位置出现一名骷髅商人（NPC 453）；
/// 玩家全部走远他就离开，当天不再出现，被打死也不补位，第二天再来。
///
/// 货品完全沿用原版：骷髅商人的商品表由客户端按 <c>Main.moonPhase</c> 现场计算
/// （Chest.SetupShop + ShopHelper.GetSkeletonMerchantPrices），而 moonPhase 在每天清晨 4:30
/// 由 Main.UpdateTime 递增一次。所以本插件不缓存、不改写任何商店数据，
/// 只要人在场，第二天清晨就会自动随原版换货。
///
/// 与洞穴里自然刷出的骷髅商人不冲突：场上已经有 453 时插件当天不干预、不生成第二只。
/// </summary>
[ApiVersion(2, 1)]
public class DailySkeletonMerchant : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "不是现在";
    public override string Description => GetString("骷髅商人每天上午出现在出生点附近");
    public override Version Version => new(1, 0);

    private const string LogPrefix = "[DailySkeletonMerchant] ";
    private const string CommandPermission = "tshock.admin";

    /// <summary>骷髅商人 NPC ID（原版常数）。</summary>
    private const int MerchantId = NPCID.SkeletonMerchant;

    /// <summary>到访窗口：上午 4:30 起 450 分钟 = 中午 12 点。</summary>
    private const int ArrivalWindowMinutes = 450;

    /// <summary>
    /// 判定间隔（毫秒）。用真实时间而不是"每 N 帧"：GameUpdate 每帧触发一次，
    /// 60 FPS 下每秒 60 次，按 20 帧算只有 0.33 秒。
    /// </summary>
    private const long ScanIntervalMs = 1000;

    /// <summary>锚点搜索：以出生点为圆心向外找可站立的位置，起始圈数与最大圈数。</summary>
    private const int AnchorSearchStart = 2;
    private const int AnchorSearchRadius = 40;

    /// <summary>
    /// 离场半径（像素）。附近多少像素内没有在线玩家就送走，
    /// 取原版 NPC 反卸载半径（800）保持一致。
    /// </summary>
    private const float LeaveRadiusPx = 800f;

    private Command? _command;
    private long _lastScanAt;
    private long _scanCount;
    private bool _worldReady;

    /// <summary>今天已经安排过了（成功生成过，或因为场上已有骷髅商人不干预）。</summary>
    private bool _handledThisDay;
    private bool _handledAtDay;
    private double _handledAtTime;
    private int _handledAtMoonPhase;

    /// <summary>标记"今天的名额已用掉"，并记住当时的昼夜、时钟与月相。</summary>
    private void MarkHandled()
    {
        _handledThisDay = true;
        _handledAtDay = Main.dayTime;
        _handledAtTime = Main.time;
        _handledAtMoonPhase = Main.moonPhase;
    }

    /// <summary>
    /// 判断是否已经进入新的一天，三个信号任一变化就算新的一天：
    /// 1. 昼夜不同 —— 采样到的翻转（正常游玩时每秒都会采到）；
    /// 2. 月相不同 —— 原版 Main.UpdateTime 在每天清晨 4:30 自己 +1，这是游戏自己的日计数器；
    /// 3. 时钟明显倒退 —— 管理员用 /time 跳回清晨。
    /// 必须在每次判定里跑，不能只靠 GameUpdate：空服时它根本不触发。
    /// 已知局限：/time 在"同一天的相同时刻"之间来回跳（例如 22:00 → 04:30 → 22:00）时
    /// 三个信号都看不出来，会当成同一天；正常游玩不会遇到这种情况。
    /// </summary>
    private void UpdateDayState()
    {
        if (!_handledThisDay)
            return;

        if (Main.dayTime != _handledAtDay ||
            Main.moonPhase != _handledAtMoonPhase ||
            Main.time < _handledAtTime - 120.0)
            _handledThisDay = false;
    }

    /// <summary>插件自己生成的那只的槽位；-1 表示当前没有"我们的"骷髅商人。</summary>
    private int _spawnedIndex = -1;

    public DailySkeletonMerchant(Main game) : base(game) { }

    // ------------------------------------------------------------------ 生命周期

    public override void Initialize()
    {
        RegisterCommand();
        ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);

        TShock.Log.ConsoleInfo(
            $"{LogPrefix}v{Version} 已加载：骷髅商人 NPC ID = {MerchantId}，" +
            $"每天上午 4:30 起在出生点附近出现一次，玩家走远即离场、当天不再出现、死亡不补位，" +
            $"货品沿用原版（每天清晨随月相自动换货），命令 /skeleton（权限 {CommandPermission}）。");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            RemoveCommand();
        }

        base.Dispose(disposing);
    }

    // ------------------------------------------------------------------ 每秒扫描

    private void OnUpdate(EventArgs args)
    {
        // Terraria 空服不推进世界（GameUpdate 也不触发），有人进服后这里会自动补上判定。
        if (!EnsureWorldReady())
            return;

        if (Environment.TickCount64 - _lastScanAt < ScanIntervalMs)
            return;
        _lastScanAt = Environment.TickCount64;

        RunScan();
    }

    /// <summary>
    /// 世界没载入（主菜单、世界数据还没准备好）时返回 false，此时不做任何判定。
    /// 只有在"就绪状态发生变化"的那一次才重置当天状态，之后都是空操作——
    /// 否则空服时第一个判定来自 /skeleton 这类命令，会把命令刚记下的东西顺手清掉。
    /// </summary>
    private bool EnsureWorldReady()
    {
        bool ready = !Main.gameMenu && Main.maxTilesX > 100;
        if (ready == _worldReady)
            return ready;

        _worldReady = ready;
        if (!ready)
            return false;

        _handledThisDay = false;
        _spawnedIndex = -1;   // 换了世界 / 地图，NPC 槽位记录作废
        TShock.Log.ConsoleDebug($"{LogPrefix}世界已载入（{(Main.dayTime ? "白天" : "夜晚")}），开始每日到访判定。");
        return true;
    }

    /// <summary>
    /// 一次判定。每秒自动跑一次，也可以用 /skeleton check 手动跑一次
    /// （空服时世界不推进，自动判定不会触发，测试或排查时可用手动触发）。
    /// </summary>
    private void RunScan()
    {
        if (!EnsureWorldReady())
            return;

        UpdateDayState();
        _scanCount++;

        // ---- 1. 离场判定：附近没人就送走（只管插件自己生成的那一只）。
        CheckLeave();

        // ---- 2. 到访判定：一天一次。
        if (_handledThisDay)
            return;

        if (!Main.dayTime)
            return;

        if (Main.time >= ArrivalWindowMinutes * 60.0)
            return;

        if (Main.IsFastForwardingTime())   // 日晷 / Moondial 生效时原版不刷怪
            return;

        // 场上已经有骷髅商人（洞穴里刷到的、昨天留下的、管理员 summon 的）：
        // 按需求"插件不加干预"，今天就让游戏自己来。
        if (CountMerchants() > 0)
        {
            MarkHandled();
            TShock.Log.ConsoleDebug($"{LogPrefix}场上已有骷髅商人，插件今天不干预。");
            return;
        }

        // 没人在线就不占用今天的机会，等有人进服再判定。
        if (!AnyPlayerOnline())
            return;

        if (!TryFindAnchor(out Point tile))
        {
            TShock.Log.ConsoleDebug($"{LogPrefix}出生点附近 {AnchorSearchRadius} 格内找不到能站立的位置，本次跳过。");
            return;
        }

        if (!SpawnAt(tile, out int index))
        {
            TShock.Log.ConsoleDebug($"{LogPrefix}生成失败（NPC 槽位不足或位置被占），稍后重试。");
            return;
        }

        _spawnedIndex = index;
        MarkHandled();
    }

    // ------------------------------------------------------------------ 离场

    /// <summary>插件生成的那只在附近没人时离场；被玩家打死则只清记录，不补位。</summary>
    private void CheckLeave()
    {
        if (_spawnedIndex < 0)
            return;

        if (_spawnedIndex >= Main.maxNPCs)
        {
            _spawnedIndex = -1;
            return;
        }

        NPC? npc = Main.npc[_spawnedIndex];
        if (npc == null || !npc.active || npc.netID != MerchantId)
        {
            // 死亡、或槽位已经被别的 NPC 顶替：清记录，今天不再出现。
            _spawnedIndex = -1;
            return;
        }

        if (AnyPlayerNear(npc.Center, LeaveRadiusPx))
            return;

        Despawn(_spawnedIndex);
        _spawnedIndex = -1;
        TShock.Log.ConsoleDebug($"{LogPrefix}附近没有玩家了，骷髅商人离场（今天不会再出现）。");
    }

    /// <summary>清场方式与原版 UnspawnTravelNPC 一致：清 active/life 并广播 23 号包。</summary>
    private static void Despawn(int index)
    {
        if (index < 0 || index >= Main.maxNPCs)
            return;

        NPC npc = Main.npc[index];
        npc.active = false;
        npc.life = 0;

        if (Main.netMode == 2)
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, index);
    }

    // ------------------------------------------------------------------ 生成

    private static bool SpawnAt(Point tile, out int index)
    {
        index = NPC.NewNPC(new EntitySource_SpawnNPC(), tile.X * 16 + 8, tile.Y * 16, MerchantId);
        if (index < 0 || index >= Main.maxNPCs)
            return false;

        NPC npc = Main.npc[index];

        // 他不是城镇 NPC（townNPC = false），但跑城镇 AI；
        // 按无家 NPC 处理才不会去寻路回"家"，与原版造旅商时的做法一致。
        npc.homeless = true;
        npc.netUpdate = true;

        if (Main.netMode == 2)
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, index);

        return true;
    }

    /// <summary>
    /// 在出生点附近找一个能站立的位置。由近及远逐圈查找，顺序只取决于出生点坐标，
    /// 所以每天算出来都是同一个位置（固定）。
    /// </summary>
    private static bool TryFindAnchor(out Point tile)
    {
        tile = Point.Zero;
        int sx = Main.spawnTileX;
        int sy = Main.spawnTileY;

        if (sx <= 0 || sy <= 0 || sx >= Main.maxTilesX || sy >= Main.maxTilesY)
            return false;

        for (int r = AnchorSearchStart; r <= AnchorSearchRadius; r++)
        {
            for (int dy = -r; dy <= r; dy++)
            {
                for (int dx = -r; dx <= r; dx++)
                {
                    if (Math.Abs(dx) != r && Math.Abs(dy) != r)
                        continue;   // 只看当前这一圈

                    if (!CanStand(sx + dx, sy + dy))
                        continue;

                    tile = new Point(sx + dx, sy + dy);
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>脚下有实地、身体两格与两侧为空、不泡水，才算能站人。</summary>
    private static bool CanStand(int x, int y)
    {
        if (x < 25 || x >= Main.maxTilesX - 25 || y < 20 || y >= Main.maxTilesY - 20)
            return false;

        if (!WorldGen.SolidTile(x, y + 1))
            return false;

        if (WorldGen.SolidTile(x, y) || WorldGen.SolidTile(x, y - 1))
            return false;

        if (WorldGen.SolidTile(x - 1, y) || WorldGen.SolidTile(x + 1, y))
            return false;

        if (Main.tile[x, y].liquid > 0 || Main.tile[x, y - 1].liquid > 0)
            return false;

        return true;
    }

    // ------------------------------------------------------------------ 场上查询

    private static int CountMerchants()
    {
        int count = 0;

        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc != null && npc.active && npc.life > 0 && npc.netID == MerchantId)
                count++;
        }

        return count;
    }

    private static bool AnyPlayerOnline()
    {
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            Player? player = Main.player[i];
            if (player != null && player.active)
                return true;
        }

        return false;
    }

    private static int CountPlayersNear(Vector2 center, float radius)
    {
        int count = 0;

        for (int i = 0; i < Main.maxPlayers; i++)
        {
            Player? player = Main.player[i];
            if (player != null && player.active && Vector2.Distance(player.Center, center) <= radius)
                count++;
        }

        return count;
    }

    private static bool AnyPlayerNear(Vector2 center, float radius) =>
        CountPlayersNear(center, radius) > 0;

    /// <summary>骷髅商人附近（离场半径内）的在线玩家数，排查"为什么不离场"用。</summary>
    private static int NearbyPlayerCount()
    {
        int count = 0;

        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc != null && npc.active && npc.netID == MerchantId)
                count += CountPlayersNear(npc.Center, LeaveRadiusPx);
        }

        return count;
    }

    private static string Where()
    {
        var spots = new List<string>();

        for (int i = 0; i < Main.maxNPCs && spots.Count < 5; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc == null || !npc.active || npc.life <= 0 || npc.netID != MerchantId)
                continue;

            spots.Add($"({(int)(npc.position.X / 16f)}, {(int)(npc.position.Y / 16f)})");
        }

        return spots.Count == 0 ? GetString("骷髅商人当前不在场") : string.Join("、", spots);
    }

    /// <summary>把游戏内时刻换算成 HH:MM：白天 0 刻 = 4:30，夜晚 0 刻 = 19:30。</summary>
    private static string Clock()
    {
        double hours = (Main.dayTime ? 4.5 + Main.time / 3600.0 : 19.5 + Main.time / 3600.0) % 24.0;
        int h = (int)hours;
        int m = (int)((hours - h) * 60.0);
        return $"{h:00}:{m:00}";
    }

    private static bool InWindow() =>
        Main.dayTime && Main.time < ArrivalWindowMinutes * 60.0 && !Main.IsFastForwardingTime();

    /// <summary>当前"今天能不能来"的诊断文案，供 status / check 使用。</summary>
    private string BlockReason()
    {
        // 场上有人时先说这个：不管他是插件生成的还是原版刷的，插件都不会再生成第二只。
        if (CountMerchants() > 0)
            return GetString("场上已有骷髅商人，插件不干预");

        if (_handledThisDay)
            return GetString("今天已经安排过了（不会再出现）");

        if (!Main.dayTime)
            return GetString("现在是夜晚（明天上午再来）");

        if (Main.IsFastForwardingTime())
            return GetString("日晷 / Moondial 生效中");

        if (Main.time >= ArrivalWindowMinutes * 60.0)
            return Format(GetString("已过上午4:30 + {0} 分钟"), ArrivalWindowMinutes);

        if (!AnyPlayerOnline())
            return GetString("暂无玩家在线，等有人进服再判定");

        if (!TryFindAnchor(out Point tile))
            return GetString($"出生点附近 {AnchorSearchRadius} 格内找不到能站立的位置");

        return Format(GetString("条件已满足，将出现在出生点附近（{0}, {1}）"), tile.X, tile.Y);
    }

    /// <summary>
    /// 带占位符的文案统一这样拼：先取译文（没有译文时返回原文模板），再自己填值。
    /// 不能写成 GetString($"…{0}…", args) —— GetText.NET 8 的 FormattableStringAdapter 会先把
    /// 插值算好再查表，位置参数 {0} 会被当成"没有这个实参"填成 0。
    /// 也不能直接写 GetString($"…{expr}…") —— 这样查表用的键是渲染后的文本，译文永远命中不了。
    /// </summary>
    private static string Format(string template, params object?[] args)
        => string.Format(template, args);

    // ------------------------------------------------------------------ 命令

    private void RegisterCommand()
    {
        RemoveCommand();
        _command = new Command(CommandPermission, OnCommand, "skeleton", "skeletonmerchant", "骷髅商人", "骷髅")
        {
            HelpText = GetString("骷髅商人每日到访（/skeleton summon|despawn|check|status）"),
            AllowServer = true
        };
        Commands.ChatCommands.Add(_command);
    }

    private void RemoveCommand()
    {
        if (_command == null)
            return;

        Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == OnCommand);
        _command = null;
    }

    private void OnCommand(CommandArgs args)
    {
        TSPlayer player = args.Player;

        // 命令也算一次"世界已载入"的确认，否则首个命令的操作会被世界初始化顺手清掉。
        EnsureWorldReady();

        string sub = args.Parameters.Count > 0 ? args.Parameters[0].ToLowerInvariant() : "help";

        switch (sub)
        {
            case "help" or "帮助" or "?":
                SendHelp(player);
                break;

            case "summon" or "s" or "召唤" or "来":
                if (CountMerchants() > 0)
                {
                    player.SendInfoMessage(GetString("场上已经有骷髅商人了。"));
                    break;
                }

                if (!TryFindAnchor(out Point tile))
                {
                    player.SendErrorMessage($"§e[骷髅商人] §7{GetString("出生点附近找不到能站立的位置。")}");
                    break;
                }

                if (!SpawnAt(tile, out int index))
                {
                    player.SendErrorMessage($"§e[骷髅商人] §7{GetString("召唤失败：NPC 槽位不足。")}");
                    break;
                }

                _spawnedIndex = index;
                MarkHandled();
                player.SendInfoMessage($"§e[骷髅商人] §7{GetString("已召唤，他今天就在这里了。")} §f({tile.X}, {tile.Y})");
                break;

            case "despawn" or "d" or "离开" or "送走":
                if (CountMerchants() == 0)
                {
                    player.SendInfoMessage(GetString("场上本来就没有骷髅商人。"));
                    break;
                }

                DespawnAll();
                _spawnedIndex = -1;
                MarkHandled();
                player.SendInfoMessage($"§e[骷髅商人] §7{GetString("已请骷髅商人离场（今天不会再出现）。")}");
                break;

            case "status" or "st" or "状态":
                SendStatus(player);
                break;

            case "check" or "tick" or "检查" or "判定":
                // 空服时世界不推进、自动判定不会触发，这里手动跑一次（也是自动化测试的入口）。
                _lastScanAt = Environment.TickCount64;
                RunScan();
                player.SendInfoMessage($"§e[骷髅商人] §7{GetString("已手动判定一次：")}§f{BlockReason()}§7");
                break;

            default:
                SendHelp(player);
                break;
        }
    }

    private static void DespawnAll()
    {
        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc != null && npc.active && npc.netID == MerchantId)
                Despawn(i);
        }
    }

    private static void SendHelp(TSPlayer player)
    {
        player.SendInfoMessage("§e[骷髅商人] §7" + GetString("指令用法："));
        player.SendInfoMessage("§f/skeleton summon §7— " + GetString("立刻召唤到出生点附近（/骷髅商人 召唤）"));
        player.SendInfoMessage("§f/skeleton despawn §7— " + GetString("请他离场，今天不再出现（/骷髅商人 离开）"));
        player.SendInfoMessage("§f/skeleton check §7— " + GetString("手动跑一次到访判定（/骷髅商人 检查）"));
        player.SendInfoMessage("§f/skeleton status §7— " + GetString("查看每日到访状态（/骷髅商人 状态）"));
    }

    private void SendStatus(TSPlayer player)
    {
        int alive = CountMerchants();
        bool hasAnchor = TryFindAnchor(out Point tile);
        int near = NearbyPlayerCount();

        player.SendInfoMessage($"§e[骷髅商人] §7{GetString("启用中")} · " +
                               $"{GetString("骷髅商人 NPC ID =")} §f{MerchantId}§7 · " +
                               $"{GetString("每天上午 4:30 起一次")} · " +
                               $"{GetString("窗口")} §f{GetString("上午4:30 起")} {ArrivalWindowMinutes} {GetString("分钟")}§7");
        player.SendInfoMessage($"§e[骷髅商人] §7{GetString("当前：")}{(Main.dayTime ? "白天" : "夜晚")} §f{Clock()}§7，" +
                               $"{GetString("月相")} §f{Main.moonPhase}§7（{GetString("每天清晨 4:30 递增，货品随之更换")}），" +
                               $"{GetString("场上骷髅商人")} §f{alive}§7 {GetString("位")}，{GetString("位置")} §f{Where()}§7");
        player.SendInfoMessage($"§e[骷髅商人] §7{GetString("出生点：")}§f({Main.spawnTileX}, {Main.spawnTileY})§7，" +
                               $"{GetString("锚点")} §f" + (hasAnchor ? $"({tile.X}, {tile.Y})" : GetString("未找到")) + "§7");
        player.SendInfoMessage($"§e[骷髅商人] §7{GetString("今日：")}§f" +
                               (_handledThisDay ? GetString("已安排") : GetString("未安排")) + "§7 · " +
                               $"{GetString("离场半径")} §f{LeaveRadiusPx / 16f}{GetString("格")}§7 · " +
                               $"{GetString("附近玩家")} §f{near}§7 · " +
                               $"{GetString("判定已跑")} §f{_scanCount}§7 {GetString("次（每秒 1 次自动，空服不跑）")}");
        player.SendInfoMessage($"§e[骷髅商人] §7{GetString("判定：")}§f{BlockReason()}§7");
    }
}