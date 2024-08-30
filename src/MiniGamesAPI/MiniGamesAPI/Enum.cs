namespace MiniGamesAPI;

public static class Enum
{
    public enum RoomStatus
    {
        Waiting,
        Selecting,
        Gaming,
        Concluding,
        Restoring,
        Stopped
    }

    public enum PlayerStatus
    {
        Waiting,
        Selecting,
        Gaming
    }
}