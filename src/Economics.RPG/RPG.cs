using Economics.RPG.Setting;
using EconomicsAPI.Configured;
using EconomicsAPI.EventArgs.PlayerEventArgs;
using EconomicsAPI.Events;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Economics.RPG;

[ApiVersion(2, 1)]
public class RPG : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => GetString("提供RPG玩法!");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2, 0, 0, 4);

    public static Config Config { get; set; } = new Config();

    public static PlayerLevelManager PlayerLevelManager { get; private set; } = null!;

    private static string PATH => Path.Combine(EconomicsAPI.Economics.SaveDirPath, "RPG.json");

    public RPG(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        this.LoadConfig();
        PlayerLevelManager = new();
        PlayerHooks.PlayerPermission += this.PlayerHooks_PlayerPermission;
        PlayerHooks.PlayerChat += this.PlayerHooks_PlayerChat;
        PlayerHandler.OnPlayerCountertop += this.OnCounterTop;
        GeneralHooks.ReloadEvent += this.LoadConfig;
    }

    private void LoadConfig(ReloadEventArgs? args = null)
    {
        Config = ConfigHelper.LoadConfig(PATH, Config);
        Config.Init();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            EconomicsAPI.Economics.RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            EconomicsAPI.Economics.RemoveAssemblyRest(Assembly.GetExecutingAssembly());
            PlayerHooks.PlayerPermission -= this.PlayerHooks_PlayerPermission;
            PlayerHooks.PlayerChat -= this.PlayerHooks_PlayerChat;
            PlayerHandler.OnPlayerCountertop -= this.OnCounterTop;
            GeneralHooks.ReloadEvent -= this.LoadConfig;
        }
        base.Dispose(disposing);
    }


    public static bool InLevel(string Name, IEnumerable<string> list)
    {
        var level = PlayerLevelManager.GetLevel(Name);
        if (!list.Any() || list.Contains(level.Name))
        {
            return true;
        }

        foreach (var name in list)
        {
            if (level.AllParentLevels.Any(x => x.Name == name))
            {
                return true;
            }
        }
        return false;
    }

    private void OnCounterTop(PlayerCountertopArgs args)
    {
        var level = PlayerLevelManager.GetLevel(args.Player!.Name);
        args.Messages.Add(new(GetString($"当前职业: {level.Name}"), 10));
        args.Messages.Add(new(GetString($"升级职业: {string.Join(",", level.RankLevels.Select(x => $"{x.Name}"))}"), 11));
    }

    private void PlayerHooks_PlayerChat(PlayerChatEventArgs e)
    {
        var level = PlayerLevelManager.GetLevel(e.Player.Name);
        if (!e.Player.HasPermission("economics.rpg.chat") && level != null && !string.IsNullOrEmpty(level.ChatFormat))
        {
            TShock.Utils.Broadcast(EconomicsAPI.Utils.Helper.GetGradientText(string.Format(level.ChatFormat,
                e.Player.Group.Name, //{0} 组名
                level.Name,          //{1} 职业名
                level.ChatPrefix,    //{2} 聊天前缀
                e.Player.Name,       //{3} 玩家名
                level.ChatSuffix,    //{4} 聊天后缀
                e.RawText)),
                (byte) level.ChatRGB[0],
                (byte) level.ChatRGB[1],
                (byte) level.ChatRGB[2]);
            e.Handled = true;
        }
    }

    private void PlayerHooks_PlayerPermission(PlayerPermissionEventArgs e)
    {
        var level = PlayerLevelManager.GetLevel(e.Player.Name);
        if (level != null && level.AllPermission != null && level.AllPermission.Contains(e.Permission))
        {
            e.Result = PermissionHookResult.Granted;
        }
    }
}
