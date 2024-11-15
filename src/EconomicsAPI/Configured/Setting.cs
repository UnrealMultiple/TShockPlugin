using Newtonsoft.Json;

namespace EconomicsAPI.Configured;

public class Setting
{
    [JsonProperty("保存时间间隔")]
    public int SaveTime = 30;

    [JsonProperty("显示收益")]
    public bool ShowAboveHead = true;

    [JsonProperty("禁用雕像")]
    public bool IgnoreStatue = false;

    [JsonProperty("显示信息")]
    public bool StatusText = true;

    [JsonProperty("显示信息左移")]
    public int StatusTextShiftLeft = 60;

    [JsonProperty("显示信息下移")]
    public int StatusTextShiftDown = 0;

    [JsonProperty("渐变颜色")]
    public List<string> GradientColor = new();

    [JsonProperty("货币配置")]
    public List<CustomizeCurrency> CustomizeCurrencys = new();

    public CustomizeCurrency? GetCurrencyOption(string name)
    {
        return this.CustomizeCurrencys.Find(x => x.Name == name);
    }

    public bool HasCustomizeCurrency(string type)
    {
        return this.CustomizeCurrencys.Any(x => x.Name == type);
    }
}