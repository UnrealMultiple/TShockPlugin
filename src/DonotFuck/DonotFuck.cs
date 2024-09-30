using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;


namespace DonotFuck
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        //作者名称
        public override string Author => "Cai 羽学修改";
        //插件的一句话描述
        public override string Description => "禁止脏话";
        //插件的名称
        public override string Name => "Don't Fuck";
        //插件的版本
        public override Version Version => new Version(2, 0, 1);

        internal static Configuration Config; //将Config初始化


        //插件的构造器
        public Plugin(Main game) : base(game)
        {
            // 实例化配置对象，用于存储和管理插件的配置信息。
            Config = new Configuration();

            // 验证配置对象及其内部的敏感词集合是否已被正确初始化。
            // 如果任一条件未满足（即Config为null或者Config.DirtyWords为null），说明配置未正确完成，
            // 这时抛出InvalidOperationException异常，提示开发者或系统管理员检查配置初始化逻辑。
            if (Config == null || Config.DirtyWords == null)
                throw new InvalidOperationException("\n配置未正确初始化。");
        }


        //插件加载时执行的代码
        public override void Initialize()
        {
            LoadConfig();
            ServerApi.Hooks.ServerChat.Register(this, OnChat); //注册聊天钩子

            // 注册重新加载事件的监听器，当接收到重新加载信号时重新加载配置。
            GeneralHooks.ReloadEvent += LoadConfig;
        }

        // 重新加载配置文件的方法，支持在接收到重新加载事件或直接调用时更新配置。
        private static void LoadConfig(ReloadEventArgs args = null)
        {
            // 使用Read方法加载配置文件，若文件不存在则自动创建。
            Config = Configuration.Read(Configuration.FilePath);

            // 写回配置文件，这一步通常用于确保配置的持久化或格式化，读取方法
            Config.Write(Configuration.FilePath);

            // 如果ReloadEventArgs不为空且包含玩家信息，则向该玩家发送成功重新加载配置的消息。
            if (args != null && args.Player != null)
            {
                args.Player.SendSuccessMessage("[禁止脏话]重新加载配置完毕。");
            }
        }

        // 检查玩家聊天行为
        private void OnChat(ServerChatEventArgs args)
        {
            TSPlayer player = TShock.Players[args.Who];

            if (player == null || args.Who == null || player.HasPermission("Civilized") || player.Group.Name.Equals("owner", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            int WordsCount = 0;

            // 遍历脏话列表，计算本次聊天触发的脏话数量
            foreach (var badWord in Config.DirtyWords)
            {
                if (args.Text.Contains(badWord, StringComparison.OrdinalIgnoreCase))
                {
                    WordsCount++;
                }
            }

            // 如果触发了脏话，提醒玩家并更新累计违规次数
            if (WordsCount > 0)
            {
                string Text = args.Text; // 原始发言内容
                List<string> BadWordList = new List<string>(); // 存储玩家准确的脏话词语

                // 遍历脏话表检查是否有匹配项
                foreach (var badWord in Config.DirtyWords)
                {
                    if (Text.Contains(badWord, StringComparison.OrdinalIgnoreCase))
                    {
                        BadWordList.AddRange(GetExactMatches(Text, badWord)); // 获取并添加精确匹配的脏话
                    }
                }

                // 如果有触发脏话，显示给玩家的信息
                if (BadWordList.Any())
                {
                    string ShowBadWords = "";
                    foreach (string badWord in BadWordList)
                    {
                        ShowBadWords += $"- {badWord}\n";
                    }

                    //通知所有玩家
                    TSPlayer.All.SendInfoMessage($"玩家[c/FFCCFF:{player.Name}]触发了以下敏感词：\n{ShowBadWords.TrimEnd('\n')}");


                    foreach (string badWord in BadWordList)
                    {
                        // 输出准确的脏话词语到控制台
                        TShock.Log.ConsoleInfo($"玩家 [{player.Name}] 发言中的脏话：{badWord}");
                    }
                }

                var Count = Ban.Trigger(player.Name);
                // 只有累计违规次数达到上限才发送提醒信息并执行封禁逻辑
                if (Count > Config.InspectedQuantity)
                {
                    TSPlayer.All.SendSuccessMessage($"玩家[c/FFCCFF:{player.Name}]被检测到多次用词不当！");
                    Console.WriteLine($"玩家[{player.Name}]被检测到多次用词不当！");

                    // 达到违规次数上限后执行封禁逻辑
                    if (Config.Ban)
                    {
                        //Ban类，触发封禁方法
                        Ban.AddBan(player.Name, $"不许说脏话", Config.ProhibitionTime * 60);
                        //通知所有玩家
                        TSPlayer.All.SendInfoMessage($"{player.Name}已被封禁！原因：连续说了脏话");
                        //输出到日志文件
                        TShock.Log.ConsoleInfo($"{player.Name}已被封禁！原因：连续说了脏话");
                        //断开玩家链接，并通知原因
                        player.Disconnect($"你已被封禁！原因：连续说了脏话！");

                        Ban.Remove(player.Name); //Ban类，清零玩家违规次数
                        return;
                    }
                }
            }
        }

        // 定义获取原始文本中精确匹配脏话的辅助函数
        private static IEnumerable<string> GetExactMatches(string text, string badWord)
        {
            int index = 0;
            while ((index = text.IndexOf(badWord, index, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                yield return text.Substring(index, badWord.Length);
                index += badWord.Length;
            }
        }

        //释放钩子
        protected override void Dispose(bool disposing)
        {
            // 当disposing为true，表示是通过代码显式调用Dispose()，此时应执行额外的清理工作。
            if (disposing)
            {
                ServerApi.Hooks.ServerChat.Deregister(this, OnChat); //卸载聊天钩子
            }
            // 调用基类的Dispose方法，以确保基类中可能存在的资源也得到正确释放。
            base.Dispose(disposing);
        }
    }
}