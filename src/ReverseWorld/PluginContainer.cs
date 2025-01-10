using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ReverseWorld
{
    [ApiVersion(2, 1)]
    public class PluginContainer : TerrariaPlugin
    {
        public static void ReverseWorld()
        {
            var chestList = new List<Chest>(Main.chest);
            foreach (var chest in chestList)
            {
                if (chest != null)
                {
                    ReverseChest(chest);
                }
            }

            int underworldLayer = Main.UnderworldLayer;
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < underworldLayer / 2; y++)
                {
                    if (Main.tile[x, y] != null && Main.tile[x, underworldLayer - 1 - y] != null)
                    {
                        var tempTile = (ITile)Main.tile[x, y].Clone();
                        Main.tile[x, y] = (ITile)Main.tile[x, underworldLayer - 1 - y].Clone();
                        Main.tile[x, underworldLayer - 1 - y] = tempTile;
                    }
                }
            }

            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < underworldLayer; y++)
                {
                    var tile = Main.tile[x, y];
                    if (tile != null)
                    {
                        byte slope = tile.slope();
                        switch (slope)
                        {
                            case 1:
                                slope = 3;
                                break;
                            case 2:
                                slope = 4;
                                break;
                            case 3:
                                slope = 1;
                                break;
                            case 4:
                                slope = 2;
                                break;
                        }
                        tile.slope(slope);
                    }
                }
            }
        }

        private static void ReverseChest(Chest chest)
        {
            int maxX = Main.maxTilesX - 1;
            int maxY = Main.maxTilesY - 1;

            // 确保 chest.x 和 chest.y 在有效范围内
            if (chest.x >= 0 && chest.x < maxX && chest.y >= 0 && chest.y < maxY)
            {
                // 反转 chest 的位置
                chest.y = Main.UnderworldLayer - 1 - chest.y - 1;
            }
        }

        public static void Method(CommandArgs args)
        {
            ReverseWorld();
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("tofout.reverseworld", Method, new string[] { "reverseworld", "rw", "反转世界" })
            {
                HelpText = "反转整个世界的方向和地形"
            });

            Commands.ChatCommands.Add(new Command("tofout.placelandmine", (cmd) =>
            {
                Code.Method(cmd.Player, cmd.Parameters);
            }, new string[] { "placelandmine", "plm", "放置地雷" })
            {
                HelpText = "在玩家当前位置放置地雷"
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Commands.ChatCommands.RemoveAll(cmd => cmd.HasAlias("reverseworld"));
            Commands.ChatCommands.RemoveAll(cmd => cmd.HasAlias("placelandmine"));
        }

        public PluginContainer(Main game) : base(game) { }

        public override string Author => "1413, 肝帝熙恩适配1449";

        public override string Name => "世界反转插件";

        public override Version Version => new Version(1, 0, 0);

    }
}

