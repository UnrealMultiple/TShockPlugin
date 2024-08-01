using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BetterWhitelist
{
	public class BConfig
	{
        [JsonProperty("插件开关")]
        public bool Disabled { get; set; }
        [JsonProperty("白名单玩家")]
        public List<string> WhitePlayers = new List<string>();
        public static BConfig Load(string path)
		{
			BConfig result;
            if (File.Exists(path))
            {
				result = JsonConvert.DeserializeObject<BConfig>(File.ReadAllText(path));
			}
			else
			{
				result = new BConfig
				{
					Disabled = false
				};
			}
			return result;
		}

	}
}
