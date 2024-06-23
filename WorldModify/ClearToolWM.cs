using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;
using WorldModify;

namespace WorldModify
{
    internal class ClearToolWM
    {
        private enum Type
        {
            Tomb,
            Dress
        }

        public static void Manage(CommandArgs args)
        {
            args.Parameters.RemoveAt(0);
            TSPlayer op = args.Player;
            if (args.Parameters.Count == 0)
            {
                Help();
                return;
            }
            switch (args.Parameters[0].ToLowerInvariant())
            {
                case "help":
                    Help();
                    break;
                case "tombstone":
                case "tomb":
                case "墓碑":
                    if (!TileHelper.NeedWaitTask(op))
                    {
                        Action(op, Type.Tomb);
                    }
                    break;
                case "dressers":
                case "dress":
                    if (!TileHelper.NeedWaitTask(op))
                    {
                        Action(op, Type.Dress);
                    }
                    break;
                default:
                    op.SendInfoMessage("语法错误，输入 /wm clear help 查看帮助");
                    break;
            }
            void Help()
            {
                op.SendInfoMessage("/wm clear 指令用法：");
                op.SendInfoMessage("/wm clear tombstone，全图清理 墓碑");
                op.SendInfoMessage("/wm clear dressers，全图清理 梳妆台（全类型）");
            }
        }

        public static void ClearTomb(CommandArgs args)
        {
            TSPlayer player = args.Player;
            if (!TileHelper.NeedWaitTask(player))
            {
                Action(player, Type.Tomb);
            }
        }

        private static async void Action(TSPlayer op, Type type)
        {
            FindTool.ResetSkip();
            int secondLast = Utils.GetUnixTimestamp;
            string opString = GetOpString();
            Rectangle rect = Utils.GetWorldArea();
            bool flag = false;
            int count = 0;
            await Task.Run(delegate
            {
                op.SendSuccessMessage("全图 清除" + opString + " 开始");
                for (int i = rect.X; i < rect.Right; i++)
                {
                    for (int j = rect.Y; j < rect.Bottom; j++)
                    {
                        switch (type)
                        {
                            case Type.Tomb:
                                flag = ClearTomb(i, j);
                                break;
                            case Type.Dress:
                                flag = ClearDress(i, j);
                                break;
                            default:
                                flag = false;
                                break;
                        }
                        if (flag)
                        {
                            count++;
                        }
                    }
                }
            }).ContinueWith(delegate
            {
                TileHelper.FinishGen();
                op.SendSuccessMessage($"全图共清理了{count}个{opString}（用时 {Utils.GetUnixTimestamp - secondLast}秒）");
            });
            string GetOpString()
            {
                return type switch
                {
                    Type.Tomb => "墓碑",
                    Type.Dress => "梳妆台",
                    _ => "",
                };
            }
        }

        private static bool ClearTomb(int x, int y)
        {
            int num = 85;
            ITile val = Main.tile[x, y];
            if (!val.active() || val.type != num)
            {
                return false;
            }
            FindInfo findInfo = new FindInfo(num, -1, 2, 2);
            if (FindTool.GetItem(x, y, findInfo))
            {
                TileHelper.ClearTile(x, y, findInfo.w, findInfo.h);
                return true;
            }
            return false;
        }

        private static bool ClearDress(int x, int y)
        {
            int num = 88;
            ITile val = Main.tile[x, y];
            if (!val.active() || val.type != num)
            {
                return false;
            }
            FindInfo findInfo = new FindInfo(num, -1, 3, 2);
            if (FindTool.GetItem(x, y, findInfo))
            {
                TileHelper.ClearTile(x, y, findInfo.w, findInfo.h);
                return true;
            }
            return false;
        }
    }
}
