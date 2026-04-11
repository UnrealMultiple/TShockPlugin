using DeltaForce.Game.Modules;
using LazyAPI;
using LazyAPI.Utility;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace DeltaForce.Game;

[ApiVersion(2, 1)]
public class Game(Main game) : LazyPlugin(game)
{
    public override string Author => "少司命";

    public override string Description => "三角洲游戏服务器";

    public override string Name => "三角洲行动";

    public override Version Version => new(1, 0, 0, 0);

    internal static Client Client { get; private set; } = new();

    public override void Initialize()
    {
        Main.ServerSideCharacter = true;
        TimingUtils.Schedule(60, GameManager.GameLoop);
        Task.Run(() => Client.ConnectAsync(Config.Instance.SocketOption.Address, Config.Instance.SocketOption.Port));
        ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreet);
        ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);
        GetDataHandlers.KillMe.Register(OnKillMe);
        GetDataHandlers.TogglePvp.Register(OnTogglePvp);
        GetDataHandlers.PlayerTeam.Register(OnPlayerTeam);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreet);
            ServerApi.Hooks.ServerLeave.Deregister(this, OnServerLeave);
            GetDataHandlers.KillMe.UnRegister(OnKillMe);
            GetDataHandlers.TogglePvp.UnRegister(OnTogglePvp);
            GetDataHandlers.PlayerTeam.UnRegister(OnPlayerTeam);
        }
        base.Dispose(disposing);
    }

    private void OnTogglePvp(object? sender, GetDataHandlers.TogglePvpEventArgs e)
    {
        e.Player.SetPvP(true);
        e.Handled = true;
    }

    private void OnGreet(GreetPlayerEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if(player != null)
        {
            var team = GameManager.GetPlayerTeam(player);
            player.SetTeam(team);
            player.SetPvP(true);
        }
    }

    private void OnServerLeave(LeaveEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null) return;

        if (GameManager._isGame)
        {
            TShock.Log.ConsoleInfo($"[Game] 玩家 {player.Name} 在游戏进行中主动离开，保存背包数据");
            try
            {
                var response = InventoryManager.SavePlayerInventory(player);

                if (response?.Success == true)
                {
                    TShock.Log.ConsoleInfo($"[Game] 玩家 {player.Name} 背包数据已同步到特勤处");
                }
                else
                {
                    TShock.Log.ConsoleInfo($"[Game] 玩家 {player.Name} 背包数据同步失败: {response?.Message}");
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleInfo($"[Game] 保存玩家 {player.Name} 背包数据时发生错误: {ex.Message}");
            }

            GameManager.CheckAllPlayersLeft();
        }
    }

    private void OnPlayerTeam(object? sender, GetDataHandlers.PlayerTeamEventArgs e)
    {
        var team = GameManager.GetPlayerTeam(e.Player);
        e.Player.SetTeam(team);
        e.Handled = true;
    }

    private void OnKillMe(object? sender, GetDataHandlers.KillMeEventArgs e)
    {
        GameUitls.ClearPlayerInventory(e.Player, true);
        e.Player.TPlayer.ghost = true;
        TSPlayer.All.SendData(PacketTypes.PlayerUpdate, "", e.Player.Index);
    }
}
