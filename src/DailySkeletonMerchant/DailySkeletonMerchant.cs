using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

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
/// 出现位置：种子是"按队伍分配出生点"时（<c>Main.teamBasedSpawnsSeed</c>）会有多个出生点，
/// 随机挑一个；普通种子用世界出生点。出生点本身常常不能站人（墙里、水里、地牢砖、
/// 悬空平台），所以每个出生点都要再往外找一个能站稳的位置。
///
/// 与洞穴里自然刷出的骷髅商人不冲突：场上已经有 453 时插件当天不干预、不生成第二只。
/// 全程无指令、无广播、无配置文件。
/// </summary>
[ApiVersion(2, 1)]
public class DailySkeletonMerchant : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "不是现在";
    public override string Description => GetString("骷髅商人每天上午出现在出生点附近");
    public override Version Version => new(1, 0);

    private const string LogPrefix = "[DailySkeletonMerchant] ";

    /// <summary>骷髅商人 NPC ID（原版常数）。</summary>
    private const int MerchantId = NPCID.SkeletonMerchant;

    /// <summary>到访窗口：上午 4:30 起 450 分钟 = 中午 12 点。</summary>
    private const int ArrivalWindowMinutes = 450;

    /// <summary>
    /// 判定间隔（毫秒）。用真实时间而不是"每 N 帧"：GameUpdate 每帧触发一次，
    /// 60 FPS 下每秒 60 次，按 20 帧算只有 0.33 秒。
    /// </summary>
    private const long ScanIntervalMs = 1000;

    /// <summary>锚点搜索：以出生点为圆心向外找可站立的位置，起始圈数与常规最大圈数。</summary>
    private const int AnchorSearchStart = 2;
    private const int AnchorSearchRadius = 40;

    /// <summary>所有出生点都找不到位置时的兜底搜索半径（有些种子的出生点周围几十格都没法站）。</summary>
    private const int AnchorSearchFallbackRadius = 120;

    /// <summary>锚点与地图边缘保持的距离，避免卡在边界或掉出世界。</summary>
    private const int MapMargin = 25;

    /// <summary>
    /// 离场半径（像素）。附近多少像素内没有在线玩家就送走，
    /// 取原版 NPC 反卸载半径（800）保持一致。
    /// </summary>
    private const float LeaveRadiusPx = 800f;

    private long _lastScanAt;
    private bool _worldReady;

    /// <summary>今天已经安排过了（成功生成过，或因为场上已有骷髅商人不干预）。</summary>
    private bool _handledThisDay;
    private bool _handledAtDay;
    private double _handledAtTime;
    private int _handledAtMoonPhase;

    /// <summary>插件自己生成的那只的槽位；-1 表示当前没有"我们的"骷髅商人。</summary>
    private int _spawnedIndex = -1;

    public DailySkeletonMerchant(Main game) : base(game) { }

    // ------------------------------------------------------------------ 生命周期

    public override void Initialize()
    {
        ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);

        TShock.Log.ConsoleInfo(
            $"{LogPrefix}v{Version} 已加载：骷髅商人 NPC ID = {MerchantId}，" +
            $"每天上午 4:30 起在出生点附近出现一次，玩家走远即离场、当天不再出现、死亡不补位，" +
            $"货品沿用原版（每天清晨随月相自动换货）。");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);

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
    /// 否则空服时第一个判定来自管理命令，会把命令刚记下的东西顺手清掉。
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

    /// <summary>一次到访判定。每秒自动跑一次（有人在线时）。</summary>
    private void RunScan()
    {
        UpdateDayState();

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

        // 场上已经有骷髅商人（洞穴里刷到的、昨天留下的、管理员召唤的）：
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

        if (!TryFindAnchor(out Point tile, out Point spawn))
        {
            TShock.Log.ConsoleDebug($"{LogPrefix}出生点附近找不到能站立的位置（常规半径 {AnchorSearchRadius} 格、兜底半径 {AnchorSearchFallbackRadius} 格都试过），本次跳过。");
            return;
        }

        if (!SpawnAt(tile, out int index))
        {
            TShock.Log.ConsoleDebug($"{LogPrefix}生成失败（NPC 槽位不足或位置被占），稍后重试。");
            return;
        }

        _spawnedIndex = index;
        MarkHandled();
        TShock.Log.ConsoleDebug($"{LogPrefix}骷髅商人已出现：出生点 ({spawn.X}, {spawn.Y}) → 位置 ({tile.X}, {tile.Y})。");
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

    // ------------------------------------------------------------------ 出生点与锚点

    /// <summary>
    /// 找一个能站人的出现位置。返回实际选中的出生点，便于排查。
    ///
    /// 步骤：收集候选出生点 → 随机排序 → 每个出生点先按常规半径找 → 全都不行再放宽半径重来一次。
    /// 候选出生点本身往往是站不住人的（世界生成只保证"大致能站"），所以每个都要再搜一圈。
    /// </summary>
    private static bool TryFindAnchor(out Point tile, out Point spawn)
    {
        tile = Point.Zero;
        spawn = Point.Zero;

        List<Point> candidates = SpawnCandidates();
        if (candidates.Count == 0)
            return false;

        foreach (Point candidate in candidates)
        {
            if (SearchNear(candidate, AnchorSearchRadius, out tile))
            {
                spawn = candidate;
                return true;
            }
        }

        // 兜底：有些种子的出生点周围几十格都站不住人，放宽到 120 格再试一遍。
        foreach (Point candidate in candidates)
        {
            if (SearchNear(candidate, AnchorSearchFallbackRadius, out tile))
            {
                spawn = candidate;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 出生点候选：
    /// - "按队伍分配出生点"的种子（Main.teamBasedSpawnsSeed）有多个出生点，
    ///   存在 ExtraSpawnPointManager.extraSpawnPoints 里（下标 0 是主出生点），随机挑一个；
    /// - 普通种子只有世界出生点 Main.spawnTileX / Main.spawnTileY。
    /// 返回的顺序是随机的，调用方按顺序试即可。
    /// </summary>
    private static List<Point> SpawnCandidates()
    {
        var list = new List<Point>();

        if (Main.teamBasedSpawnsSeed)
        {
            Point[] extra = ExtraSpawnPointManager.extraSpawnPoints;
            for (int i = 0; i < extra.Length; i++)
            {
                Point p = extra[i];
                if (p.X <= 0 || p.Y <= 0 || p.X >= Main.maxTilesX || p.Y >= Main.maxTilesY)
                    continue;

                if (!list.Contains(p))
                    list.Add(p);
            }
        }

        if (list.Count == 0)
        {
            Point main = new(Main.spawnTileX, Main.spawnTileY);
            if (main.X > 0 && main.Y > 0 && main.X < Main.maxTilesX && main.Y < Main.maxTilesY)
                list.Add(main);
        }

        // Fisher-Yates：多出生点种子每天随机落在其中一个。
        Random rng = Random.Shared;
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }

        return list;
    }

    /// <summary>
    /// 以某个出生点为圆心，由近及远逐圈查找能站立的位置。
    /// 顺序只取决于出生点坐标，所以同一个出生点每次算出来的锚点都是同一个格子。
    /// </summary>
    private static bool SearchNear(Point center, int maxRadius, out Point tile)
    {
        tile = Point.Zero;

        for (int r = AnchorSearchStart; r <= maxRadius; r++)
        {
            for (int dy = -r; dy <= r; dy++)
            {
                for (int dx = -r; dx <= r; dx++)
                {
                    if (Math.Abs(dx) != r && Math.Abs(dy) != r)
                        continue;   // 只看当前这一圈

                    if (!CanStand(center.X + dx, center.Y + dy))
                        continue;

                    tile = new Point(center.X + dx, center.Y + dy);
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 能不能站人：脚下是实地、脚下没有液体、自己和两侧身体位置没有方块也没有水。
    /// 额外排掉地牢砖和 226 号砖（蓝砖）——原版挑队伍出生点时同样排掉它们。
    /// </summary>
    private static bool CanStand(int x, int y)
    {
        if (x < MapMargin || x >= Main.maxTilesX - MapMargin ||
            y < 20 || y >= Main.maxTilesY - MapMargin)
            return false;

        if (!WorldGen.SolidTile(x, y + 1))
            return false;

        // 脚下这块不能是地牢 / 蓝砖。
        ITile floor = Main.tile[x, y + 1];
        if (Main.tileDungeon[floor.type] || floor.type == 226)
            return false;

        if (WorldGen.SolidTile(x, y) || WorldGen.SolidTile(x, y - 1))
            return false;

        if (WorldGen.SolidTile(x - 1, y) || WorldGen.SolidTile(x + 1, y))
            return false;

        if (Main.tile[x, y].liquid > 0 || Main.tile[x, y - 1].liquid > 0)
            return false;

        if (Main.tile[x - 1, y].liquid > 0 || Main.tile[x + 1, y].liquid > 0)
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

    private static bool AnyPlayerNear(Vector2 center, float radius)
    {
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            Player? player = Main.player[i];
            if (player != null && player.active && Vector2.Distance(player.Center, center) <= radius)
                return true;
        }

        return false;
    }
}