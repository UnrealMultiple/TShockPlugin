using Newtonsoft.Json;
using StatusTextManager.Utils.Attrs;
using StatusTextManager.Utils.JsonConverters;

namespace StatusTextManager.SettingsModel;

[JsonConverter(typeof(InterfaceConcreteConverter))]
[Implements(typeof(StaticText), typeof(HandlerInfoOverride), typeof(DynamicText))]
public interface IStatusTextSetting
{
    void ProcessHandlers(List<StatusTextUpdateHandlerItem> handlers, List<IStatusTextUpdateHandler> processedHandlers, int settingsIdx);
}