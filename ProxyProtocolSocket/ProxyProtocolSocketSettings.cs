using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ProxyProtocolSocket.Utils;

namespace ProxyProtocolSocket
{
    public class ProxyProtocolSocketSettings
    {
        [JsonProperty("log_level")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LogLevel LogLevel = LogLevel.Info;

        [JsonProperty("timeout")]
        public int TimeOut = 1000;
    }
}
