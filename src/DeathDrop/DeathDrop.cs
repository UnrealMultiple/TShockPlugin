using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace DeathDrop;

[ApiVersion(2, 1)]
public class DeathDrop : TerrariaPlugin
{
    public static Random RandomGenerator = new Random();
    public static Configuration Config = null!;

    public override string Author => "大豆子，肝帝熙恩更新优化";
    public override string Description => GetString("怪物死亡随机和自定义掉落物品");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override Version Version => new Version(1, 0, 6);

    public DeathDrop(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        GeneralHooks.ReloadEvent += ReloadConfig;
        ServerApi.Hooks.NpcKilled.Register(this, this.NPCDead);
        LoadConfig();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
            ServerApi.Hooks.NpcKilled.Deregister(this, this.NPCDead);
        }
        base.Dispose(disposing);
    }
    private static void LoadConfig()
    {
        Config = Configuration.Read(Configuration.FilePath);
        Config.Write(Configuration.FilePath);
    }

    private static void ReloadConfig(ReloadEventArgs args)
    {
        LoadConfig();
        args.Player?.SendSuccessMessage(GetString("[死亡掉落] 重新加载配置完毕。"));
    }

    private void NPCDead(NpcKilledEventArgs args)
    {
        var npcNetID = args.npc.netID;
        var npcPosition = args.npc.position;

        // 获取击杀者玩家（可能为 null）
        var player = args.npc.lastInteraction != 255 && args.npc.lastInteraction >= 0
            ? TShock.Players[args.npc.lastInteraction]
            : null;

        if (Config.EnableRandomDrops)
        {
            if (this.GetRandomItemIdFromGlobalConfig(Config) == 0)
            {
                return;
            }
            var itemId = this.GetRandomItemIdFromGlobalConfig(Config);

            if (Candorp(Config.RandomDropChance))
            {
                var item = TShock.Utils.GetItemById(itemId);
                var dropAmount = RandomGenerator.Next(Config.MinRandomDropAmount, Config.MaxRandomDropAmount + 1);

                var itemNumber = Item.NewItem(
                    null,
                    (int) args.npc.position.X,
                    (int) args.npc.position.Y,
                    item.width,
                    item.height,
                    item.type,
                    dropAmount
                );

                // 只有当玩家不为 null 时才发送数据包
                if (player != null)
                {
                    player.SendData(PacketTypes.SyncExtraValue, null, itemNumber);
                    player.SendData(PacketTypes.ItemOwner, null, itemNumber);
                    player.SendData(PacketTypes.TweakItem, null, itemNumber, 255f, 63f);
                }
            }
        }

        if (Config.EnableCustomDrops)
        {
            foreach (var monster in Config.DeathDropSet)
            {
                if (monster.NPCID == npcNetID && Candorp(monster.DropChance))
                {
                    var dropItemId = this.GetRandomItemIdFromMonsterConfig(monster);

                    var dropItem = TShock.Utils.GetItemById(dropItemId);
                    var dropAmount = RandomGenerator.Next(monster.RandomDropMinAmount, monster.RandomDropMaxAmount + 1);

                    var dropItemNumber = Item.NewItem(
                        null,
                        (int) npcPosition.X,
                        (int) npcPosition.Y,
                        dropItem.width,
                        dropItem.height,
                        dropItem.type,
                        dropAmount
                    );

                    // 只有当玩家不为 null 时才发送数据包
                    if (player != null)
                    {
                        player.SendData(PacketTypes.SyncExtraValue, null, dropItemNumber);
                        player.SendData(PacketTypes.ItemOwner, null, dropItemNumber);
                        player.SendData(PacketTypes.TweakItem, null, dropItemNumber, 255f, 63f);
                    }
                }
            }
        }
    }

    private int GetRandomItemIdFromGlobalConfig(Configuration config)
    {
        if (config.FullRandomDrops)
        {
            int itemId;
            do
            {
                itemId = RandomGenerator.Next(1, 5453);
            } while (config.FullRandomExcludedItems.Contains(itemId));

            return itemId;
        }
        else
        {
            // 检查CommonRandomDrops列表是否非空
            if (config.CommonRandomDrops.Count > 0)
            {
                var randomIndex = RandomGenerator.Next(config.CommonRandomDrops.Count);
                return config.CommonRandomDrops[randomIndex];
            }
            else
            {
                return 0;
            }
        }
    }

    private int GetRandomItemIdFromMonsterConfig(Configuration.Monster monster)
    {
        if (monster.FullRandomDrops)
        {
            var itemId = RandomGenerator.Next(1, 5453);

            while (monster.FullRandomExcludedItems.Contains(itemId))
            {
                itemId = RandomGenerator.Next(1, 5453);
            }

            return itemId;
        }
        else
        {
            if (monster.CommonRandomDrops.Count > 0)
            {
                var randomIndex = RandomGenerator.Next(monster.CommonRandomDrops.Count);
                return monster.CommonRandomDrops[randomIndex];
            }
            else
            {
                return 0;
            }
        }
    }

    private static bool Candorp(int probability)
    {
        return RandomGenerator.Next(100) <= probability;
    }
}