using System.Text;
using TShockAPI;

namespace DonotFuck;

internal class Commands
{
    #region 主指令方法
    public static void DFCmd(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            Help(args);
        }

        if (args.Parameters.Count == 1)
        {
            if (args.Parameters[0].ToLower() == "list")
            {
                ListDirtyWords(args.Player, 1);
                return;
            }

            if (args.Parameters[0].ToLower() == "log" && args.Player.HasPermission("DonotFuck.admin"))
            {
                Configuration.Instance.EnableLog = !Configuration.Instance.EnableLog;
                args.Player.SendSuccessMessage(Configuration.Instance.EnableLog
                    ? GetString($"已启用敏感词记录功能。")
                    : GetString($"已禁用敏感词记录功能。"));
                Configuration.Save();
                return;
            }

            if (args.Parameters[0].ToLower() == "clear" && args.Player.HasPermission("DonotFuck.admin"))
            {
                Configuration.Instance.DisposeLog();
                var dirinfo = new DirectoryInfo(Path.Combine(TShock.SavePath, Configuration._Directory));
                foreach (var file in dirinfo.GetFiles("*.log"))
                {
                    file.Delete();
                }
                args.Player.SendSuccessMessage(GetString("《脏话纪录》文件夹已成功清空。"));
                return;
            }
        }

        if (args.Parameters.Count == 2)
        {
            var action = args.Parameters[0].ToLower();
            var word = args.Parameters[1];

            switch (action)
            {
                case "add":
                    if (args.Player.HasPermission("DonotFuck.admin"))
                    {
                        if (Configuration.Instance.Add(word))
                        {
                            args.Player.SendSuccessMessage(GetString("已成功将敏感词添加到表中: [c/92C5EC:{0}]!", word));
                        }
                        else
                        {
                            args.Player.SendErrorMessage(GetString("[c/92C5EC:{0}] 已在敏感词表中!", word));
                        }
                    }
                    break;

                case "del":
                    if (args.Player.HasPermission("DonotFuck.admin"))
                    {
                        if (Configuration.Instance.Del(word))
                        {
                            args.Player.SendSuccessMessage(GetString("已成功将敏感词从表中移除: [c/92C5EC:{0}]!", word));
                        }
                        else
                        {
                            args.Player.SendErrorMessage(GetString("[c/92C5EC:{0}] 不在敏感词表中!", word));
                        }
                    }
                    break;

                case "l":
                case "list":

                    if (int.TryParse(args.Parameters[1], out var page))
                    {
                        ListDirtyWords(args.Player, page);
                    }
                    else
                    {
                        args.Player.SendErrorMessage(GetString("无效的页码。请输入一个数字。"));
                    }
                    break;

                default:
                    Help(args);
                    break;
            }
        }
    }
    #endregion

    #region 菜单方法
    private static void Help(CommandArgs args)
    {
        if (args.Player.HasPermission("DonotFuck.admin"))
        {
            var mess = new StringBuilder();
            mess.AppendFormat(GetString("《禁止脏话》 [i:3456][C/F2F2C7:插件开发] [C/BFDFEA:by] [c/00FFFF:羽学][i:3459]\n"));
            mess.AppendFormat(GetString("/df —— 查看菜单\n"));
            mess.AppendFormat(GetString("/df list 页数 —— [c/B3B5EA:列出]所有[c/B3EADD:敏感词]\n"));
            mess.AppendFormat(GetString("/df log —— [c/8AE072:开启]|[c/ED7985:关闭]敏感词[c/EBB4E2:记录]\n"));
            mess.AppendFormat(GetString("/df clear —— [c/B3EADB:清理]所有[c/F2F191:脏话纪录]\n"));
            mess.AppendFormat(GetString("/df add 或 del 词语 —— [c/87DF86:添加]|[c/F19092:删除]敏感词\n"));
            mess.AppendFormat(GetString($"敏感词纪录：[c/B3B6EA:{Configuration.Instance.EnableLog}]"));
            args.Player.SendMessage(mess.ToString(), 245, 241, 188);
        }
        else
        {
            var mess2 = new StringBuilder();
            mess2.AppendFormat(GetString("《禁止脏话》 [i:3456][C/F2F2C7:插件开发] [C/BFDFEA:by] [c/00FFFF:羽学][i:3459]\n"));
            mess2.AppendFormat(GetString("/df —— 查看菜单\n"));
            mess2.AppendFormat(GetString("/df list 页数 —— [c/B3B5EA:列出]所有[c/B3EADD:敏感词]"));
            args.Player.SendMessage(mess2.ToString(), 245, 241, 188);
        }
    }
    #endregion

    #region 辅助方法 用Linq查询并排列 列出敏感词
    private static void ListDirtyWords(TSPlayer plr, int page)
    {
        var Size = Configuration.Instance.PageSize;
        var dirty = Configuration.Instance.DirtyWords.OrderBy(word => word).ToList();

        var count = dirty.Count;
        var pages = (int) Math.Ceiling(count / (double) Size);

        if (page < 1 || page > pages)
        {
            plr.SendErrorMessage(GetString("无效的页码。总共有 {0} 页。", pages));
            return;
        }

        var start = (page - 1) * Size;
        var end = Math.Min(start + Size, count);

        var words = dirty.Skip(start).Take(Size).Select((word, index) => $"{index + 1 + start}. {word}");

        plr.SendInfoMessage(GetString($"《敏感词》第 {page} 页，共 {pages} 页:\n{string.Join("\n", words)}"));
    }
    #endregion
}
