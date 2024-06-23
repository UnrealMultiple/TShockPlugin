using Newtonsoft.Json;

namespace WorldModify
{
    internal class RetileTool
    {
        private static RetileConfig _config;

        public static string SaveFile;

        public static RetileConfig Con => _config;

        public static bool FirstCreate()
        {
            if (File.Exists(SaveFile))
            {
                return false;
            }
            string content = Utils.FromEmbeddedPath("retile.json");
            Utils.Save(SaveFile, content);
            return true;
        }

        public static void Init()
        {
            Reload();
        }

        public static void Reload()
        {
            _config = RetileConfig.Load(SaveFile);
        }

        public static void Save()
        {
            Utils.Save(SaveFile, JsonConvert.SerializeObject(_config, (Formatting)1));
        }
    }
}
