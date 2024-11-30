using LazyAPI;
using LazyAPI.ConfigFiles;

namespace AutoPluginManager;

[Config]
public class Config : JsonConfigBase<Config>
{
    [LocalizedPropertyName(CultureType.Chinese, "允许自动更新插件")]
    [LocalizedPropertyName(CultureType.English, "AutoUpdate")]
    public bool AutoUpdate = false;

    [LocalizedPropertyName(CultureType.Chinese, "使用Github源")]
    [LocalizedPropertyName(CultureType.English, "UseGithubDownload")]
    public bool UseGithubSource = false;

    [LocalizedPropertyName(CultureType.Chinese, "使用自定义源")]
    [LocalizedPropertyName(CultureType.English, "UseCustomDownloadUrl")]
    public bool UseCustomSource = false;

    [LocalizedPropertyName(CultureType.Chinese, "自定义源清单地址")]
    [LocalizedPropertyName(CultureType.English, "CustomSourceManifestUrl")]
    public string CustomSourceManifestUrl = string.Empty;

    [LocalizedPropertyName(CultureType.Chinese, "自定义源压缩文件地址")]
    [LocalizedPropertyName(CultureType.English, "CustomSourceArchiveUrl")]
    public string CustomSourceArchiveUrl = string.Empty;

    [LocalizedPropertyName(CultureType.Chinese, "插件排除列表")]
    [LocalizedPropertyName(CultureType.English, "UpdateBlackList")]
    public List<string> UpdateBlackList = new ();

    [LocalizedPropertyName(CultureType.Chinese, "热重载升级插件")]
    [LocalizedPropertyName(CultureType.English, "HotReloadPlugin")]
    public bool HotReloadPlugin = true;

    [LocalizedPropertyName(CultureType.Chinese, "热重载出错时继续")]
    [LocalizedPropertyName(CultureType.English, "ConinueHotReloadWhenError")]
    public bool ConinueHotReloadWhenError = true;

    protected override string Filename => "AutoPluginManager";
}