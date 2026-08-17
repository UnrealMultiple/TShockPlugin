using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace RandomFishingLoot;

[ApiVersion(2, 1)]
public sealed partial class RandomFishingLoot : TerrariaPlugin
{
    private const string AdminPermission = "fishrand.admin";

    private readonly List<Command> _commands = new();
    private readonly Dictionary<int, PendingLootChoice> _pendingChoices = new();
    private readonly Dictionary<ProjectileKey, PendingNetworkCatch> _networkCatches = new();
    private NpcPool _npcPool = NpcPool.Empty;
    private FishingLootConfig _config = FishingLootConfig.CreateDefault();
    private string _configPath = "";
    private string _loadSummary = "未加载";
    private List<string> _configWarnings = new();

    public override string Name => "随机渔获";
    public override string Author => "愚蠢";
    public override string Description => "按当前进度阶段替换钓到的物品。";
    public override Version Version => new(2, 5, 0);

    public RandomFishingLoot(Main game) : base(game)
    {
        Order = 44;
    }

    public override void Initialize()
    {
        _configPath = ResolveConfigPath();
        InitializeConfig();

        Register(new Command(FishRandCommand, "fishrand")
        {
            HelpText = "随机渔获管理。用法：/fishrand [reload|sample [数量]|stages]"
        });

        HookEvents.Terraria.Projectile.FishingCheck_RollItemDrop += OnFishingRollItemDrop;
        HookEvents.Terraria.Projectile.FishingCheck_RollEnemySpawns += OnFishingRollEnemySpawns;
        HookEvents.Terraria.Projectile.AI_061_FishingBobber_GiveItemToPlayer += OnFishingGiveItem;

        GetDataHandlers.NewProjectile.Register(OnNewProjectilePacket);
        GetDataHandlers.ProjectileKill.Register(OnProjectileKillPacket);
        ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _pendingChoices.Clear();
            _networkCatches.Clear();

            HookEvents.Terraria.Projectile.FishingCheck_RollItemDrop -= OnFishingRollItemDrop;
            HookEvents.Terraria.Projectile.FishingCheck_RollEnemySpawns -= OnFishingRollEnemySpawns;
            HookEvents.Terraria.Projectile.AI_061_FishingBobber_GiveItemToPlayer -= OnFishingGiveItem;

            GetDataHandlers.NewProjectile.UnRegister(OnNewProjectilePacket);
            GetDataHandlers.ProjectileKill.UnRegister(OnProjectileKillPacket);
            ServerApi.Hooks.ServerLeave.Deregister(this, OnServerLeave);

            foreach (Command command in _commands)
                Commands.ChatCommands.Remove(command);
            _commands.Clear();
        }

        base.Dispose(disposing);
    }

    private void OnFishingRollItemDrop(Terraria.Projectile sender, HookEvents.Terraria.Projectile.FishingCheck_RollItemDropEventArgs args)
    {
        if (!_config.Enabled || !UseProgressionItemMode())
            return;

        // 原版 "crate"（宝匣）掉落层级命中时保留宝匣。原版判定宝匣优先于任务鱼，
        // 所以这个检查放在任务鱼之前，直接交还原版逻辑产出宝匣。
        if (_config.AllowCrates && args.fisher.crate)
            return;

        // 玩家处于渔夫任务中且原版 "uncommon" 掉落层级命中时，保留任务鱼，
        // 否则任务鱼会被下面的自定义渔获表完全覆盖，永远钓不到。
        if (_config.AllowQuestFish && args.fisher.questFish > 0 && args.fisher.uncommon)
        {
            FishingAttempt questFisher = args.fisher;
            questFisher.rolledEnemySpawn = 0;
            questFisher.rolledItemDrop = questFisher.questFish;
            args.fisher = questFisher;
            args.ContinueExecution = false;
            _pendingChoices[sender.whoAmI] = MakeQuestFishChoice(questFisher.questFish);
            return;
        }

        PendingLootChoice? choice = RollCurrentLootChoice();
        if (choice == null)
            return;

        FishingAttempt fisher = args.fisher;
        fisher.rolledEnemySpawn = 0;
        fisher.rolledItemDrop = choice.ItemId;
        args.fisher = fisher;
        args.ContinueExecution = false;
        _pendingChoices[sender.whoAmI] = choice;
    }

    private void OnFishingRollEnemySpawns(Terraria.Projectile sender, HookEvents.Terraria.Projectile.FishingCheck_RollEnemySpawnsEventArgs args)
    {
        if (!_config.Enabled || !UseRandomNpcMode() || _npcPool.Count == 0)
            return;

        if (!ShouldReplaceWithNpc())
            return;

        PendingNpcChoice? choice = _npcPool.Next();
        if (choice == null)
            return;

        FishingAttempt fisher = args.fisher;
        fisher.rolledEnemySpawn = choice.NpcId;
        args.fisher = fisher;
        args.ContinueExecution = false;
    }

    private void OnFishingGiveItem(Terraria.Projectile sender, HookEvents.Terraria.Projectile.AI_061_FishingBobber_GiveItemToPlayerEventArgs args)
    {
        if (!_config.Enabled || args.thePlayer == null)
            return;

        // 钓上来的是宝匣时直接放行，避免下面的自定义渔获表把它顶掉。
        if (_config.AllowCrates && IsCrateItem(args.itemType))
            return;

        if (UseRandomNpcMode())
        {
            if (!ShouldReplaceWithNpc())
                return;

            PendingNpcChoice? npcChoice = _npcPool.Next();
            if (npcChoice == null)
                return;

            args.ContinueExecution = false;
            SpawnFishingNpc(sender, args.thePlayer, npcChoice);
            return;
        }

        if (!UseProgressionItemMode())
            return;

        if (!_pendingChoices.Remove(sender.whoAmI, out PendingLootChoice? choice))
            choice = RollCurrentLootChoice();

        if (choice == null)
            return;

        args.ContinueExecution = false;

        int stack = RollStack(choice.MinStack, choice.MaxStack, choice.ItemMaxStack);
        args.thePlayer.QuickSpawnItem(new EntitySource_FishedOut(sender), choice.ItemId, stack);

        if (!_config.AnnounceToPlayer)
            return;

        TSPlayer? player = PlayerByIndex(args.thePlayer.whoAmI);
        if (player?.Active == true)
            player.SendInfoMessage($"本次渔获：{choice.DisplayName} x{stack} [{choice.StageName}]");
    }

    private static void SpawnFishingNpc(Terraria.Projectile sender, Player player, PendingNpcChoice choice)
    {
        int spawnX = (int)player.Center.X;
        int spawnY = (int)player.position.Y - 96;
        int npcIndex = NPC.NewNPC(
            new EntitySource_FishedOut(sender),
            spawnX,
            spawnY,
            choice.NpcId,
            Target: player.whoAmI);

        if (npcIndex < 0 || npcIndex >= Main.maxNPCs)
            return;

        NPC npc = Main.npc[npcIndex];
        npc.target = player.whoAmI;
        npc.netUpdate = true;

        if (Main.netMode == 2)
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npcIndex);
    }

    private void Register(Command command)
    {
        _commands.Add(command);
        Commands.ChatCommands.Add(command);
    }

    private void InitializeConfig()
    {
        if (TryLoadConfig(out _))
            return;

        _config = FishingLootConfig.CreateDefault();
        _loadSummary = "已回退到默认进度渔获表。";
        SaveConfig(_config);
    }

    private bool UseProgressionItemMode()
    {
        return string.Equals(_config.Mode, "progression_items", StringComparison.OrdinalIgnoreCase)
            || string.Equals(_config.Mode, "items", StringComparison.OrdinalIgnoreCase);
    }

    private bool UseRandomNpcMode()
    {
        return string.Equals(_config.Mode, "random_npcs", StringComparison.OrdinalIgnoreCase)
            || string.Equals(_config.Mode, "npcs", StringComparison.OrdinalIgnoreCase)
            || string.Equals(_config.Mode, "npc", StringComparison.OrdinalIgnoreCase);
    }

    private bool ShouldReplaceWithNpc()
    {
        int chance = Math.Clamp(_config.RandomNpc.ReplaceChancePercent, 0, 100);
        return chance >= 100 || Random.Shared.Next(100) < chance;
    }
}
