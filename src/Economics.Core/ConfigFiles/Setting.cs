using Economics.Core.ConfigFiles;
using Newtonsoft.Json;

namespace Economics.Core.ConfigFiles;

public class Setting : JsonConfigBase<Setting>
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
    public List<string> GradientColor = [];

    [JsonProperty("货币配置")]
    public List<CustomizeCurrency> CustomizeCurrencys = [];

    protected override string Filename => "Economics.json";

    protected override void SetDefault()
    {
        this.GradientColor =
        [
            "[c/00ffbf:{0}]",
            "[c/1aecb8:{0}]",
            "[c/33d9b1:{0}]",
            "[c/A6D5EA:{0}]",
            "[c/A6BBEA:{0}]",
            "[c/B7A6EA:{0}]",
            "[c/A6EAB3:{0}]",
            "[c/D5F0AA:{0}]",
            "[c/F5F7AF:{0}]",
            "[c/F8ECB0:{0}]",
            "[c/F8DEB0:{0}]",
            "[c/F8D0B0:{0}]",
            "[c/F8B6B0:{0}]",
            "[c/EFA9C6:{0}]",
            "[c/00ffbf:{0}]",
            "[c/1aecb8:{0}]"
        ];
    }

    public CustomizeCurrency? GetCurrencyOption(string name)
    {
        return this.CustomizeCurrencys.Find(x => x.Name == name);
    }

    public bool HasCustomizeCurrency(string type)
    {
        return this.CustomizeCurrencys.Any(x => x.Name == type);
    }
}