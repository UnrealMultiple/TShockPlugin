using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ProgressBag
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public override string Author => "少司命";

        public override string Description => "进度礼包";

        public override string Name => "进度礼包";

        public override Version Version => new(1, 0, 0, 7);

        public static Config config = new();

        public static string PATH = Path.Combine(TShock.SavePath, "进度礼包.json");

        public Plugin(Main game) : base(game)
        {
            Order = 3;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Commands.ChatCommands.RemoveAll(f => f.Name == "礼包");
            }
            base.Dispose(disposing);
        }
        public override void Initialize()
        {
            LoadConfig();
            TShockAPI.Hooks.GeneralHooks.ReloadEvent += e => LoadConfig();
            Commands.ChatCommands.Add(new Command("bag.use", GiftBag, "礼包"));
        }

        public bool ReceiveBag(TSPlayer Player, Bag bag, bool msg = true)
        {
            if (!Player.InProgress(bag.Limit))
            {
                if (msg)
                    Player.SendErrorMessage("当前进度无法领取该礼包!");
                return false;
            }
            if (bag.Group.Count > 0 && !bag.Group.Contains(Player.Group.Name))
            {
                if (msg)
                    Player.SendErrorMessage("你当前所在的组无法领取该礼包!");
                return false;
            }
            if (!bag.Receive.Contains(Player.Name))
            {
                foreach (Award award in bag.Award)
                {
                    Player.GiveItem(award.netID, award.stack, award.prefix);
                }
                foreach (string cmd in bag.Command)
                {
                    Player.HandleCommand(cmd);
                }
                TShock.Log.Write($"[进度礼包]: {Player.Name} 领取了 {bag.Name}", System.Diagnostics.TraceLevel.Info);
                if (msg)
                    Player.SendSuccessMessage("领取成功 [{0}] 礼包", bag.Name);
                bag.Receive.Add(Player.Name);
                config.Write(PATH);
                return true;
            }
            else
            {
                if (msg)
                    Player.SendErrorMessage("[{0}] 礼包已经领取过了，不能重复领取", bag.Name);
                return false;
            }
        }

        private void GiftBag(CommandArgs args)
        {
            void ShowBag(List<string> line)
            {
                if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out int pageNumber))
                    return;

                PaginationTools.SendPage(
                        args.Player,
                        pageNumber,
                        line,
                        new PaginationTools.Settings
                        {
                            MaxLinesPerPage = 6,
                            HeaderFormat = "礼包列表 ({0}/{1})：",
                            FooterFormat = "输入 {0}礼包 list {{0}} 查看更多".SFormat(Commands.Specifier)
                        }
                    );
            }
            if (args.Parameters.Count > 0 && args.Parameters[0].ToLower() == "help")
            {
                args.Player.SendInfoMessage("/礼包 list");
                args.Player.SendInfoMessage("/礼包 领取全部");
                args.Player.SendInfoMessage("/礼包 领取 <礼包名称>");
                args.Player.SendInfoMessage("/礼包 reload");
                args.Player.SendInfoMessage("/礼包 重置");
            }
            else if (args.Parameters.Count >= 1 && args.Parameters[0] == "list")
            {
                var lines = config.Bag.Select(b => b.Receive.Contains(args.Player.Name) ?
                    $"[{b.Name}] => {"已领取".Color(TShockAPI.Utils.GreenHighlight)}" :
                    $"[{b.Name}] => {"未领取".Color(TShockAPI.Utils.BoldHighlight)}"
                ).ToList();
                ShowBag(lines);
            }
            else if (args.Parameters.Count == 1 && args.Parameters[0] == "重置")
            {
                if (!args.Player.HasPermission("bag.admin"))
                {
                    args.Player.SendErrorMessage("没有足够权限执行此命令");
                    return;
                }
                config.Reset();
                args.Player.SendSuccessMessage("礼包领取重置成功!");
            }
            else if (args.Parameters.Count == 1 && args.Parameters[0] == "领取全部")
            {
                int i = 0;
                foreach (Bag bag in config.Bag)
                {
                    if (ReceiveBag(args.Player, bag, false))
                        i++;
                }
                args.Player.SendSuccessMessage(i > 0 ? $"成功领取{i}个进度礼包!" : "没有进度礼包可以领取!");
            }
            else if (args.Parameters.Count == 2 && args.Parameters[0] == "领取")
            {
                foreach (Bag bag in config.Bag)
                {
                    if (bag.Name == args.Parameters[1])
                    {
                        ReceiveBag(args.Player, bag);
                        return;
                    }
                }
                args.Player.SendErrorMessage("没有此礼包");
            }
            else
            {
                args.Player.SendInfoMessage("输入/礼包 help");
            }

        }

        private void LoadConfig()//读取/重读配置文件
        {
            try
            {
                if (File.Exists(PATH))
                {
                    config = Config.Read(PATH);
                }
                else
                {
                    foreach (var n in ProgressUtil.Names)
                    {
                        Bag bag = new();
                        bag.Limit.Add(n);
                        bag.Name = n + "礼包";
                        bag.Award.Add(new Award());
                        config.Bag.Add(bag);
                    }
                    TShock.Log.ConsoleError("未找到进度礼包配置文件，已为您创建！");
                }
                config.Write(PATH);
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError("进度礼包配置读取错误:" + ex.ToString());
            }
        }
    }
}
