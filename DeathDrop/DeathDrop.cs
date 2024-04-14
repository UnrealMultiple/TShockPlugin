using System;
using System.IO;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace DeathDrop
{
    [ApiVersion(2, 1)]
    public class DeathDrop : TerrariaPlugin
    {
        public static Random RandomGenerator = new Random();

        public override string Author => "大豆子，肝帝熙恩更新优化";
        public override string Description => "自定义怪物死亡随机掉落物";
        public override string Name => "死亡随机掉落";
        public override Version Version => new Version(1, 0, 1);

        public DeathDrop(Main game) : base(game) { }

        public override void Initialize()
        {
            ServerApi.Hooks.NpcKilled.Register(this, NPCDead);
        }

        private void NPCDead(NpcKilledEventArgs args)
        {
            int npcNetID = args.npc.netID;
            Vector2 npcPosition = args.npc.position;

            // 获取击杀者玩家（可能为 null）
            TSPlayer player = args.npc.lastInteraction != 255 && args.npc.lastInteraction >= 0
                ? TShock.Players[args.npc.lastInteraction]
                : null;

            try
            {
                DeathDropConfig config = DeathDropConfig.GetConfig();

                if (config.EnableRandomDrops)
                {
                    int itemId = GetRandomItemIdFromGlobalConfig(config);

                    if (Candorp(config.RandomDropChance))
                    {
                        Item item = TShock.Utils.GetItemById(itemId);
                        int dropAmount = RandomGenerator.Next(config.MinRandomDropAmount, config.MaxRandomDropAmount + 1);

                        int itemNumber = Item.NewItem(
                            null,
                            (int)args.npc.position.X,
                            (int)args.npc.position.Y,
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

                if (config.EnableCustomDrops)
                {
                    foreach (DeathDropConfig.Monster monster in config.DeathDropSet)
                    {
                        if (monster.NPCID == npcNetID && Candorp(monster.DropChance))
                        {
                            int dropItemId = GetRandomItemIdFromMonsterConfig(monster);

                            Item dropItem = TShock.Utils.GetItemById(dropItemId);
                            int dropAmount = RandomGenerator.Next(monster.RandomDropMinAmount, monster.RandomDropMaxAmount + 1);

                            int dropItemNumber = Item.NewItem(
                                null,
                                (int)npcPosition.X,
                                (int)npcPosition.Y,
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
            catch (Exception ex)
            {
                Console.WriteLine($"死亡掉落插件发生异常：{ex.Message}");
            }
        }

        private int GetRandomItemIdFromGlobalConfig(DeathDropConfig config)
        {
            if (config.FullRandomDrops)
            {
                int itemId = RandomGenerator.Next(1, 5453);

                while (config.FullRandomExcludedItems.Contains(itemId))
                {
                    itemId = RandomGenerator.Next(1, 5453);
                }

                return itemId;
            }
            else
            {
                int randomIndex = RandomGenerator.Next(config.CommonRandomDrops.Count);
                return config.CommonRandomDrops[randomIndex];
            }
        }

        private int GetRandomItemIdFromMonsterConfig(DeathDropConfig.Monster monster)
        {
            if (monster.FullRandomDrops)
            {
                int itemId = RandomGenerator.Next(1, 5453);

                while (monster.FullRandomExcludedItems.Contains(itemId))
                {
                    itemId = RandomGenerator.Next(1, 5453);
                }

                return itemId;
            }
            else
            {
                int randomIndex = RandomGenerator.Next(monster.CommonRandomDrops.Count);
                return monster.CommonRandomDrops[randomIndex];
            }
        }

        private static bool Candorp(int probability)
        {
            return RandomGenerator.Next(100) <= probability;
        }
    }
}