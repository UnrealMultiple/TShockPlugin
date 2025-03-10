﻿using Rests;
using ServerTools.DB;
using TerrariaApi.Server;
using TShockAPI;

namespace ServerTools;

public partial class Plugin
{
    public static readonly List<TSPlayer> ActivePlayers = new();

    private object Queryduration(RestRequestArgs args)
    {
        var data = PlayerOnline.GetOnlineRank().Select(x => new { name = x.Name, duration = x.Duration });
        return new RestObject() { { "response", "查询成功" }, { "data", data } };
    }

    private void OnLeaveV2(LeaveEventArgs args)
    {
        ActivePlayers.Remove(TShock.Players[args.Who]);
    }

    private void OnGreet(GreetPlayerEventArgs args)
    {
        var ply = TShock.Players[args.Who];
        if (ply != null)
        {
            ActivePlayers.Add(ply);
            ply.RespawnTimer = 0;
        }

    }

    private void OnUpdatePlayerOnline(EventArgs args)
    {
        ActivePlayers.ForEach(p =>
        {
            if (p != null && p.Active)
            {
                PlayerOnline.Add(p.Name, 1);
            }
        });
    }
}