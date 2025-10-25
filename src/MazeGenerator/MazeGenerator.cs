using MazeGenerator.Commands;
using MazeGenerator.Models;
using MazeGenerator.Services;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace MazeGenerator;

[ApiVersion(2, 1)]
public class MazeGenerator : TerrariaPlugin
{
    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "Eustia";
    public override string Description => GetString("迷宫生成器");
    public override Version Version => new (1, 0, 0, 0);

    public static MazeGenerator Instance { get; private set; } = null!;
    public MazeGameManager GameManager { get; private set; } = null!;
    public MazeBuilder MazeBuilder { get; private set; } = null!;
    public LeaderboardService Leaderboard { get; private set; } = null!;
    private MazeCommandHandler _commandHandler = null!;

    public MazeGenerator(Main game) : base(game)
    {
        Instance = this;
    }

    public override void Initialize()
    {
        this.GameManager = new MazeGameManager();
        this.MazeBuilder = new MazeBuilder();
        this.Leaderboard = new LeaderboardService();
        this._commandHandler = new MazeCommandHandler();

        this.MazeBuilder.Initialize();
        this.Leaderboard.Initialize();
        this._commandHandler.Initialize();

        GetDataHandlers.TileEdit += this.OnTileEdit;
        GeneralHooks.ReloadEvent += this.OnReload;
        ServerApi.Hooks.GamePostUpdate.Register(this, this.OnGameUpdate);
        GetDataHandlers.PlayerUpdate += this.OnPlayerUpdate;
        ServerApi.Hooks.ServerLeave.Register(this, this.OnServerLeave);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.TileEdit -= this.OnTileEdit;
            GeneralHooks.ReloadEvent -= this.OnReload;
            ServerApi.Hooks.GamePostUpdate.Deregister(this, this.OnGameUpdate);
            GetDataHandlers.PlayerUpdate -= this.OnPlayerUpdate;
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnServerLeave);

            this.GameManager.Dispose();
            this.MazeBuilder.Dispose();
            this.Leaderboard.Dispose();
            this._commandHandler.Dispose();
        }

        base.Dispose(disposing);
    }

    private void OnTileEdit(object? sender, GetDataHandlers.TileEditEventArgs args)
    {
        this._commandHandler.HandleTileEdit(args);
    }

    private void OnReload(ReloadEventArgs e)
    {
            Config.Load();
            this.MazeBuilder.Reload();
            this.Leaderboard.Reload();
    }

    private void OnGameUpdate(EventArgs args)
    {
        this.GameManager.Update();
    }

    private void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs args)
    {
        this.GameManager.HandlePlayerUpdate(args);
    }

    private void OnServerLeave(LeaveEventArgs args)
    {
        if (args.Who < 0 || args.Who >= TShock.Players.Length)
        {
            return;
        }

        var player = TShock.Players[args.Who];
        if (player != null && !string.IsNullOrEmpty(player.Name))
        {
            this.GameManager.HandlePlayerLeave(player.Name);
        }
    }
}