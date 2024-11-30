using LazyAPI.ConfigFiles;

namespace AutoTeam;

public class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "AutoTeam";

    [LocalizedPropertyName(CultureType.Chinese, "开启插件")]
    [LocalizedPropertyName(CultureType.English, "enable")]
    public bool Enabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese,  "组对应的队伍")]
    [LocalizedPropertyName(CultureType.Chinese, "groupTemp")]
    public Dictionary<string, string> GroupTeamMap { get; set; } = new Dictionary<string, string>();


    public string GetTeamForGroup(string groupName)
    {
        return this.GroupTeamMap.TryGetValue(groupName, out var team) ? team : GetString("无队伍");
    }
}