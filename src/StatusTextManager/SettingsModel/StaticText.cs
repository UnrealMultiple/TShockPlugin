using Newtonsoft.Json;
using TShockAPI;

namespace StatusTextManager.SettingsModel;

public class StaticText : IStatusTextSetting, IStatusTextUpdateHandler
{
    [JsonProperty] public static string TypeName => nameof(StaticText);

    public string Text { get; set; } = "";

    public void ProcessHandlers(List<StatusTextUpdateHandlerItem> handlers, List<IStatusTextUpdateHandler> processedHandlers, int settingsIdx)
    {
        processedHandlers.Add(this);
    }


    public bool Invoke(TSPlayer player, bool forceUpdate = false)
    {
        return forceUpdate;
    }

    public string GetPlayerStatusText(TSPlayer player)
    {
        return this.Text;
    }
}