using Economics.RPG.Setting;
using EconomicsAPI.Configured;
using Newtonsoft.Json;
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

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    internal static Config Config { get; set; } = new Config();

    public static PlayerLevelManager PlayerLevelManager { get; private set; }

    private static string PATH => Path.Combine(EconomicsAPI.Economics.SaveDirPath, "RPG.json");

    public RPG(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Config = ConfigHelper.LoadConfig<Config>(PATH);
        Config.Init();
        PlayerLevelManager = new();
        PlayerHooks.PlayerPermission += PlayerHooks_PlayerPermission;
        PlayerHooks.PlayerChat += PlayerHooks_PlayerChat;
        GeneralHooks.ReloadEvent += (_) =>
        {
            Config = ConfigHelper.LoadConfig(PATH, Config);
            Config.Init();
        };
    }

    private void PlayerHooks_PlayerChat(PlayerChatEventArgs e)
    {
        var level = PlayerLevelManager.GetLevel(e.Player.Name);
        if (level != null && !string.IsNullOrEmpty(level.ChatFormat))
        {
            TShock.Utils.Broadcast(string.Format(level.ChatFormat,
                level.ChatPrefix,
                e.Player.Name, 
                level.ChatSuffix, 
                e.RawText), 
                (byte)level.ChatRGB[0], 
                (byte)level.ChatRGB[1], 
                (byte)level.ChatRGB[2]);
            e.Handled = true;
        }
    }

    private void PlayerHooks_PlayerPermission(PlayerPermissionEventArgs e)
    {
        var level = PlayerLevelManager.GetLevel(e.Player.Name);
        if (level != null && level.AllPermission != null && level.AllPermission.Contains(e.Permission))
            e.Result = PermissionHookResult.Granted;
    }
}
