using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace EssentialsPlus;

[Config]
public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "EssentialsPlus";

    [LocalizedPropertyName(CultureType.Chinese, "Pvp禁用命令")]
    [LocalizedPropertyName(CultureType.English, "Disabled Commands in Pvp")]
    public string[] DisabledCommandsInPvp { get; set; } = Array.Empty<string>();

    [LocalizedPropertyName(CultureType.Chinese, "禁言是否判断IP")]
    [LocalizedPropertyName(CultureType.English, "MuteCheckIP")]
    public bool IsMuteCheckIP { get; set; } 

    [LocalizedPropertyName(CultureType.Chinese, "回退位置历史记录")]
    [LocalizedPropertyName(CultureType.English, "Back Position History")]
    public int BackPositionHistory { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "MySql主机")]
    [LocalizedPropertyName(CultureType.English, "MySQL Host")]
    public string? MySqlHost { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "MySql数据库名称")]
    [LocalizedPropertyName(CultureType.English, "MySQL Database Name")]
    public string? MySqlDbName { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "MySql用户名")]
    [LocalizedPropertyName(CultureType.English, "MySQL Username")]
    public string? MySqlUsername { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "MySql密码")]
    [LocalizedPropertyName(CultureType.English, "MySQL Password")]
    public string? MySqlPassword { get; set; }

    protected override void SetDefault()
    {
        this.DisabledCommandsInPvp = new[] { "eback" };
        this.IsMuteCheckIP = true;
        this.BackPositionHistory = 10;
        this.MySqlHost = "";
        this.MySqlDbName = "";
        this.MySqlUsername = "";
        this.MySqlPassword = "";
    }
}
