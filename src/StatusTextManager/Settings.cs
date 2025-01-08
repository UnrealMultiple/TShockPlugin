using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StatusTextManager.SettingsModel;
using StatusTextManager.Utils;

namespace StatusTextManager;

public class Settings
{
    [JsonConverter(typeof(StringEnumConverter))]
    public LogLevel LogLevel = LogLevel.INFO;

    public List<IStatusTextSetting> StatusTextSettings = new();
}