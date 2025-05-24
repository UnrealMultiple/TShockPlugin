using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace AutoAirItem;

[Config]
internal class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "AutoAirItem";

    #region 实例变量

    [LocalizedPropertyName(CultureType.Chinese, "插件开关")]
    [LocalizedPropertyName(CultureType.English, "Enabled")]
    public bool Enabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "保存数据")]
    [LocalizedPropertyName(CultureType.English, "SaveDatabase")]
    public bool SaveDatabase { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "排除垃圾表")]
    [LocalizedPropertyName(CultureType.English, "exclude")]
    public int[] Exclude = new int[] { 71, 72, 73, 74 };
    #endregion

}