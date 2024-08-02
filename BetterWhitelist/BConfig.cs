using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BetterWhitelist
{
	public class BConfig
	{
        [JsonProperty("插件开关",Order = 0)]
        public bool Disabled { get; set; }
        [JsonProperty("连接时不在白名单提示", Order = 1)]
        public string NotInWhiteList { get; set; } = "你不在服务器白名单中！";
        [JsonProperty("白名单玩家", Order = 3)]
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
