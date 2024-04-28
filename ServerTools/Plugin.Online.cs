using Rests;
using ServerTools.DB;
using TerrariaApi.Server;
using TShockAPI;

namespace ServerTools;

public partial class Plugin
{
    public static readonly List<TSPlayer> ActivePlayers = new();

    public static readonly PlayerOnline PlayerOnlines = new();

    private object Queryduration(RestRequestArgs args)
    {
        var data = PlayerOnlines.OrderByDescending(x => x.Value).Select(x => new { name = x.Key, duration = x.Value});
        return new RestObject() { { "response", "查询成功" }, { "data", data } };
    }

    private void _OnLeave(LeaveEventArgs args)
    {
        ActivePlayers.Remove(TShock.Players[args.Who]);
        PlayerOnlines.UpdateAll();
    }

    private void OnGreet(GreetPlayerEventArgs args)
    {
        var ply = TShock.Players[args.Who];
        if(ply != null)
            ActivePlayers.Add(ply);
    }

    private void OnUpdatePlayerOnline(EventArgs args)
    {
        ActivePlayers.ForEach(p =>
        {
            if (p != null && p.Active)
            {
                PlayerOnlines[p.Name] += 1;
            }
        });
    }
}
