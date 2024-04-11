using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace SFactions
{
    [ApiVersion(2, 1)]
    public class SFactions : TerrariaPlugin
    {
        public override string Name => "BridgeBuilder";
        public override Version Version => new Version(1, 0, 4);
        public override string Author => "Soofa，肝帝熙恩汉化1449";
        public override string Description => "铺桥!";
        public static Configuration Config;
        public SFactions(Main game) : base(game)
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
            TShockAPI.Commands.ChatCommands.Add(new("bridgebuilder.bridge", BridgeCmd, "bridge" , "桥来")
            {
                AllowServer = false,
                HelpText = "朝着你看的方向建造桥梁。（你需要持有一定数量的平台或团队块或种植盆。）"
            });
        }

        private async void BridgeCmd(CommandArgs args)
        {
            await Task.Run(() => {
                TSPlayer plr = args.Player;
                int direction = plr.TPlayer.direction;
                int startX = direction == -1 ? plr.TileX - 1 : plr.TileX + 2;
                int i = 0;
                int j = plr.TileY + 3;
                Item selectedItem = plr.SelectedItem;

                if (selectedItem.createTile < 0 && selectedItem.createWall < 0)
                {
                    plr.SendErrorMessage("你手持的物品无法放置。");
                    return;
                }

                bool isTile = selectedItem.createTile >= 0;
                int styleId = selectedItem.placeStyle;

                if (j > Main.maxTilesY || j < 0)
                {
                    plr.SendErrorMessage("无法在这里建造桥梁。");
                    return;
                }

                if (isTile)
                {
                    if (!Config.AllowedTileIDs.Contains(selectedItem.createTile))
                    {
                        plr.SendErrorMessage("你手持的物品无法自动建造桥梁。（一般仅允许使用平台或团队块或种植盆。）");
                        return;
                    }

                    int tileCount = 0;
                    while (CheckTileAvailability(startX, startX + i, j, plr))
                    {
                        WorldGen.PlaceTile(startX + i, j, selectedItem.createTile, false, true, -1, styleId);
                        TSPlayer.All.SendTileRect((short)(startX + i), (short)j, 1, 1);
                        plr.SelectedItem.stack--;
                        i += direction;
                        tileCount++;
                    }

                    plr.SendSuccessMessage($"{tileCount}格桥梁建造完成！");
                }
                else
                {
                    while (CheckTileAvailability(startX, startX + i, j, plr))
                    {
                        Main.tile[startX + i, j].wall = (ushort)selectedItem.createWall;
                        TSPlayer.All.SendTileRect((short)(startX + i), (short)j, 1, 1);
                        plr.SelectedItem.stack--;
                        i += direction;
                    }
                }

                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(plr.SelectedItem.Name), plr.Index, plr.TPlayer.selectedItem);
                NetMessage.SendData((int)PacketTypes.PlayerSlot, plr.Index, -1, NetworkText.FromLiteral(plr.SelectedItem.Name), plr.Index, plr.TPlayer.selectedItem);
            });
        }


        private static bool CheckTileAvailability(int startX, int x, int y, TSPlayer plr)
        {
            return x < Main.maxTilesX && x >= 0 &&
                   Math.Abs(startX - x) < Config.MaxPlaceLength &&
                   plr.SelectedItem.stack > 0 &&
                   !TShock.Regions.InArea(x, y) &&
                   !Main.tile[x, y].active();
        }
    }
}

