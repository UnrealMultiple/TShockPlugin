using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TShockAPI;

namespace SurvivalCrisis
{
	using DataSet = Dictionary<int, PlayerData>;
	public class DataManager
	{
		private DataSet ExistedDatas;
		[JsonIgnore]
		public List<PlayerData> SortedDatas { get; }
		[JsonIgnore]
		public string SavePath { get; }

		public DataManager(string savePath)
		{
			SavePath = savePath;
			SortedDatas = new List<PlayerData>();
			LoadFromFile();
		}

		private void LoadFromFile()
		{
			if (File.Exists(SavePath))
			{
				var text = File.ReadAllText(SavePath);
				ExistedDatas = JsonConvert.DeserializeObject<DataSet>(text);
			}
			else
			{
				ExistedDatas = new DataSet();
				Save();
			}
			SortedDatas.AddRange(ExistedDatas.Values);
			UpdateRank();
		}

		public void Save()
		{
			var text = JsonConvert.SerializeObject(ExistedDatas, Formatting.Indented);
			File.WriteAllText(SavePath, text);
		}

		public GamePlayer GetPlayer(int index)
		{
			var player = TShock.Players[index];
			var data = GetPlayerData(player.Account.ID);
			data.Name = player.Account.Name;
			return new GamePlayer(index, data);
		}
		public PlayerData GetPlayerData(int userID)
		{
			if (!ExistedDatas.TryGetValue(userID, out PlayerData data))
			{
				data = new PlayerData(userID);
				ExistedDatas.Add(userID, data);
				UpdateRank();
			}
			return data;
		}

		public List<PlayerData> GetSurvivorRank()
		{
			var list = ExistedDatas.Where(p => p.Value.SurvivorDatas.TotalScore != 0).Select(p => p.Value);
			list = list.OrderBy(data => -data.SurvivorDatas.TotalScore);
			return list.ToList();
		}

		public List<PlayerData> GetTraitorRank()
		{
			var list = ExistedDatas.Where(p => p.Value.TraitorDatas.TotalScore != 0).Select(p => p.Value);
			list = list.OrderBy(data => -data.TraitorDatas.TotalScore);
			return list.ToList();
		}

		public void UpdateRank()
		{
			SortedDatas.Sort((left, right) =>
			{
				var subtraction = right.Score - left.Score;
				if (subtraction == 0)
				{
					return right.UserID - left.UserID;
				}
				return subtraction;
			});
		}
	}
}
