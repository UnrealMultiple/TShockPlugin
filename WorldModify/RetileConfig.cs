using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WorldModify;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace WorldModify
{
    internal class RetileConfig
    {
        public List<ReTileInfo> replace = new List<ReTileInfo>();

        public static RetileConfig Load(string path)
        {
            string text = "";
            if (File.Exists(path))
            {
                text = File.ReadAllText(path);
            }
            else
            {
                text = Utils.FromEmbeddedPath("retile.json");
                Utils.Save(path, text);
            }
            return JsonConvert.DeserializeObject<RetileConfig>(text, new JsonSerializerSettings
            {
                Error = delegate (object? sender, ErrorEventArgs error)
                {
                    error.ErrorContext.Handled = true;
                }
            });
        }
    }
}
