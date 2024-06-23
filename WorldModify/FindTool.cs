using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TShockAPI;

namespace WorldModify
{

    internal class FindTool
    {
        private static readonly Dictionary<string, FindInfo> FindList = new Dictionary<string, FindInfo>
    {
        {
            "附魔剑",
            new FindInfo(187, 5, 3, 2, 918)
        },
        {
            "花苞",
            new FindInfo(238, -1, 2, 2)
        },
        {
            "暗影珠",
            new FindInfo(31, 0, 2, 2, 0)
        },
        {
            "猩红之心",
            new FindInfo(31, 1, 2, 2, 36)
        },
        {
            "生命水晶",
            new FindInfo(12, -1, 2, 2)
        },
        {
            "生命果",
            new FindInfo(236, -1, 2, 2)
        },
        {
            "幼虫",
            new FindInfo(231, -1, 3, 3)
        },
        {
            "丛林蜥蜴祭坛",
            new FindInfo(237, -1, 3, 2)
        },
        {
            "地狱熔炉",
            new FindInfo(77, -1, 3, 2)
        },
        {
            "提炼机",
            new FindInfo(219, -1, 3, 3)
        },
        {
            "织布机",
            new FindInfo(86, -1, 3, 2)
        },
        {
            "恶魔祭坛",
            new FindInfo(26, 0, 3, 2)
        },
        {
            "猩红祭坛",
            new FindInfo(26, 1, 3, 2)
        },
        {
            "墓碑",
            new FindInfo(85, -1, 2, 2)
        },
        {
            "梳妆台",
            new FindInfo(88, -1, 3, 2)
        },
        {
            "最脏的块",
            new FindInfo(668, -1)
        }
    };

        private static List<Rectangle> skip = new List<Rectangle>();

        public static void Manage(CommandArgs args)
        {
            args.Parameters.RemoveAt(0);
            TSPlayer player = args.Player;
            if (args.Parameters.Count == 0)
            {
                player.SendErrorMessage("请输入要查找的名字，例如：附魔剑，可供查找的有：\n" + string.Join(", ", FindList.Keys));
                return;
            }
            ResetSkip();
            string text = args.Parameters[0].ToLowerInvariant();
            int result;
            if (FindList.ContainsKey(text))
            {
                ListedExtra(player, text, FindList[text]);
            }
            else if (int.TryParse(text, out result))
            {
                FindInfo fd = new FindInfo(result);
                ListedExtra(player, text, fd);
            }
            else
            {
                player.SendErrorMessage("要查找的名字不对 或 不支持，可供查找的有：\n" + string.Join(", ", FindList.Keys));
            }
        }

        private static void ListedExtra(TSPlayer op, string opName, FindInfo fd)
        {
            ResetSkip();
            List<Point16> list = new List<Point16>();
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    ITile val = Main.tile[i, j];
                    if (val.active() && val.type == fd.id && GetItem(i, j, fd))
                    {
                        list.Add(new Point16(i, j));
                    }
                }
            }
            if (list.Count == 0)
            {
                op.SendInfoMessage("未找到 " + opName + "！");
                return;
            }
            Point16 val2 = list[0];
            op.SendInfoMessage($"{opName} 查找结果（{list.Count}）:\n位于：{Utils.GetLocationDesc(val2.X, val2.Y)}（/tppos {val2.X} {val2.Y}）\n所有坐标：{string.Join(", ", list.GetRange(0, Math.Min(20, list.Count)))}");
        }

        public static bool GetItem(int tileX, int tileY, FindInfo fd)
        {
            ITile val = Main.tile[tileX, tileY];
            int frameX = val.frameX;
            int frameY = val.frameY;
            bool flag = fd.frameX == -1 || ((frameX == fd.frameX) ? true : false);
            bool flag2 = fd.frameY == -1 || ((frameY == fd.frameY) ? true : false);
            if (flag && flag2 && check(fd.w, fd.h))
            {
                return true;
            }
            return false;
            bool check(int w, int h)
            {
                bool flag3 = true;
                for (int i = tileX; i < tileX + w; i++)
                {
                    for (int j = tileY; j < tileY + h; j++)
                    {
                        if (ContainsSkip(i, j) || !Main.tile[i, j].active() || Main.tile[i, j].type != fd.id)
                        {
                            flag3 = false;
                            break;
                        }
                    }
                }
                if (flag3)
                {
                    skip.Add(new Rectangle(tileX, tileY, w, h));
                }
                return flag3;
            }
        }

        public static void ResetSkip()
        {
            skip.Clear();
        }

        private static bool ContainsSkip(int x, int y)
        {
            foreach (Rectangle item in skip)
            {
                Rectangle current = item;
                if (((Rectangle)(current)).Contains(x, y))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
