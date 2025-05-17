using LazyAPI;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;

namespace BridgeBuilder;

[ApiVersion(2, 1)]
public class BridgeBuilder : LazyPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override Version Version => new Version(1, 1, 4);
    public override string Author => "Soofa，肝帝熙恩汉化1449";
    public override string Description => GetString("铺桥!");

    public BridgeBuilder(Main game) : base(game)
    {
    }



    public override void Initialize()
    {
        Commands.ChatCommands.Add(new("bridgebuilder.bridge", this.BridgeCmd, "bridge", "桥来")
        {
            AllowServer = false,
            HelpText = GetString("朝着你看的方向建造桥梁。（你需要持有一定数量的平台或团队块或种植盆。）")
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.BridgeCmd);
        }

        // Call the base class dispose method.
        base.Dispose(disposing);
    }

    private async void BridgeCmd(CommandArgs args)
    {
        await Task.Run(() =>
        {
            var plr = args.Player;
            var directionX = plr.TPlayer.direction;
            var directionY = 0;

            if (args.Parameters.Count > 0)
            {
                var direction = args.Parameters[0].ToLower();
                if (direction == "up" || direction == "down")
                {
                    directionY = direction == "up" ? -1 : 1; // 确保正确的垂直方向
                    directionX = 0; // 在垂直建造时，水平方向不移动
                }
            }

            var startX = plr.TileX + (directionX == -1 ? -2 : 1);
            var i = 0;
            var j = plr.TileY + (directionY == -1 ? -1 : (directionY == 1 ? 3 : 3));

            var selectedItem = plr.SelectedItem;

            if (selectedItem.createTile < 0 && selectedItem.createWall < 0)
            {
                plr.SendErrorMessage(GetString("你手持的物品无法放置。"));
                return;
            }

            var isTile = selectedItem.createTile >= 0;
            var styleId = selectedItem.placeStyle;

            if (j < 0 || j >= Main.maxTilesY)
            {
                plr.SendErrorMessage(GetString("无法在这里建造桥梁。"));
                return;
            }

            var placedCount = 0;

            while (CheckTileAvailability(startX + (i * directionX), j, plr) && selectedItem.stack > 0)
            {
                if (isTile)
                {
                    WorldGen.PlaceTile(startX + (i * directionX), j, selectedItem.createTile, false, true, -1, styleId);
                }
                else
                {
                    Main.tile[startX + (i * directionX), j].wall = (ushort) selectedItem.createWall;
                }

                TSPlayer.All.SendTileRect((short) (startX + (i * directionX)), (short) j, 1, 1);
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
                plr.SendSuccessMessage(GetString($"{placedCount}格桥梁建造完成！"));
            }
            else
            {
                plr.SendErrorMessage(GetString("没有足够的空间或物品来建造桥梁。"));
            }

            NetMessage.SendData((int) PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(plr.SelectedItem.Name), plr.Index, plr.TPlayer.selectedItem);
            NetMessage.SendData((int) PacketTypes.PlayerSlot, plr.Index, -1, NetworkText.FromLiteral(plr.SelectedItem.Name), plr.Index, plr.TPlayer.selectedItem);
        });
    }

    private static bool CheckTileAvailability(int x, int y, TSPlayer plr, int directionX = 0, int directionY = 0)
    {
        var canPlace = x < Main.maxTilesX && x >= 0 && y < Main.maxTilesY && y >= 0;
        if (!canPlace)
        {
            return false;
        }

        var horizontalDistance = Math.Abs(plr.TileX - x); // 计算水平方向上的距离
        var verticalDistance = Math.Abs(plr.TileY - y); // 计算竖直方向上的距离
        canPlace &= horizontalDistance < Configuration.Instance.MaxPlaceLength && verticalDistance < Configuration.Instance.MaxPlaceLength;
        canPlace &= horizontalDistance < Configuration.Instance.MaxPlaceLength && verticalDistance < Configuration.Instance.MaxPlaceLength;
        canPlace &= plr.SelectedItem.stack > 0;
        canPlace &= !TShock.Regions.InArea(x, y);

        // 根据物品类型进一步检查
        if (plr.SelectedItem.createTile < 0 && plr.SelectedItem.createWall >= 0)
        {
            canPlace &= Main.tile[x, y].wall == 0;
        }
        else if (plr.SelectedItem.createTile >= 0)
        {
            canPlace &= !Main.tile[x, y].active();
        }

        return canPlace;
    }


}