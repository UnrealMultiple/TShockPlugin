using StatusTextManager.Utils;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Configuration;
using TShockAPI.Hooks;

namespace StatusTextManager;

[ApiVersion(2, 1)]
public class StatusTextManager : TerrariaPlugin
{
    #region Cmds

    private void CommandSt(CommandArgs args)
    {
        switch (args.Parameters.Count)
        {
            case 0:
                this._isPlayerStatusTextVisible[args.Player.Index] = !this._isPlayerStatusTextVisible[args.Player.Index];
                if (this._isPlayerStatusTextVisible[args.Player.Index])
                {
                    this._doesPlayerNeedInit[args.Player.Index] = true;
                    args.Player.SendSuccessMessage(GetString("已开启模板显示"));
                }
                else
                {
                    this._doesPlayerNeedInit[args.Player.Index] = false;
                    args.Player.SendData(PacketTypes.Status, "", 0, 0x1f);
                    args.Player.SendSuccessMessage(GetString("已关闭模板显示"));
                }

                break;

            case 1:
                switch (args.Parameters[0])
                {
                    case "on":
                    case "show":
                        if (!this._isPlayerStatusTextVisible[args.Player.Index])
                        {
                            this._isPlayerStatusTextVisible[args.Player.Index] = true;
                            this._doesPlayerNeedInit[args.Player.Index] = true;
                        }

                        args.Player.SendSuccessMessage(GetString("已开启模板显示"));
                        break;

                    case "off":
                    case "hide":
                        if (this._isPlayerStatusTextVisible[args.Player.Index])
                        {
                            this._isPlayerStatusTextVisible[args.Player.Index] = false;
                            this._doesPlayerNeedInit[args.Player.Index] = false;
                            args.Player.SendData(PacketTypes.Status, "", 0, 0x1f);
                        }

                        args.Player.SendSuccessMessage(GetString("已关闭模板显示"));
                        break;

                    // case "help":
                    default:
                        args.Player.SendInfoMessage(GetString("用法：/st <on/show/off/hide>"));
                        break;
                }

                break;
        }
    }

    #endregion

    #region Plugin Infos

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "LaoSparrow";
    public override string Description => GetString("Manage status text of different plugins");
    public override Version Version => new Version(1, 1, 3);

    #endregion

    #region Fields

    // Config
    private static readonly string ConfigFilePath = Path.Combine(TShock.SavePath, "StatusTextManager.json");
    private static readonly ConfigFile<Settings> Config = new();
    internal static Settings Settings => Config.Settings;

    // States
    private readonly bool[] _doesPlayerNeedInit = new bool[Main.maxPlayers];
    private readonly bool[] _isPlayerStatusTextVisible = new bool[Main.maxPlayers];

    #endregion

    #region Initialize / Dispose

    public StatusTextManager(Main game) : base(game)
    {
        this.Order = 1;
    }

    public override void Initialize()
    {
        ServerApi.Hooks.GameInitialize.Register(this, this.OnGameInitialize);
        ServerApi.Hooks.GamePostUpdate.Register(this, this.OnGamePostUpdate);
        ServerApi.Hooks.ServerJoin.Register(this, this.OnServerJoin);
        GeneralHooks.ReloadEvent += this.OnReload;

        Commands.ChatCommands.Add(new Command(this.CommandSt, "statustext", "st"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameInitialize.Deregister(this, this.OnGameInitialize);
            ServerApi.Hooks.GamePostUpdate.Deregister(this, this.OnGamePostUpdate);
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnServerJoin);
            GeneralHooks.ReloadEvent -= this.OnReload;

            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == this.CommandSt);
        }

        base.Dispose(disposing);
    }

    #endregion

    #region Hooks

    private void OnGameInitialize(EventArgs args)
    {
        // Config Loading
        if (!Directory.Exists(TShock.SavePath))
        {
            Directory.CreateDirectory(TShock.SavePath);
        }

        this.LoadConfig();
    }

    private void OnReload(ReloadEventArgs args)
    {
        this.LoadConfig();
    }

    private void LoadConfig()
    {
        try
        {
            Config.Read(ConfigFilePath, out var incompleteSettings);
            if (incompleteSettings)
            {
                Config.Write(ConfigFilePath);
            }

            Hooks.OnStatusTextUpdate.LoadSettings();
            foreach (var p in Common.PlayersOnline)
            {
                this._doesPlayerNeedInit[p.Index] = true;
            }
        }
        catch (Exception ex)
        {
            Logger.Warn($"Failed to load config, Exception:\n{ex}");
        }
    }

    private void OnGamePostUpdate(EventArgs args)
    {
        try
        {
            foreach (var tsplr in Common.PlayersOnline)
            {
                if (!this._isPlayerStatusTextVisible[tsplr.Index])
                {
                    continue;
                }

                var sb = StringBuilderCache.Acquire();
                if (Hooks.OnStatusTextUpdate.Invoke(tsplr, sb, this._doesPlayerNeedInit[tsplr.Index]))
                {
                    tsplr.SendData(PacketTypes.Status, StringBuilderCache.GetStringAndRelease(sb), 0, 0x1f);
                    // 0x1f -> HideStatusTextPercent
                }
                else
                {
                    StringBuilderCache.Release(sb);
                }

                this._doesPlayerNeedInit[tsplr.Index] = false;
            }

            Common.CountTick();
        }
        catch (Exception ex)
        {
            Logger.Warn($"An exception has been thrown in OnGamePostUpdate:\n{ex}");
        }
    }

    private void OnServerJoin(JoinEventArgs args)
    {
        this._isPlayerStatusTextVisible[args.Who] = true;
        this._doesPlayerNeedInit[args.Who] = true;
    }

    #endregion
}