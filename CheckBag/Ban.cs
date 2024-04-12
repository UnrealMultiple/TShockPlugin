using System;
using System.Collections.Generic;
using System.Linq;
using TShockAPI;
using TShockAPI.DB;

namespace CheckBag
{
    public class Ban
    {
        static string BanningUser = "CheckBag";
        static Dictionary<string, int> _bans = new();


        /// <summary>
        /// 触发规则
        /// </summary>
        public static int Trigger(string name)
        {
            if (_bans.ContainsKey(name))
            {
                _bans[name]++;
            }
            else
            {
                _bans.Add(name, 1);
            }
            return _bans[name];
        }


        /// <summary>
        /// 移除记录
        /// </summary>
        public static void Remove(string name)
        {
            if (_bans.ContainsKey(name))
            {
                _bans.Remove(name);
            }
        }


        /// <summary>
        /// 添加封禁
        /// </summary>
        public static AddBanResult AddBan(string playerName, string reason, int durationSeconds)
        {
            DateTime expiration = DateTime.UtcNow.AddSeconds(durationSeconds);
            AddBanResult banResult = TShock.Bans.InsertBan("acc:" + playerName, reason, BanningUser, DateTime.UtcNow, expiration);
            if (banResult.Ban != null)
            {
                TShock.Log.Info($"已封禁{playerName}。输入 /ban del {banResult.Ban.TicketNumber} 解除封禁。");
            }
            else
            {
                TShock.Log.Info($"封禁{playerName}失败！原因: {banResult.Message}。");
            }
            return banResult;
        }


        /// <summary>
        /// 列出封禁
        /// </summary>
        public static void ListBans(CommandArgs args)
        {
            var bans = (
                from ban in TShock.Bans.Bans
                where ban.Value.BanningUser == BanningUser
                orderby ban.Value.ExpirationDateTime descending
                select ban
                ).ToList();
            var lines = new List<string>();
            var flag = false;
            foreach (var ban in bans)
            {
                if (!flag && (ban.Value.ExpirationDateTime <= DateTime.UtcNow))
                {
                    lines.Add("----下面的记录都已失效----");
                    flag = true;
                }
                lines.Add($"{ban.Value.Identifier.Substring(4)}, 截止：{ban.Value.ExpirationDateTime.ToLocalTime():yyyy-dd-HH hh:mm:ss}, 原因：{ban.Value.Reason}, 解封：/ban del {ban.Key}");
            }

            if (!lines.Any())
            {
                args.Player.SendInfoMessage("没有记录！看来没人作弊(*^▽^*)");
                return;
            }

            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out int pageNumber))
            {
                return;
            }
            PaginationTools.SendPage(args.Player, pageNumber, lines, new PaginationTools.Settings
            {
                MaxLinesPerPage = 15, // 每页显示15行
                HeaderFormat = "封禁记录 ({0}/{1})：",
                FooterFormat = "输入/cbag ban {{0}}查看更多".SFormat(Commands.Specifier),
            }) ;
        }

    }
}
