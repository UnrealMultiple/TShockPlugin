using Economics.Core.ConfigFiles;
using System.Reflection;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Economics.Regain;

[ApiVersion(2, 1)]
public class Regain : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => GetString("对玩家的物品进行回收!");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2, 0, 0, 4);

    public Regain(Main game) : base(game)
    {
    }

    internal static Config Config { get; set; } = new();

    public override void Initialize()
    {
        Config.Load();
        Commands.ChatCommands.Add(new Command("economics.regain", this.CRegain, "regain"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Core.Economics.RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            Core.Economics.RemoveAssemblyRest(Assembly.GetExecutingAssembly());
            Config.UnLoad();
        }
        base.Dispose(disposing);
    }


    private void CRegain(CommandArgs args)
    {
        void Show(List<string> line)
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
                        MaxLinesPerPage = Config.PageMax,
                        NothingToDisplayString = GetString("当前可回收物品"),
                        HeaderFormat = GetString("回收物品列表 ({0}/{1})："),
                        FooterFormat = GetString("输入 {0}regain list {{0}} 查看更多").SFormat(Commands.Specifier)
                    }
                );
        }
        bool Verify(out Config.RegainInfo? regain)
        {
            if (!Config.TryGetRegain(args.Player.SelectedItem.type, out regain) || regain == null)
            {
                args.Player.SendErrorMessage(GetString("该物品暂时无法回收!"));
                return false;
            }
            if (args.Player.SelectedItem.stack == 0 || args.Player.SelectedItem.type == 0)
            {
                args.Player.SendErrorMessage(GetString("请手持一个有效物品!"));
                return false;
            }
            return true;
        }
        if (!args.Player.RealPlayer || !args.Player.IsLoggedIn)
        {
            args.Player.SendErrorMessage(GetString("你必须登录游戏使用此命令!"));
            return;
        }

        switch (args.Parameters.Count)
        {
            case 0:
            {
                if (!Verify(out var regain) || regain == null)
                {
                    return;
                }
                var sb = new StringBuilder();
                foreach (var rro in regain.RedemptionRelationshipsOption)
                {
                    var num = args.Player.SelectedItem.stack * rro.Number;
                    Core.Economics.CurrencyManager.AddUserCurrency(args.Player.Name, num, rro.CurrencyType);
                    sb.Append($"{rro.CurrencyType}x{num} ");
                }
                args.Player.SelectedItem.stack = 0;
                args.Player.SendData(PacketTypes.PlayerSlot, "", args.Player.Index, args.Player.TPlayer.selectedItem);
                args.Player.SendSuccessMessage(GetString($"成功兑换{sb.ToString().Trim()}"));
                break;
            }
            case 1:
            case 2:
            {
                if (args.Parameters[0].ToLower() == "list")
                {
                    var line = Config.Regains.Select(x => x.ToString()).ToList();
                    Show(line);
                    return;
                }
                if (!int.TryParse(args.Parameters[0], out var count) && count > 0)
                {
                    args.Player.SendErrorMessage(GetString($"值{args.Parameters[0]}无效!"));
                    return;
                }
                if (!Verify(out var regain) || regain == null)
                {
                    return;
                }

                count = count > args.Player.SelectedItem.stack ? args.Player.SelectedItem.stack : count;
                var sb = new StringBuilder();
                foreach (var rro in regain.RedemptionRelationshipsOption)
                {
                    var num = count * rro.Number;
                    Core.Economics.CurrencyManager.AddUserCurrency(args.Player.Name, num, rro.CurrencyType);
                    sb.Append($"{rro.CurrencyType}x{num} ");
                }
                args.Player.SelectedItem.stack -= count;
                args.Player.SendData(PacketTypes.PlayerSlot, "", args.Player.Index, args.Player.TPlayer.selectedItem);
                args.Player.SendSuccessMessage(GetString($"成功兑换{sb.ToString().Trim()}"));
                break;
            }
            default:
                args.Player.SendInfoMessage(GetString("/regain 语法"));
                args.Player.SendInfoMessage(GetString("/regain"));
                args.Player.SendInfoMessage(GetString("/regain [数量]"));
                args.Player.SendInfoMessage(GetString("/regain list"));
                break;

        }
    }
}