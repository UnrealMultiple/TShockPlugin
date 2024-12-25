using Rests;
using ServerTools.DB;
using TShockAPI;

namespace ServerTools;

public partial class Plugin
{

    public static readonly List<TSPlayer> Deads = new();

    private void KillMe(object? sender, GetDataHandlers.KillMeEventArgs e)
    {
        if (e.Player == null)
        {
            return;
        }

        DB.PlayerDeath.Add(e.Player.Name);
        Deads.Add(e.Player);
    }

    private object DeadRank(RestRequestArgs args)
    {
        var data = DB.PlayerDeath.GetDeathRank()
            .Select(x => new { x.Name , x.Count  })
            .ToList();
        return new RestObject("200")
        {
            ["response"] = "获取成功",
            ["data"] = data
        };
    }
}