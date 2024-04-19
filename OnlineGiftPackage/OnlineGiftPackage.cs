using Microsoft.Xna.Framework;
using System.Collections.Concurrent;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using 在线礼包;

// 插件命名空间
namespace OnlineGiftPackage
{
    // 插件使用TShockAPI v2.1
    [ApiVersion(2, 1)]
    public class OnlineGiftPackage : TerrariaPlugin
    {
        // 插件作者信息
        public override string Author => "星夜神花 羽学适配";
        // 插件描述
        public override string Description => "在线礼包插件 ";
        // 插件名称
        public override string Name => "在线礼包";
        // 插件版本号
        public override Version Version => new Version(1, 0, 1, 1);
        // 构造函数，初始化插件与游戏关联
        public OnlineGiftPackage(Main game) : base(game)
        {
        }

        // 当前系统日期的天数
        public static int Day = DateTime.Now.Day;

        public long FrameCount;

        // 使用线程安全的字典存储玩家在线时长
        private ConcurrentDictionary<string, int> players = new ConcurrentDictionary<string, int>();

        // 配置文件加载实例
        private static Configuration config = new(); // 将config声明为静态字段

        private object syncRoot = new object(); // 用于锁定发放礼包的临界区

        // 插件初始化方法
        public override void Initialize()
        {
            // 调用LoadConfig方法获取配置实例并赋值给config
            LoadConfig();
            // 注册命令，用于显示礼包获取概率
            Commands.ChatCommands.Add(new Command(GetProbability, "在线礼包"));
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
            // 监听服务器重载事件，以便在重载后重新设置定时器
            GeneralHooks.ReloadEvent += ReloadEvent;
            
        }

        private void OnUpdate(EventArgs args)
        {
            FrameCount++;
            if (FrameCount % (60 * config.DistributionInterval) == 0)
                Timer_Elapsed();
        }

        //加载并创建配置文件
        private void LoadConfig()
        {
            //如果配置文件存在，只读不覆盖
            if (File.Exists(Configuration.FilePath))
            {
                config = Configuration.Read(Configuration.FilePath);
                config.CalculateTotalProbability();
            }
            // 如果配置文件不存在或加载失败，则创建并保存默认配置文件
            else
            {
                config = Configuration.CreateDefaultConfig();
                config.Write(Configuration.FilePath);
            }
        }

        // 重载事件处理程序
        private void ReloadEvent(ReloadEventArgs e)
        {
            LoadConfig();

            // 调用UpdateTotalProbabilityOnReload方法来更新总概率
            config.UpdateTotalProbabilityOnReload();

            Console.WriteLine($"已重载 [在线礼包] 配置文件,下次发放将在{config.DistributionInterval}秒后");
            int totalProbability = config.CalculateTotalProbability();
            Console.WriteLine($"所有礼包的总概率为：{totalProbability}");
        }

        private void Timer_Elapsed()
        {

            // 获取当前日期，并清理在线时长记录（每天只在第一次执行时清空）
            if (DateTime.Now.Day != Day)
            {
                Day = DateTime.Now.Day;
                players.Clear();
            }

            foreach (var player in TShock.Players.Where(p => p != null && p.Active && p.IsLoggedIn && p.TPlayer.statLifeMax < config.SkipStatLifeMax))
            {
                if (!config.Enabled)
                {
                    return;
                }

                // 记录或增加玩家在线时长
                players.AddOrUpdate(player.Name, 1, (_, currentCount) => currentCount + 1);

                // 跳过生命值大于多少的玩家
                if (player.TPlayer.statLifeMax >= config.SkipStatLifeMax)
                {
                    continue;
                }

                // 根据玩家在线时长发放对应礼包
                if (players[player.Name] >= config.TriggerSequence.Keys.Min())
                {
                    Gift gift = RandGift();
                    if (gift == null)
                    {
                        Console.WriteLine($"无法获取有效礼包，玩家 {player.Name} 的在线时长：{players[player.Name]} 秒");
                        continue;
                    }

                    // 获取随机ItemAmount
                    int itemCount = new Random().Next(minValue: gift.ItemAmount[0], gift.ItemAmount[1]);

                    // 给玩家发放物品
                    player.GiveItem(gift.ItamID, itemCount);

                    // 构建礼包发放提示消息
                    string playerMessageFormat = config.TriggerSequence[players[player.Name]];
                    string packageInfoMessage = string.Format(playerMessageFormat + " [i/s{0}:{1}] ", players[player.Name], gift.ItamID, itemCount);

                    // 添加DistributionInterval信息
                    int intervalForDisplay = config.DistributionInterval;
                    string intervalMessage = $"下次发放将在{intervalForDisplay}秒后";

                    // 合并两条消息
                    string combinedMessage = $"{packageInfoMessage} {intervalMessage}";

                    // 将合并后的消息发送给玩家
                    player.SendMessage(combinedMessage, Color.GreenYellow);

                    // 控制台输出
                    if (config.OutputConsole)
                    {
                        Console.WriteLine($"执行在线礼包发放任务，下次发放将在{config.DistributionInterval}秒后");
                        int totalProbability = config.CalculateTotalProbability();
                        Console.WriteLine($"所有礼包的总概率为：{totalProbability}");
                    }

                    // 发放成功后重置玩家在线时长
                    players[player.Name] %= config.DistributionInterval;
                }
            }
        }

        // 显示礼包获取概率的命令处理程序
        private void GetProbability(CommandArgs args)
        {
            if (args.Player.HasPermission("OnlineGiftPackage"))
            {
                Task.Run(() =>
                {
                    StringBuilder sb = new StringBuilder();

                    // 添加标题行
                    sb.AppendLine("在线礼包概率表：\n");

                    // 显示所有礼包的获取概率，按每5个一组分批显示
                    for (int i = 0; i < config.GiftPackList.Count; i++)
                    {
                        Gift gift = config.GiftPackList[i];
                        sb.Append("[i/s1:{0}]:{1:0.##}% ".SFormat(gift.ItamID, 100.0 * ((double)gift.Probability / config.TotalProbability)));

                        // 每显示5个礼包后换行
                        if ((i + 1) % 5 == 0)
                        {
                            sb.AppendLine();
                        }
                    }

                    // 计算并添加总概率信息
                    int totalProbability = config.CalculateTotalProbability();
                    sb.AppendLine($"\n所有礼包的总概率为：{totalProbability}%");

                    // 发送给玩家
                    args.Player.SendMessage(sb.ToString(), Color.Cornsilk);
                });
            }
            else
            {
                args.Player.SendMessage("你没有足够的权限来查看礼包获取概率。", Color.Red); // 或者使用您的错误提示方式
            }
        }

        // 随机选取礼包的方法
        Random rand = new Random();
        public Gift? RandGift()
        {
            int index = rand.Next(config.TotalProbability);
            int sum = 0;

            // 从索引0开始遍历，修正for循环起点
            for (int i = 0; i < config.GiftPackList.Count; i++)
            {
                sum += config.GiftPackList[i].Probability;
                if (index < sum)
                {
                    return config.GiftPackList[i];
                }
            }
            return null;
        }
    }
}