

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
        if (!args.Player.RealPlayer || !args.Player.IsLoggedIn)
        {
            args.Player.SendErrorMessage("你必须登录游戏使用此命令!");
            return;
        }
        if (!Config.TryGetRegain(args.Player.SelectedItem.netID, out var regain) || regain == null)
        {
            args.Player.SendErrorMessage("该物品暂时无法回收!");
            return;
        }
        if (args.Player.SelectedItem.stack == 0 || args.Player.SelectedItem.netID == 0)
        {
            args.Player.SendErrorMessage("请手持一个有效物品!");
            return;
        }
        switch (args.Parameters.Count)
        {
            case 0:
                { 
                    var num = args.Player.SelectedItem.stack * regain.Cost;
                    EconomicsAPI.Economics.CurrencyManager.AddUserCurrency(args.Player.Name, num);
                    args.Player.SendSuccessMessage($"成功兑换{num}个{EconomicsAPI.Economics.Setting.CurrencyName}!");
                    break;
                }
            case 1:
                { 
                    if (!int.TryParse(args.Parameters[0], out var count) && count > 0)
                    {
                        args.Player.SendErrorMessage($"值{args.Parameters[0]}无效!");
                        return;
                    }
                    var num = count > args.Player.SelectedItem.stack ?  args.Player.SelectedItem.stack * regain.Cost : count* regain.Cost;
                    EconomicsAPI.Economics.CurrencyManager.AddUserCurrency(args.Player.Name, num);
                    args.Player.SendSuccessMessage($"成功兑换{num}个{EconomicsAPI.Economics.Setting.CurrencyName}!");
                    break;
                }
            default:
                args.Player.SendInfoMessage("/regain语法");
                args.Player.SendInfoMessage("/regain");
                args.Player.SendInfoMessage("/regain [数量]");
                break;
               
        }
    }
}
