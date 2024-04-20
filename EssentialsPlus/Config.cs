using System.IO;
using Newtonsoft.Json;

namespace EssentialsPlus
{
	public class Config
	{
		public string[] DisabledCommandsInPvp = new string[]
		{
			"back"
		};

		public int BackPositionHistory = 10;

		public string MySqlHost = "";
		public string MySqlDbName = "";
		public string MySqlUsername = "";
		public string MySqlPassword = "";

		public void Write(string path)
		{
			File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
		}

		public static Config Read(string path)
		{
			return File.Exists(path) ? JsonConvert.DeserializeObject<Config>(File.ReadAllText(path)) : new Config();
		}
	}
}
