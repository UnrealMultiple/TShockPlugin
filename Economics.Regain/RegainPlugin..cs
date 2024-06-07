using EconomicsAPI.Configured;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Economics.Regain;

[ApiVersion(2, 1)]
public class RegainPlugin : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    internal static string PATH = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "Regain.json");

    public RegainPlugin(Main game) : base(game)
    {
    }

    internal static Config Config { get; set; } = new();

    public override void Initialize()
    {
        Config = ConfigHelper.LoadConfig<Config>(PATH);
        GeneralHooks.ReloadEvent += (e) => Config = ConfigHelper.LoadConfig(PATH, Config);
        Commands.ChatCommands.Add(new("economics.regain", CRegain, "回收", "regain"));
    }

    private void CRegain(CommandArgs args)
    {
        void Show(List<string> line)
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out int pageNumber))
                return;

            PaginationTools.SendPage(
                    args.Player,
                    pageNumber,
                    line,
                    new PaginationTools.Settings
                    {
                        MaxLinesPerPage = Config.PageMax,
                        NothingToDisplayString = "当前可回收物品",
                        HeaderFormat = "回收物品列表 ({0}/{1})：",
                        FooterFormat = "输入 {0}regain list {{0}} 查看更多".SFormat(Commands.Specifier)
                    }
                );
        }
        bool Verify(out Config.RegainInfo? regain)
        {
            if (!Config.TryGetRegain(args.Player.SelectedItem.netID, out regain) || regain == null)
            {
                args.Player.SendErrorMessage("该物品暂时无法回收!");
                return false;
            }
            if (args.Player.SelectedItem.stack == 0 || args.Player.SelectedItem.netID == 0)
            {
                args.Player.SendErrorMessage("请手持一个有效物品!");
                return false;
            }
            return true;
        }
        if (!args.Player.RealPlayer || !args.Player.IsLoggedIn)
        {
            args.Player.SendErrorMessage("你必须登录游戏使用此命令!");
            return;
        }

        switch (args.Parameters.Count)
        {
            case 0:
                {
                    if (!Verify(out var regain) || regain == null)
                        return;
                    var num = args.Player.SelectedItem.stack * regain.Cost;
                    EconomicsAPI.Economics.CurrencyManager.AddUserCurrency(args.Player.Name, num);
                    args.Player.SelectedItem.stack = 0;
                    args.Player.SendData(PacketTypes.PlayerSlot, "", args.Player.Index, args.Player.TPlayer.selectedItem);
                    args.Player.SendSuccessMessage($"成功兑换{num}个{EconomicsAPI.Economics.Setting.CurrencyName}!");
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
                        args.Player.SendErrorMessage($"值{args.Parameters[0]}无效!");
                        return;
                    }
                    if (!Verify(out var regain) || regain == null)
                        return;
                    count = count > args.Player.SelectedItem.stack ? args.Player.SelectedItem.stack : count;
                    EconomicsAPI.Economics.CurrencyManager.AddUserCurrency(args.Player.Name, count * regain.Cost);
                    args.Player.SelectedItem.stack -= count;
                    args.Player.SendData(PacketTypes.PlayerSlot, "", args.Player.Index, args.Player.TPlayer.selectedItem);
                    args.Player.SendSuccessMessage($"成功兑换{count * regain.Cost}个{EconomicsAPI.Economics.Setting.CurrencyName}!");
                    break;
                }
            default:
                args.Player.SendInfoMessage("/regain语法");
                args.Player.SendInfoMessage("/regain");
                args.Player.SendInfoMessage("/regain [数量]");
                args.Player.SendInfoMessage("/regain list");
                break;

        }
    }
}
