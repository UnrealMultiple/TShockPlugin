namespace PvPer
{
    public class DPlayer
    {
        public int AccountID { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Rating { get; set; }
        public int RatingDeviation { get; set; }
        public int RatingVolatility { get; set; }

        public int WinStreak { get; set; } // Ìí¼ÓWinStreakÊôĞÔ

        public DPlayer(int accountID, int kills = 0, int deaths = 0, int winStreak = 0)
        {
            AccountID = accountID;
            Kills = kills;
            Deaths = deaths;
            WinStreak = winStreak;
        }

        public double GetKillDeathRatio()
        {
            return Deaths == 0 ? Kills : (double)Kills / Deaths;
        }
    }
}