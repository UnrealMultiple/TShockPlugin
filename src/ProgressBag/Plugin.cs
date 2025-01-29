using LazyAPI;
using LazyAPI.Extensions;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ProgressBag;

[ApiVersion(2, 1)]
public class Plugin : LazyPlugin
{
    public override string Author => "少司命";

    public override string Description => GetString("进度礼包");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 1, 5);

    public Plugin(Main game) : base(game)
    {
        this.Order = 3;
    }
    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("bag.use", this.GiftBag, "礼包", "bag"));
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(f => f.CommandDelegate == this.GiftBag);
        }
        base.Dispose(disposing);
    }
    public bool ReceiveBag(TSPlayer Player, Bag bag, bool msg = true)
    {
        if (!Player.InProgress(bag.Limit))
        {
            if (msg)
            {
                Player.SendErrorMessage(GetString("当前进度无法领取该礼包!"));
            }

            return false;
        }
        if (bag.Group.Count > 0 && !bag.Group.Contains(Player.Group.Name))
        {
            if (msg)
            {
                Player.SendErrorMessage(GetString("你当前所在的组无法领取该礼包!"));
            }

            return false;
        }
        if (!bag.Receive.Contains(Player.Name))
        {
            foreach (var award in bag.Award)
            {
                Player.GiveItem(award.netID, award.stack, award.prefix);
            }
            foreach (var cmd in bag.Command)
            {
                Player.HandleCommand(cmd);
            }
            TShock.Log.Write(GetString($"[进度礼包]: {Player.Name} 领取了 {bag.Name}"), System.Diagnostics.TraceLevel.Info);
            if (msg)
            {
                Player.SendSuccessMessage(GetString("领取成功 [{0}] 礼包"), bag.Name);
            }

            bag.Receive.Add(Player.Name);
            Config.Save();
            return true;
        }
        else
        {
            if (msg)
            {
                Player.SendErrorMessage(GetString("[{0}] 礼包已经领取过了，不能重复领取"), bag.Name);
            }

            return false;
        }
    }

    private void GiftBag(CommandArgs args)
    {
        void ShowBag(List<string> line)
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out var pageNumber))
            {
                return;
            }

            PaginationTools.SendPage(
                    args.Player,
                    pageNumber,
                    line,
                    new PaginationTools.Settings
                    {
                        MaxLinesPerPage = 6,
                        HeaderFormat = GetString("礼包列表 ({0}/{1})："),
                        FooterFormat = GetString("输入 {0}礼包 list {{0}} 查看更多").SFormat(Commands.Specifier)
                    }
                );
        }
        if (args.Parameters.Count > 0 && args.Parameters[0].ToLower() == "help")
        {
            args.Player.SendInfoMessage(GetString("/bag list"));
            args.Player.SendInfoMessage(GetString("/bag rall"));
            args.Player.SendInfoMessage(GetString("/bag r <礼包名称>"));
            args.Player.SendInfoMessage(GetString("/bag reload"));
            args.Player.SendInfoMessage(GetString("/bag reset"));
        }
        else if (args.Parameters.Count >= 1 && args.Parameters[0] == "list")
        {
            var lines = Config.Instance.Bag.Select(b => b.Receive.Contains(args.Player.Name) ?
                GetString("[{0}] => {1}", b.Name, GetString("已领取").Color(TShockAPI.Utils.GreenHighlight)) :
                GetString("[{0}] => {1}", b.Name, GetString("未领取").Color(TShockAPI.Utils.BoldHighlight))
            ).ToList();
            ShowBag(lines);
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0] == "reset")
        {
            if (!args.Player.HasPermission("bag.admin"))
            {
                args.Player.SendErrorMessage(GetString("没有足够权限执行此命令"));
                return;
            }
            Config.Instance.Reset();
            args.Player.SendSuccessMessage(GetString("礼包领取重置成功!"));
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0] == "rall")
        {
            var i = 0;
            foreach (var bag in Config.Instance.Bag)
            {
                if (this.ReceiveBag(args.Player, bag, false))
                {
                    i++;
                }
            }
            args.Player.SendSuccessMessage(i > 0 ? GetString($"成功领取{i}个进度礼包!") : GetString("没有进度礼包可以领取!"));
        }
        else if (args.Parameters.Count == 2 && args.Parameters[0] == "r")
        {
            foreach (var bag in Config.Instance.Bag)
            {
                if (bag.Name == args.Parameters[1])
                {
                    this.ReceiveBag(args.Player, bag);
                    return;
                }
            }
            args.Player.SendErrorMessage(GetString("没有此礼包"));
        }
        else
        {
            args.Player.SendInfoMessage(GetString("输入/bag help"));
        }

    }
}