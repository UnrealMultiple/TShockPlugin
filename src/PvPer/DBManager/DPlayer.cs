namespace PvPer;

public class DPlayer
{
    public int AccountID { get; set; }
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public int Rating { get; set; }
    public int RatingDeviation { get; set; }
    public int RatingVolatility { get; set; }

    public int WinStreak { get; set; } // 添加WinStreak属性

    public DPlayer(int accountID, int kills = 0, int deaths = 0, int winStreak = 0)
    {
        this.AccountID = accountID;
        this.Kills = kills;
        this.Deaths = deaths;
        this.WinStreak = winStreak;
    }

    public double GetKillDeathRatio()
    {
        return this.Deaths == 0 ? this.Kills : (double) this.Kills / this.Deaths;
    }
}