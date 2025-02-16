using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace AutoTeam;

[Config]
public class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "AutoTeam";

    [LocalizedPropertyName(CultureType.Chinese, "开启插件")]
    [LocalizedPropertyName(CultureType.English, "Enable")]
    public bool Enabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "组对应的队伍")]
    [LocalizedPropertyName(CultureType.English, "GroupTemp")]
    public Dictionary<string, string> GroupTeamMap { get; set; } = new Dictionary<string, string>();


    public string GetTeamForGroup(string groupName)
    {
        return this.GroupTeamMap.TryGetValue(groupName, out var team) ? team : GetString("未配置");
    }

    protected override void SetDefault()
    {
        this.GroupTeamMap = new Dictionary<string, string>
        {
            {"guest", "pink"},
            {"default", "蓝队"},
            {"owner", "红队"}, 
            {"admin", "green"},
            {"vip", "none"}
        };
    }
}