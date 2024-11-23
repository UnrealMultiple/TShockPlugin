namespace MiniGamesAPI;

public static class HookManager
{
    public delegate void GameSecondD(GameSecondArgs args);

    public delegate void JoinRoomD(JoinRoomArgs args);

    public delegate void LeaveRoomD(LeaveRoomArgs args);

    public static GameSecondD? GameSecond;

    public static JoinRoomD? JoinRoom;

    public static LeaveRoomD? LeaveRoom;

    public static void OnGameSecond(MiniPlayer player)
    {
        GameSecond?.Invoke(new GameSecondArgs(player));
    }
}