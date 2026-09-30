using System.Reflection;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace DM;

/// <summary>
/// 旅商每日到访（DailyMerchant）
///
/// 原版里「流动旅商」（Travelling Merchant，NPC 368）每 tick 掷一次 1/108000，
/// 窗口是上午 4:30–12:00（27000 tick），所以一天只有 1-(107999/108000)^27000 ≈ 22.12% 的机会；
/// 入睡时概率 ×5，日晷 / Moondial 生效时永不出现。
///
/// 这个插件不自己造 NPC，只是把"每天必然到访"做成默认：
///   * 每秒扫一次，判定条件与原版逐条对齐（白天、窗口内、城镇 NPC 数量、日晷不生效）；
///   * 条件满足就直接调用游戏自己的 WorldGen.SpawnTravelNPC()，
///     商品池（Chest.SetupTravelShop + SendTravelShop）、客户端同步、到访/离开播报全部走原版；
///   * 条件不满足（例如城镇 NPC 还没房子）就静默跳过，等下一个扫描周期，不做任何强行补位。
///
/// 黄昏离场、月食/入侵暂停、天黑后原版自动请他离开，这些本来就有，插件不重复实现。
/// 无配置文件，行为固定为"每天 100% 到访"。
/// </summary>
[ApiVersion(2, 1)]
public class DailyMerchantPlugin : TerrariaPlugin
{
    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "不是现在";
    public override Version Version => new(1, 0);
    public override string Description => GetString("让旅商每天到访：原版流动旅商每天只有约 22% 机会出现，这里改成每天必到，生成与播报仍走原版。");

    public DailyMerchantPlugin(Main game) : base(game) { }

    private const string LogPrefix = "[DailyMerchant] ";
    private const string CommandPermission = "tshock.admin";

    /// <summary>到访窗口：上午 4:30 起 450 分钟 = 中午 12 点，与原版一致。</summary>
    private const int ArrivalWindowMinutes = 450;

    private Command? _command;
    private int _scanCountdown;
    private bool _worldReady;
    private bool _arrivedThisDay;
    private bool _wasDayTime = true;
    private bool _warnedNoHousing;
    private long _scanCount;

    // ------------------------------------------------------------------ 生命周期

    public override void Initialize()
    {
        RegisterCommand();
        ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);

        TShock.Log.ConsoleInfo(
            $"{LogPrefix}v{Version} 已加载：旅商 NPC ID = {NPCID.TravellingMerchant}（流动旅商），" +
            $"每天到访 100%（原版 22.12%），窗口上午4:30 起 {ArrivalWindowMinutes} 分钟，" +
            $"条件不满足时静默跳过，命令 /merchant（权限 {CommandPermission}）。");
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
        bool ready = !Main.gameMenu && Main.maxTilesX > 100;
        if (!ready)
        {
            _worldReady = false;
            _arrivedThisDay = false;
            return;
        }

        if (!_worldReady)
        {
            _worldReady = true;
            _wasDayTime = Main.dayTime;
            _arrivedThisDay = false;
            TShock.Log.ConsoleDebug($"{LogPrefix}世界已载入（{(Main.dayTime ? "白天" : "夜晚")}），开始判定到访。");
        }

        // 天黑 = 今天结束，明天重新赌一次。
        if (Main.dayTime != _wasDayTime)
        {
            if (!Main.dayTime)
            {
                _arrivedThisDay = false;
                _warnedNoHousing = false;
            }

            _wasDayTime = Main.dayTime;
        }

        if (--_scanCountdown > 0)
            return;
        _scanCountdown = 20;   // 20 刻 = 1 秒

        RunScan();
    }

    /// <summary>
    /// 一次到访判定。每秒自动跑一次，也可以用 /merchant check 手动跑一次
    /// （空服时世界不推进，自动判定不会触发，测试或排查时可用手动触发）。
    /// </summary>
    private void RunScan()
    {
        _scanCount++;

        if (_arrivedThisDay || CountMerchants() > 0)
            return;

        // ---- 与原版逐条对齐的条件；任何一条不满足就静默跳过，等下一次判定。
        if (!Main.dayTime)
            return;

        if (Main.time >= ArrivalWindowMinutes * 60.0)
            return;

        if (Main.IsFastForwardingTime())   // 日晷 / Moondial 生效时原版不掷骰
            return;

        if (CountTownNpcs() < 2)
            return;

        // 最后一步交给原版：它自己判断月食/入侵/有没有已入住的 NPC。
        // 不满足就什么都不做（静默跳过），等城镇条件变好了自然会来。
        if (!TryVanillaSpawn())
        {
            if (!_warnedNoHousing)
            {
                _warnedNoHousing = true;
                TShock.Log.ConsoleDebug($"{LogPrefix}{LastVanillaBlock}；静默等待中…");
            }

            return;
        }

        // 到访/离场由游戏自己播报，这里不再往控制台刷常态输出。
        _arrivedThisDay = true;
        _warnedNoHousing = false;
    }

    /// <summary>把游戏内时刻换算成 HH:MM：白天 0 刻 = 4:30，夜晚 0 刻 = 19:30。</summary>
    private static string Clock()
    {
        double hours = (Main.dayTime ? 4.5 + Main.time / 3600.0 : 19.5 + Main.time / 3600.0) % 24.0;
        int h = (int)hours;
        int m = (int)((hours - h) * 60.0);
        return $"{h:00}:{m:00}";
    }

    /// <summary>原版的城镇 NPC 数量要求：不含护士(37)、骷髅旅商(453)、旅商自己。</summary>
    private static int CountTownNpcs()
    {
        int count = 0;

        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc == null || !npc.active || !npc.townNPC || npc.life <= 0)
                continue;

            int type = npc.netID;
            if (type == 37 || type == 453 || type == NPCID.TravellingMerchant)
                continue;

            count++;
        }

        return count;
    }

    /// <summary>统计城镇 NPC：总数 / 已入住 / 虽无房但有空房可搬（仅用于 status 诊断）。</summary>
    private static void Town(out int town, out int housed, out int withRoom)
    {
        town = housed = withRoom = 0;

        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc == null || !npc.active || !npc.townNPC || npc.life <= 0)
                continue;

            int type = npc.netID;
            if (type == 37 || type == 453 || type == NPCID.TravellingMerchant)
                continue;

            town++;

            if (!npc.homeless && (npc.homeTileX > 0 || npc.homeTileY > 0))
            {
                housed++;
                continue;
            }

            if (WorldGen.TownManager.HasRoom(type, out _))
                withRoom++;
        }
    }

    /// <summary>调用原版生成。返回 false 表示原版拒绝（条件不满足）。</summary>
    private static bool TryVanillaSpawn()
    {
        int before = CountMerchants();
        WorldGen.SpawnTravelNPC();
        bool ok = CountMerchants() > before;
        LastVanillaBlock = ok ? GetString("原版已放行") : ExplainVanillaBlock();
        return ok;
    }

    /// <summary>按原版 SpawnTravelNPC 的判断顺序逐条复算，说明它到底卡在哪一步。</summary>
    private static string ExplainVanillaBlock()
    {
        if (Main.eclipse)
            return GetString("原版拒绝：月食天不生成旅商");

        if (!Main.dayTime)
            return GetString("原版拒绝：夜晚不生成旅商");

        if (Main.invasionType > 0 && Main.invasionDelay == 0 && Main.invasionSize > 0)
            return GetString("原版拒绝：入侵进行中");

        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc != null && npc.active && npc.type == NPCID.TravellingMerchant)
                return GetString($"原版拒绝：场上已有旅商（槽位 {0}，生命 {1}）", i, npc.life);
        }

        int homes = 0;
        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc != null && npc.active && npc.townNPC && npc.type != 37 && !npc.homeless)
                homes++;
        }

        if (homes == 0)
        {
            int rooms = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC? npc = Main.npc[i];
                if (npc != null && npc.active && npc.townNPC && npc.type != 37 && npc.homeless &&
                    WorldGen.TownManager.HasRoom(npc.type, out _))
                    rooms++;
            }

            return rooms == 0
                ? GetString("原版拒绝：既没有已入住的城镇 NPC，也没有可搬入的空房")
                : GetString("原版拒绝：TownManager 找不到可用的落脚点");
        }

        // 走到这里原版一定会调 NPC.NewNPC；如果仍然没出现，多半是生成点被占或槽位耗尽。
        int active = 0;
        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc != null && npc.active)
                active++;
        }

        return GetString($"原版已通过全部条件（有 {0} 位已入住 NPC）但仍没生成：活跃 NPC {1}/{2}", homes, active, Main.maxNPCs);
    }

    /// <summary>最近一次原版生成的结果说明，status 里会显示。</summary>
    private static string LastVanillaBlock { get; set; } = "尚未尝试";

    // ------------------------------------------------------------------ 场上查询

    private static int CountMerchants()
    {
        int count = 0;

        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc != null && npc.active && npc.life > 0 && npc.netID == NPCID.TravellingMerchant)
                count++;
        }

        return count;
    }

    private static string Where()
    {
        var spots = new List<string>();

        for (int i = 0; i < Main.maxNPCs && spots.Count < 5; i++)
        {
            NPC? npc = Main.npc[i];
            if (npc == null || !npc.active || npc.life <= 0 || npc.netID != NPCID.TravellingMerchant)
                continue;

            spots.Add($"({(int)(npc.position.X / 16f)}, {(int)(npc.position.Y / 16f)})");
        }

        return spots.Count == 0 ? GetString("旅商当前不在场") : string.Join("、", spots);
    }

    /// <summary>当前"能不能来"的诊断文案，供 status 使用。</summary>
    private static bool InWindow() =>
        Main.dayTime && Main.time < ArrivalWindowMinutes * 60.0 && !Main.IsFastForwardingTime();

    private static string BlockReason(bool inWindow)
    {
        if (!Main.dayTime)
            return GetString("现在是夜晚（原版入夜后不再来）");
        if (Main.IsFastForwardingTime())
            return GetString("日晷 / Moondial 生效中（原版此时不掷骰）");
        if (!inWindow)
            return GetString($"已过上午4:30 + {0} 分钟", ArrivalWindowMinutes);
        if (Main.eclipse)
            return GetString("月食天不生成旅商");

        int town = CountTownNpcs();
        if (town < 2)
            return GetString($"城镇里只有 {0} 位 NPC，原版要求至少 2 位", town);

        Town(out _, out int housed, out int withRoom);
        if (housed == 0 && withRoom == 0)
            return GetString($"城镇里有 {0} 位 NPC，但都没住进房子，原版没有落脚点（静默等待中）", town);

        return GetString("条件已满足，正在按原版生成");
    }

    // ------------------------------------------------------------------ 命令

    private void RegisterCommand()
    {
        RemoveCommand();
        _command = new Command(CommandPermission, OnCommand, "merchant", "旅商", "dailymerchant")
        {
            HelpText = GetString("让旅商每天到访（/merchant summon|despawn|check|status）"),
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
        string sub = args.Parameters.Count > 0 ? args.Parameters[0].ToLowerInvariant() : "help";

        switch (sub)
        {
            case "help" or "帮助" or "?":
                SendHelp(player);
                break;

            case "summon" or "s" or "召唤" or "来":
                if (CountMerchants() > 0)
                {
                    player.SendInfoMessage(GetString("场上已经有旅商了。"));
                }
                else if (TryVanillaSpawn())
                {
                    player.SendInfoMessage($"§e[旅商] §7{GetString("已召唤，旅商现在在城镇里。")}");
                }
                else
                {
                    // 召唤是管理员显式操作，这里必须说清楚为什么没成。
                    player.SendErrorMessage($"§e[旅商] §7{GetString("召唤失败：")}{BlockReason(InWindow())}。");
                }

                break;

            case "despawn" or "d" or "离开" or "送走":
                if (CountMerchants() == 0)
                {
                    player.SendInfoMessage(GetString("场上本来就没有旅商。"));
                    break;
                }

                WorldGen.UnspawnTravelNPC();   // 原版自己播报"xxx 离开了"
                _arrivedThisDay = false;
                player.SendInfoMessage($"§e[旅商] §7{GetString("已请旅商离场。")}");
                break;

            case "status" or "st" or "状态":
                SendStatus(player);
                break;

            case "check" or "tick" or "检查" or "判定":
                // 空服时世界不推进、自动判定不会触发，这里手动跑一次（也是自动化测试的入口）。
                _scanCountdown = 20;
                RunScan();
                player.SendInfoMessage($"§e[旅商] §7{GetString("已手动判定一次：")}§f{BlockReason(InWindow())}§7");
                break;

            default:
                SendHelp(player);
                break;
        }
    }

    private static void SendHelp(TSPlayer player)
    {
        player.SendInfoMessage("§e[旅商] §7" + GetString("指令用法："));
        player.SendInfoMessage("§f/merchant summon §7— " + GetString("立刻召唤旅商（/merchant 召唤）"));
        player.SendInfoMessage("§f/merchant despawn §7— " + GetString("请旅商离场（/merchant 离开）"));
        player.SendInfoMessage("§f/merchant check §7— " + GetString("手动跑一次到访判定（/merchant 检查）"));
        player.SendInfoMessage("§f/merchant status §7— " + GetString("查看到访状态与城镇条件（/merchant 状态）"));
    }

    private void SendStatus(TSPlayer player)
    {
        int alive = CountMerchants();
        bool inWindow = InWindow();
        Town(out int town, out int housed, out int withRoom);

        player.SendInfoMessage($"§e[旅商] §7{GetString("启用中")} · " +
                               $"{GetString("旅商 NPC ID =")} §f{NPCID.TravellingMerchant}§7 · " +
                               $"{GetString("每天到访")} §f100%§7（{GetString("原版 22.12%")}）· " +
                               $"{GetString("窗口")} §f{GetString("上午4:30 起")} {ArrivalWindowMinutes} {GetString("分钟")}§7");
        player.SendInfoMessage($"§e[旅商] §7{GetString("当前：")}{(Main.dayTime ? "白天" : "夜晚")} §f{Clock()}§7，" +
                               $"{GetString("场上旅商")} §f{alive}§7 {GetString("位")}，{GetString("位置")} §f{Where()}§7");
        player.SendInfoMessage($"§e[旅商] §7{GetString("城镇：")}§f{town}§7 {GetString("位 NPC（已入住")} §f{housed}§7，" +
                               $"{GetString("待搬入空房")} §f{withRoom}§7）· {GetString("判定已跑")} §f{_scanCount}§7 " +
                               $"{GetString("次（每秒 1 次自动，空服不跑）")}");
        player.SendInfoMessage($"§e[旅商] §7{GetString("判定：")}§f{BlockReason(inWindow)}§7");
        player.SendInfoMessage($"§e[旅商] §7{GetString("原版生成：")}§f{LastVanillaBlock}§7");
    }
}
