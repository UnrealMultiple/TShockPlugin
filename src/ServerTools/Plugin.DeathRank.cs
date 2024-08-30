using Rests;
using ServerTools.DB;
using TShockAPI;

namespace ServerTools;

public partial class Plugin
{
    public static readonly PlayerDeath PlayerDeathRank = new();

    public static readonly List<TSPlayer> Deads = new();
    public void InitDeathRank()
    {

    }

    private void KillMe(object? sender, GetDataHandlers.KillMeEventArgs e)
    {
        if (e.Player == null)
        {
            return;
        }

        PlayerDeathRank.Add(e.Player.Name);
        Deads.Add(e.Player);
    }

    private object DeadRank(RestRequestArgs args)
    {
        var data = PlayerDeathRank
            .OrderByDescending(x => x.Value)
            .Select(x => new { Name = x.Key, Count = x.Value })
            .ToList();
        return new RestObject("200")
        {
            ["response"] = "获取成功",
            ["data"] = data
        };
    }
}