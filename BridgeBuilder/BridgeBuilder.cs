using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace BridgeBuilder
{
    [ApiVersion(2, 1)]
    public class BridgeBuilder : TerrariaPlugin
    {
        public override string Name => "BridgeBuilder";
        public override Version Version => new Version(1, 0, 6);
        public override string Author => "Soofa，肝帝熙恩汉化1449";
        public override string Description => "铺桥!";
        public static Configuration Config;
        public BridgeBuilder(Main game) : base(game)
        {
            LoadConfig();
        }

        private static void LoadConfig()
        {
            Config = Configuration.Read(Configuration.FilePath);
            Config.Write(Configuration.FilePath);

        }
        private static void ReloadConfig(ReloadEventArgs args)
        {
            LoadConfig();
            args.Player?.SendSuccessMessage("[铺桥] 重新加载配置完毕。");
        }

        public override void Initialize()
        {
            GeneralHooks.ReloadEvent += ReloadConfig;
            TShockAPI.Commands.ChatCommands.Add(new("bridgebuilder.bridge", BridgeCmd, "bridge", "桥来")
            {
                AllowServer = false,
                HelpText = "朝着你看的方向建造桥梁。（你需要持有一定数量的平台或团队块或种植盆。）"
            });
        }

        private async void BridgeCmd(CommandArgs args)
        {
            await Task.Run(() =>
            {
                TSPlayer plr = args.Player;
                int directionX = plr.TPlayer.direction;
                int directionY = 0;

                if (args.Parameters.Count > 0)
                {
                    string direction = args.Parameters[0].ToLower();
                    if (direction == "up" || direction == "down")
                    {
                        directionY = direction == "up" ? -1 : 1; // 确保正确的垂直方向
                        directionX = 0; // 在垂直建造时，水平方向不移动
                    }
                }

                int startX = directionX == 0 ? plr.TileX : (directionX == -1 ? plr.TileX - 1 : plr.TileX + 2);
                int i = 0;
                int j = plr.TileY + (directionY == -1 ? -1 : (directionY == 1 ? 3 : 3));

                Item selectedItem = plr.SelectedItem;

                if (selectedItem.createTile < 0 && selectedItem.createWall < 0)
                {
                    plr.SendErrorMessage("你手持的物品无法放置。");
                    return;
                }

                bool isTile = selectedItem.createTile >= 0;
                int styleId = selectedItem.placeStyle;

                if (j < 0 || j >= Main.maxTilesY)
                {
                    plr.SendErrorMessage("无法在这里建造桥梁。");
                    return;
                }

                int placedCount = 0;

                while (CheckTileAvailability(startX + i * directionX, j, plr) && selectedItem.stack > 0)
                {
                    if (isTile)
                    {
                        WorldGen.PlaceTile(startX + i * directionX, j, selectedItem.createTile, false, true, -1, styleId);
                    }
                    else
                    {
                        Main.tile[startX + i * directionX, j].wall = (ushort)selectedItem.createWall;
                    }

                    TSPlayer.All.SendTileRect((short)(startX + i * directionX), (short)j, 1, 1);
                    plr.SelectedItem.stack--;
                    i++;
                    if (directionY != 0) // 只有在垂直建造时改变Y坐标
                    {
                        j += directionY;
                    }
                    placedCount++;
                }

                if (placedCount > 0)
                {
                    plr.SendSuccessMessage($"{placedCount}格桥梁建造完成！");
                }
                else
                {
                    plr.SendErrorMessage("没有足够的空间或物品来建造桥梁。");
                }

                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(plr.SelectedItem.Name), plr.Index, plr.TPlayer.selectedItem);
                NetMessage.SendData((int)PacketTypes.PlayerSlot, plr.Index, -1, NetworkText.FromLiteral(plr.SelectedItem.Name), plr.Index, plr.TPlayer.selectedItem);
            });
        }

        private static bool CheckTileAvailability(int x, int y, TSPlayer plr, int directionX = 0, int directionY = 0)
        {
            bool canPlace = x < Main.maxTilesX && x >= 0 && y < Main.maxTilesY && y >= 0 &&
                            Math.Abs(directionX != 0 ? plr.TileX - x : plr.TileY - y) < Config.MaxPlaceLength &&
                            plr.SelectedItem.stack > 0 &&
                            !TShock.Regions.InArea(x, y);

            if (plr.SelectedItem.createTile < 0 && plr.SelectedItem.createWall >= 0) // 检查墙壁
            {
                canPlace &= Main.tile[x, y].wall == 0;
            }
            else if (plr.SelectedItem.createTile >= 0) // 检查物块
            {
                canPlace &= !Main.tile[x, y].active();
            }

            return canPlace;
        }

    }
}

