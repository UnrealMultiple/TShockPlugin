using OTAPI;
using ProxyProtocolSocket.Utils;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Configuration;
using TShockAPI.Hooks;

namespace ProxyProtocolSocket;

[ApiVersion(2, 1)]
// ReSharper disable once ClassNeverInstantiated.Global
public class ProxyProtocolSocketPlugin : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "LaoSparrow";
    public override string Description => GetString("Proxy Protocol on TShock");
    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    public static string ConfigFileName = "ProxyProtocolSocket.json";
    public static string ConfigFilePath => Path.Combine(TShock.SavePath, ConfigFileName);
    public static ConfigFile<ProxyProtocolSocketSettings> Config = new();

    public ProxyProtocolSocketPlugin(Main game) : base(game)
    {
        // Must be the last to handle "OTAPI.Hooks.Netplay.CreateTcpListener"
        // Otherwise the plugin will not function
        this.Order = 1000;
    }

    public override void Initialize()
    {
        ServerApi.Hooks.GameInitialize.Register(this, this.OnGameInitialize);
        GeneralHooks.ReloadEvent += this.OnReload;
        // Override the netplay listening socket
        OTAPI.Hooks.Netplay.CreateTcpListener += this.OnSocketCreate;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameInitialize.Deregister(this, this.OnGameInitialize);
            GeneralHooks.ReloadEvent -= this.OnReload;
            OTAPI.Hooks.Netplay.CreateTcpListener -= this.OnSocketCreate;
        }
        base.Dispose(disposing);
    }

    private void OnGameInitialize(EventArgs args)
    {
        if (!Directory.Exists(TShock.SavePath))
        {
            Directory.CreateDirectory(TShock.SavePath);
        }

        LoadConfig();
    }

    private void OnReload(ReloadEventArgs args)
    {
        LoadConfig();
    }

    private void OnSocketCreate(object? _, Hooks.Netplay.CreateTcpListenerEventArgs args)
    {
        Logger.Log("OnSocketCreate called!");
        Logger.Log($"Listening on port {Netplay.ListenPort} through proxy protocol v1 and v2", LogLevel.Info);
        args.Result = new Utils.Net.ProxyProtocolSocket();
    }

    private static void LoadConfig()
    {
        Logger.Log("Loading config!");
        var writeConfig = true;
        if (File.Exists(ConfigFilePath))
        {
            Config.Read(ConfigFilePath, out writeConfig);
        }

        if (writeConfig)
        {
            Config.Write(ConfigFilePath);
        }
    }
}