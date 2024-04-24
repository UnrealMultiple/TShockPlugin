using Newtonsoft.Json;
using TShockAPI;
using MiniGamesAPI;


//命名空间，想其他项目调用这个项目必须统一命名空间
namespace MainPlugin
{
    public static class ConfigUtils
    {
        public static readonly string configDir = TShock.SavePath + "/BuildMaster";

        public static readonly string roomsPath = configDir + "/rooms.json";

        public static readonly string evaluatePath = configDir + "/eva.json";

        public static readonly string defaultPath = configDir + "/default.json";

        public static readonly string configPath = configDir + "/config.json";

        public static List<BuildRoom> rooms = new List<BuildRoom>();

        public static List<BuildPlayer> players = new List<BuildPlayer>();

        public static MiniPack defaultPack = new MiniPack("基础套", 2);

        public static MiniPack evaluatePack = new MiniPack("评分套", 3);

        public static BuildConfig config = new BuildConfig();

        public static void LoadConfig()
        {
            if (Directory.Exists(configDir))
            {
                if (File.Exists(roomsPath))
                {
                    rooms = JsonConvert.DeserializeObject<List<BuildRoom>>(File.ReadAllText(roomsPath));
                }
                else
                {
                    File.WriteAllText(roomsPath, JsonConvert.SerializeObject((object)rooms, (Formatting)1));
                }
                if (File.Exists(evaluatePath))
                {
                    evaluatePack = JsonConvert.DeserializeObject<MiniPack>(File.ReadAllText(evaluatePath));
                }
                else
                {
                    File.WriteAllText(evaluatePath, JsonConvert.SerializeObject((object)evaluatePack, (Formatting)1));
                }
                if (File.Exists(defaultPath))
                {
                    defaultPack = JsonConvert.DeserializeObject<MiniPack>(File.ReadAllText(defaultPath));
                }
                else
                {
                    File.WriteAllText(defaultPath, JsonConvert.SerializeObject((object)defaultPack, (Formatting)1));
                }
                if (File.Exists(configPath))
                {
                    config = JsonConvert.DeserializeObject<BuildConfig>(File.ReadAllText(configPath));
                }
                else
                {
                    File.WriteAllText(configPath, JsonConvert.SerializeObject((object)config, (Formatting)1));
                }
            }
            else
            {
                Directory.CreateDirectory(configDir);
                File.WriteAllText(roomsPath, JsonConvert.SerializeObject((object)rooms, (Formatting)1));
                File.WriteAllText(evaluatePath, JsonConvert.SerializeObject((object)evaluatePack, (Formatting)1));
                File.WriteAllText(defaultPath, JsonConvert.SerializeObject((object)defaultPack, (Formatting)1));
                File.WriteAllText(configPath, JsonConvert.SerializeObject((object)config, (Formatting)1));
            }
        }

        public static void UpdateRooms(int id)
        {
            List<BuildRoom> list = JsonConvert.DeserializeObject<List<BuildRoom>>(File.ReadAllText(roomsPath));
            int index = list.FindIndex((BuildRoom r) => ((MiniRoom)r).ID == id);
            list[index] = rooms.Find((BuildRoom r) => ((MiniRoom)r).ID == id);
            File.WriteAllText(roomsPath, JsonConvert.SerializeObject((object)rooms, (Formatting)1));
        }

        public static void AddRoom(BuildRoom room)
        {
            JsonConvert.DeserializeObject<List<BuildRoom>>(File.ReadAllText(roomsPath)).Add(room);
            File.WriteAllText(roomsPath, JsonConvert.SerializeObject((object)rooms, (Formatting)1));
        }

        public static void RemoveRoom(int id)
        {
            JsonConvert.DeserializeObject<List<BuildRoom>>(File.ReadAllText(roomsPath)).RemoveAll((BuildRoom r) => ((MiniRoom)r).ID == id);
            File.WriteAllText(roomsPath, JsonConvert.SerializeObject((object)rooms, (Formatting)1));
        }

        public static BuildRoom GetRoomByID(int id)
        {
            return rooms.Find((BuildRoom r) => ((MiniRoom)r).ID == id);
        }

        public static BuildRoom GetRoomByIDFromLocal(int id)
        {
            return JsonConvert.DeserializeObject<List<BuildRoom>>(File.ReadAllText(roomsPath)).Find((BuildRoom r) => ((MiniRoom)r).ID == id);
        }

        public static BuildPlayer GetPlayerByName(string name)
        {
            return players.Find((BuildPlayer p) => ((MiniPlayer)p).Name == name);
        }

        public static void UpdatePack()
        {
            File.WriteAllText(defaultPath, JsonConvert.SerializeObject((object)defaultPack, (Formatting)1));
            File.WriteAllText(evaluatePath, JsonConvert.SerializeObject((object)evaluatePack, (Formatting)1));
        }

        public static void ReloadConfig()
        {
            config = JsonConvert.DeserializeObject<BuildConfig>(File.ReadAllText(configPath));
        }
    }
}