using DeltaForce.Core.Modules;
using LazyAPI;
using LazyAPI.Utility;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace DeltaForce.Core;

[ApiVersion(2, 1)]
public partial class DeltaAction(Main game) : LazyPlugin(game)
{
    public override string Author => "少司命";
    public override string Name => "三角洲行动";
    public override string Description => "三角洲行动是一个基于Terraria的模组，旨在为玩家提供全新的游戏体验。";
    public override Version Version => new(1, 0, 0, 0);

    internal static DeltaServer DeltaServer = new(Config.Instance.Socket);

    public override void Initialize()
    {
        Main.ServerSideCharacter = true;
        Task.Run(async () => await DeltaServer.StartAsync());
        SSCHook.Initialize();
        TimingUtils.Schedule(60, OnTimer);
        ServerApi.Hooks.ServerLeave.Register(this, OnPlayerLeave);
        GetDataHandlers.PlayerTeam.Register(OnPlayerTeam);
    }

    private void OnPlayerTeam(object? sender, GetDataHandlers.PlayerTeamEventArgs e)
    {
        e.Handled = true;
    }

    private void OnPlayerLeave(LeaveEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null)
            return;
        SquadMatchManager.RemovePlayerFromSquad(player);
    }

    private void OnTimer()
    {
        SquadMatchManager.CheckSquadMatchCompletion();
    }
}
