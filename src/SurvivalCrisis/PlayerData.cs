using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SurvivalCrisis
{
	public partial class PlayerData
    {
        public string Name { get; set; }
        public int UserID { get; set; }
        [JsonIgnore]
        public int Score
        {
            get => SurvivorDatas.TotalScore + TraitorDatas.TotalScore;
        }
        public int Coins
        {
            get;
            set;
        }

        public PlayerBoosts Boosts
		{
            get;
            set;
		}

        public List<int> UnlockedTitles
		{
            get;
            set;
		}
        public List<int> UnlockedPrefixs
		{
            get;
            set;
		}
        public int CurrentTitleID
		{
            get;
            set;
		}
        public int CurrentPrefixID
		{
            get;
            set;
		}

        public DataDetail SurvivorDatas { get; set; }
        public DataDetail TraitorDatas { get; set; }

        [JsonIgnore]
        public int GameCounts
        {
            get => SurvivorDatas.GameCounts + TraitorDatas.GameCounts;
        }
        [JsonIgnore]
        public int WinCounts
        {
            get => SurvivorDatas.WinCounts + SurvivorDatas.WinCounts;
        }
        [JsonIgnore]
        public int MaxSurvivalFrames
        {
            get => Math.Max(SurvivorDatas.MaxSurvivalFrames, TraitorDatas.MaxSurvivalFrames);
        }

        public PlayerData(int userID)
        {
            UserID = userID;
            SurvivorDatas = new DataDetail();
            TraitorDatas = new DataDetail();
            Boosts = new PlayerBoosts();
            UnlockedPrefixs = new List<int>();
            UnlockedTitles = new List<int>();
        }
    }
}
