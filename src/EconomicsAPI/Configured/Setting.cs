using Newtonsoft.Json;

namespace EconomicsAPI.Configured;

public class Setting
{
    [JsonProperty("货币名称")]
    public string CurrencyName = "魂力";

    [JsonProperty("货币转换率")]
    public double ConversionRate = 1f;

    [JsonProperty("保存时间间隔")]
    public int SaveTime = 30;

    [JsonProperty("显示收益")]
    public bool ShowAboveHead = true;

    [JsonProperty("禁用雕像")]
    public bool IgnoreStatue = false;

    [JsonProperty("死亡掉落率")]
    public float DeathDropRate = 0f;

    [JsonProperty("显示信息")]
    public bool StatusText = true;

    [JsonProperty("显示信息左移")]
    public int StatusTextShiftLeft = 60;

    [JsonProperty("显示信息下移")]
    public int StatusTextShiftDown = 0;

    [JsonProperty("查询提示")]
    public string QueryFormat = "[c/FFA500:{2}当前拥有{0}{1}个]";

    [JsonProperty("渐变颜色")]
    public List<string> GradientColor = new();
}