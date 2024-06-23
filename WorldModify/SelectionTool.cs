using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace WorldModify
{
    internal class SelectionTool
    {
        private static TempPointData[] TempPoints = new TempPointData[255];

        private static bool hasEvent = false;

        public static void Mange(CommandArgs args)
        {
            args.Parameters.RemoveAt(0);
            TSPlayer player = args.Player;
            Rectangle selection = GetSelection(player.Index);
            if (args.Parameters.Count == 0)
            {
                if (selection.Width == 0 && selection.Height == 0)
                {
                    player.SendInfoMessage("选区：以玩家为中心的一屏区域，区域将实时计算");
                    return;
                }
                if (selection.Width == Main.maxTilesX && selection.Height == Main.maxTilesY)
                {
                    player.SendErrorMessage("选区：整个世界！整个世界！整个世界！");
                    return;
                }
                player.SendInfoMessage($"选区：x={selection.X} y={selection.Y} 宽={selection.Width} 高={selection.Height}");
                return;
            }
            TempPointData pointData = GetPointData(player.Index);
            switch (args.Parameters[0].ToLowerInvariant())
            {
                case "help":
                    player.SendInfoMessage("/igen s, 查看 选区");
                    player.SendInfoMessage("/igen s all，将 选区 设置为 整个世界");
                    player.SendInfoMessage("/igen s screen，将 选区 设置为 以玩家为中心的一屏区域");
                    player.SendInfoMessage("/igen s 1，设置 选区的 起始点");
                    player.SendInfoMessage("/igen s 2，设置 选区的 结束点");
                    break;
                case "all":
                    pointData.rect = new Rectangle(0, 0, Main.maxTilesX, Main.maxTilesY);
                    player.SendSuccessMessage("已将选区设置为整个世界");
                    break;
                case "screen":
                case "0":
                    pointData.rect = default(Rectangle);
                    player.SendSuccessMessage("已将选区设置为以玩家为中心的一屏区域，区域将实时计算。");
                    break;
                case "1":
                    pointData.AwaitingTempPoint = 1;
                    RegisterEvent();
                    player.SendSuccessMessage("用镐子敲击方块，以设置选区的起始点");
                    break;
                case "2":
                    if (pointData.TempPoints[0] == Point.Zero)
                    {
                        player.SendInfoMessage("请先执行[c/96FF0A:/igen s 1]设置选区的起始点");
                        break;
                    }
                    pointData.AwaitingTempPoint = 2;
                    player.SendSuccessMessage("用镐子敲击方块，以完成选区的设置。");
                    break;
            }
        }

        public static Rectangle GetSelection(int index)
        {
            if (index == -1)
            {
                return Utils.GetBaseArea();
            }
            if (TempPoints[index] == null || (TempPoints[index].rect.Width == 0 && TempPoints[index].rect.Height == 0))
            {
                return Utils.GetScreen(TShock.Players[index]);
            }
            return Utils.CloneRect(TempPoints[index].rect);
        }

        private static TempPointData GetPointData(int index)
        {
            TempPointData tempPointData = TempPoints[index];
            if (tempPointData == null)
            {
                tempPointData = new TempPointData();
                TempPoints[index] = tempPointData;
            }
            return tempPointData;
        }

        private static void RegisterEvent()
        {
            if (!hasEvent)
            {
                hasEvent = true;
                GetDataHandlers.TileEdit += new EventHandler<GetDataHandlers.TileEditEventArgs>(OnTileEdit);
            }
        }

        public static void OnTileEdit(object sender, GetDataHandlers.TileEditEventArgs e)
        {
            TSPlayer player = e.Player;
            TempPointData pointData = GetPointData(player.Index);
            if (pointData.AwaitingTempPoint != 0)
            {
                // 直接更新坐标
                pointData.TempPoints[pointData.AwaitingTempPoint - 1] = new Point(e.X, e.Y);

                if (pointData.AwaitingTempPoint == 1)
                {
                    player.SendInfoMessage("已设置起始点，输入[c/96FF0A:/igen s 2]以设置结束点");
                }
                else
                {
                    // 计算矩形的左上角和右下角坐标
                    int minX = Math.Min(pointData.TempPoints[0].X, pointData.TempPoints[1].X);
                    int minY = Math.Min(pointData.TempPoints[0].Y, pointData.TempPoints[1].Y);
                    int width = Math.Abs(pointData.TempPoints[0].X - pointData.TempPoints[1].X) + 1;
                    int height = Math.Abs(pointData.TempPoints[0].Y - pointData.TempPoints[1].Y) + 1;

                    // 创建并设置矩形
                    pointData.rect = new Rectangle(minX, minY, width, height);

                    // 通知玩家并重置
                    player.SendSuccessMessage($"已将选区设置为 x={minX} y={minY} 宽={width} 高={height}（仅本次开服有效）");
                    pointData.TempPoints[0] = Point.Zero;
                    pointData.TempPoints[1] = Point.Zero;
                    pointData.AwaitingTempPoint = 0;
                }

                player.SendTileSquareCentered(e.X, e.Y, 4);
                e.Handled = true;
            }
        }

        public static void dispose()
        {
            if (hasEvent)
            {
                hasEvent = false;
                GetDataHandlers.TileEdit -= new EventHandler<GetDataHandlers.TileEditEventArgs>(OnTileEdit);
            }
            TempPoints = null;
        }
    }
}
