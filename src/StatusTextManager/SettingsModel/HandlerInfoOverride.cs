using Newtonsoft.Json;

namespace StatusTextManager.SettingsModel;

public class HandlerInfoOverride : IStatusTextSetting
{
    [JsonProperty] public static string TypeName => nameof(HandlerInfoOverride);

    public string PluginName { get; set; } = "";
    public bool Enabled { get; set; }
    public bool OverrideInterval { get; set; }
    public ulong UpdateInterval { get; set; }

    public void ProcessHandlers(List<StatusTextUpdateHandlerItem> handlers, List<IStatusTextUpdateHandler> processedHandlers, int settingsIdx)
    {
        var handlerMatched = handlers.Find(h => h.AssemblyName == this.PluginName);
        if (handlerMatched == null)
        {
            return;
        }

        handlers.Remove(handlerMatched);
        if (!this.Enabled)
        {
            return;
        }

        if (this.OverrideInterval)
        {
            handlerMatched.UpdateInterval = this.UpdateInterval;
        }

        processedHandlers.Add(handlerMatched);
    }
}