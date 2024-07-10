using Newtonsoft.Json;

namespace CaiBotPlugin
{
    public class Config
    {
        public const string Path = "tshock/CaiBot.json";

        public static Config config = new Config();

        public void Write(string path = Path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                this.Write(fileStream);
            }
        }

        public void Write(Stream stream)
        {
            string value = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (StreamWriter streamWriter = new StreamWriter(stream))
            {
                streamWriter.Write(value);
            }
        }

        public static Config Read(string path = Path)
        {
            bool flag = !File.Exists(path);
            Config result;
            if (flag)
            {
                result = new Config();
                result.Write(path);
            }
            else
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    result = Config.Read(fileStream);
                }
            }

            config = result;
            return result;
        }

        public static Config Read(Stream stream)
        {
            Config result;
            using (StreamReader streamReader = new StreamReader(stream))
            {
                result = JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd());
            }

            return result;
        }

        [JsonProperty("密钥")] public string Token = "";
        [JsonProperty("白名单开关")] public bool WhiteList = true;
        [JsonProperty("白名单拦截提示的群号")] public long GroupNumber = 0;
    }
}