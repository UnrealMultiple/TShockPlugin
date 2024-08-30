namespace MiniGamesAPI;

public class GameSecondArgs
{
    public MiniPlayer Player { get; set; }

    public GameSecondArgs(MiniPlayer player)
    {
        this.Player = player;
    }
}