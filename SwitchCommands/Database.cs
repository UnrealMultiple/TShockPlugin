using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
//using PlaceholderAPI;

namespace SwitchCommands {
    public class Database {
        public static string databasePath = Path.Combine(TShock.SavePath, "开关配置表.json");

        public Dictionary<string, CommandInfo> switchCommandList = new Dictionary<string, CommandInfo>();

        public void Write(string path) {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static Database Read(string path) {
            if (!File.Exists(path))
                return new Database();
            return JsonConvert.DeserializeObject<Database>(File.ReadAllText(path));
        }
    }
}
