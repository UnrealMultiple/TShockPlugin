using TShockAPI; // 引入TShockAPI命名空间，用于与TShock插件交互。
using TShockAPI.DB; // 引入TShock数据库操作相关的命名空间。

// 命名空间 DonotFuck，用于存放与禁止不当行为相关的功能类。
namespace DonotFuck
{
    // 定义 Ban 类，用于管理玩家的不当行为检测、封禁及查询。
    public class Ban
    {
        // 静态变量，封禁操作的执行者名称。
        static string BanningUser = "DonotFuck";

        // 静态字段，使用字典存储玩家触发不当行为的计数。
        static Dictionary<string, int> _bans = new Dictionary<string, int>();

        // 触发规则方法，记录玩家的不当行为次数。
        public static int Trigger(string name)
        {
            lock (_bans) // 使用锁机制保护字典的并发访问，防止多线程环境下的数据竞争问题。
            {
                if (_bans.ContainsKey(name)) // 检查玩家名称是否已经在记录中。
                {
                    _bans[name]++; // 如果存在，则增加该玩家的触发次数。
                }
                else
                {
                    _bans.Add(name, 1); // 如果不存在，则添加该玩家到字典中，并初始化触发次数为1。
                }
                return _bans[name]; // 返回该玩家的当前触发次数。
            }
        }

        // 重置玩家的触发次数方法。
        public static void Remove(string name)
        {
            lock (_bans) // 使用锁保证线程安全。
            {
                if (_bans.ContainsKey(name)) // 检查字典中是否存在该玩家的记录。
                {
                    _bans[name] = 0; // 将玩家的触发次数重置为0。
                }
            }
        }

        // 添加封禁方法，根据玩家名、原因和持续时间进行封禁。
        public static AddBanResult AddBan(string playerName, string reason, int durationSeconds)
        {
            DateTime expiration = DateTime.UtcNow.AddSeconds(durationSeconds); // 计算封禁到期时间。
            AddBanResult banResult = TShock.Bans.InsertBan("acc:" + playerName, reason, BanningUser, DateTime.UtcNow, expiration); // 执行封禁操作。
            if (banResult.Ban != null)
            {
                TShock.Log.Info($"已封禁{playerName}。输入 /ban del {banResult.Ban.TicketNumber} 解除封禁。"); // 日志记录成功封禁。
            }
            else
            {
                TShock.Log.Info($"封禁{playerName}失败！原因: {banResult.Message}。"); // 日志记录封禁失败原因。
            }
            return banResult; // 返回封禁操作的结果。
        }

        // 列出封禁方法，展示由本插件执行的所有封禁记录。
        public static void ListBans(CommandArgs args)
        {
            var bans = (
                from ban in TShock.Bans.Bans // 查询所有封禁记录。
                where ban.Value.BanningUser == BanningUser // 筛选出由本插件执行的封禁。
                orderby ban.Value.ExpirationDateTime descending // 按封禁到期时间降序排列。
                select ban
            ).ToList();

            var lines = new List<string>(); // 用于存储格式化后的封禁记录信息。
            bool flag = false; // 标记是否遇到已过期的封禁记录。
            foreach (var ban in bans)
            {
                if (!flag && (ban.Value.ExpirationDateTime <= DateTime.UtcNow))
                {
                    lines.Add("----下面的记录都已失效----"); // 分隔符，表明以下封禁已过期。
                    flag = true;
                }
                // 添加单条封禁记录信息。
                lines.Add($"{ban.Value.Identifier.Substring(4)}, 截止：{ban.Value.ExpirationDateTime.ToLocalTime():yyyy-dd-HH hh:mm:ss}, 原因：{ban.Value.Reason}, 解封：/ban del {ban.Key}"); 
            }

            if (!lines.Any()) // 如果没有任何封禁记录。
            {
                args.Player.SendInfoMessage("没有记录！看来没人作弊(*^▽^*)"); // 向命令发起者发送消息。
                return;
            }

            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out int pageNumber)) // 处理分页参数。
            {
                return;
            }
            PaginationTools.SendPage(args.Player, pageNumber, lines, new PaginationTools.Settings // 分页显示封禁记录。
            {
                MaxLinesPerPage = 15, // 每页最多显示15条记录。
                HeaderFormat = "封禁记录 ({0}/{1})：", // 分页标题格式。
                FooterFormat = "输入/cbag ban {{0}}查看更多".SFormat(Commands.Specifier), // 分页底部提示信息。
            });
        }
    }
}