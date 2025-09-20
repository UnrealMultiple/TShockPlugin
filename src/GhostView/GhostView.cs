using GhostView.Managers;
using GhostView.Models;
using GhostView.Service;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace GhostView;

[ApiVersion(2, 1)]
public class GhostView : TerrariaPlugin
{
    private RespawnManager? _manager;

    public GhostView(Main game) : base(game) { }

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "Eustia";
    public override string Description => "死亡后能在鬼魂状态下观战，重连不刷新复活cd";
    public override Version Version => new (1, 0, 0);

    public override void Initialize()
    {
        var respawnService = new RespawnService();
        var respawnCountdown = new RespawnCountdown();

        this._manager = new RespawnManager(respawnService, respawnCountdown);

        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreetPlayer);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
        GetDataHandlers.PlayerSpawn.Register(this.OnPlayerSpawn);
        GetDataHandlers.KillMe.Register(this.OnKillMe);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreetPlayer);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);
            GetDataHandlers.PlayerSpawn.UnRegister(this.OnPlayerSpawn);
            GetDataHandlers.KillMe.UnRegister(this.OnKillMe);
        }

        base.Dispose(disposing);
    }

    private void OnPlayerSpawn(object? sender, GetDataHandlers.SpawnEventArgs e)
    {
        this.WithPlayer(e.PlayerId, name => this._manager?.OnPlayerSpawn(name));
    }

    private void OnGreetPlayer(GreetPlayerEventArgs e)
    {
        this.WithPlayer(e.Who, name => this._manager?.OnGreetPlayer(name));
    }

    private void OnLeave(LeaveEventArgs e)
    {
        this.WithPlayer(e.Who, name => this._manager?.OnLeave(name));
    }

    private void OnKillMe(object? sender, GetDataHandlers.KillMeEventArgs e)
    {
        this.WithPlayer(e.PlayerId, name => this._manager?.OnKillMe(name));
    }

    private void WithPlayer(int playerIndex, Action<string> action)
    {
        var player = TShock.Players[playerIndex];
        if (player != null)
        {
            action(player.Name);
        }
    }
}