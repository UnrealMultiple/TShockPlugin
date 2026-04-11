using DeltaForce.Core.Enitys;
using DeltaForce.Core.Enums;
using Microsoft.Xna.Framework;
using TShockAPI;

namespace DeltaForce.Core.Modules;

public class SquadMatchManager
{
    public const int MaxSquadMembers = 3;

    public const int MinSquads = 3;

    private static bool _isMatch = false;

    private static int _matchCount = 0;

    private static int _lastNotifiedSecond = -1;

    private static readonly Dictionary<TeamType, SquadEnity> squads = Enum.GetValues<TeamType>()
        .Where(t => t != TeamType.None)
        .ToDictionary(t => t, t => new SquadEnity() { Team = t });

    //squad match completed event, the dictionary is readonly and will not be modified after the event is invoked
    public static event Action<Dictionary<TeamType, SquadEnity>>? SquadMatchCompleted;

    public static void AddPlayerToSquad(TSPlayer player)
    {
        if (squads.Values.Any(s => s.Members.Contains(player)))
        {
            player.SendErrorMessage("You are already in a squad.");
            return;
        }
        SquadEnity? targetSquad = squads.Values
            .Where(s => s.Members.Count < MaxSquadMembers)
            .OrderBy(s => s.Members.Count)
            .FirstOrDefault();
        if (targetSquad == null)
            return;
        targetSquad.Value.Members.Add(player);
        _isMatch = true;

        var currentSquadCount = squads.Values.Count(s => s.Members.Count > 0);
        var totalPlayers = squads.Values.Sum(s => s.Members.Count);
        player.SendSuccessMessage($"已进入匹配队列！当前 {currentSquadCount} 个小队，共 {totalPlayers} 名玩家正在匹配...");
    }

    public static void RemovePlayerFromSquad(TSPlayer player) => squads.Values.FirstOrDefault(x => x.Members.Contains(player)).Members.Remove(player);

    public static Dictionary<TeamType, SquadEnity> GetAllSquads()
    {
        return squads;
    }

    public static void CheckSquadMatchCompletion()
    {
        if (!_isMatch)
            return;

        var remainingSeconds = Config.Instance.MatchSeconds - _matchCount;

        SendCountdownNotification(remainingSeconds);

        if (_matchCount >= Config.Instance.MatchSeconds)
        {
            if(squads.Values.Count(s => s.Members.Count > 0) < MinSquads)
            {
                _matchCount = 0;
                _lastNotifiedSecond = -1;
                TShock.Utils.Broadcast("没有足够的玩家匹配，无法进入对局，匹配结束！", Color.Red);
                ResetMatch();
                return;
            }
            _ = CommunicationManager.StartGameASync().Result;
            SendPlayerToGameServer();
            ResetMatch();
            return;
        }
        _matchCount++;
    }

    private static void SendCountdownNotification(int remainingSeconds)
    {
        var currentSquadCount = squads.Values.Count(s => s.Members.Count > 0);
        var totalPlayers = squads.Values.Sum(s => s.Members.Count);

        if (remainingSeconds <= 10 && remainingSeconds > 0 && remainingSeconds != _lastNotifiedSecond)
        {
            _lastNotifiedSecond = remainingSeconds;

            var message = $"[匹配倒计时] 还有 {remainingSeconds} 秒开始游戏！当前 {currentSquadCount} 个小队，共 {totalPlayers} 名玩家";
            var color = remainingSeconds <= 5 ? Color.OrangeRed : Color.Yellow;

            BroadcastToMatchingPlayers(message, color);
        }
        else if (remainingSeconds == 30 && remainingSeconds != _lastNotifiedSecond)
        {
            _lastNotifiedSecond = remainingSeconds;
            var message = $"[匹配提示] 还有 30 秒开始游戏！当前 {currentSquadCount} 个小队，共 {totalPlayers} 名玩家";
            BroadcastToMatchingPlayers(message, Color.LightGreen);
        }
        else if (remainingSeconds == 60 && remainingSeconds != _lastNotifiedSecond)
        {
            _lastNotifiedSecond = remainingSeconds;
            var message = $"[匹配提示] 还有 60 秒开始游戏！当前 {currentSquadCount} 个小队，共 {totalPlayers} 名玩家";
            BroadcastToMatchingPlayers(message, Color.LightGreen);
        }
    }

    private static void BroadcastToMatchingPlayers(string message, Color color)
    {
        foreach (var squad in squads.Values)
        {
            foreach (var player in squad.Members)
            {
                if (player?.Active == true)
                {
                    player.SendMessage(message, color);
                }
            }
        }

        TShock.Log.ConsoleInfo($"[SquadMatchManager] {message}");
    }

    private static void ResetMatch()
    {
        _isMatch = false;
        _matchCount = 0;
        _lastNotifiedSecond = -1;
    }

    private static void SendPlayerToGameServer()
    {
        var gameServer = Config.Instance.GameServer;
        squads.Values.Where(s => s.Members.Count > 0).ToList().ForEach(s =>
        {
            s.Members.ForEach(p =>
            {
                DimensionsSender.SendPlayerToCustomServer(p, gameServer.Address, gameServer.Port);
            });
        });
    }
}
