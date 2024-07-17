using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

[ApiVersion(2, 1)]
public class SessionSentinel : TerrariaPlugin
{
    public override string Name => "SessionSentinel";
    public override Version Version => new Version(1, 0, 0);
    public override string Author => "肝帝熙恩";
    public override string Description => "处理长时间不发送数据包的玩家";

    public SessionSentinel(Main game) : base(game)
    {
    }

    private readonly Dictionary<int, DateTime> _lastActivityTimes = new Dictionary<int, DateTime>();
    private const int TimeoutSeconds = 20; // 超时时间
    private int TimerCount = 0;

    public override void Initialize()
    {
        ServerApi.Hooks.NetGetData.Register(this, OnData);
        ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
        ServerApi.Hooks.ServerLeave.Register(this, OnLeave);

    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGetData.Deregister(this, OnData);
            ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
            ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);

        }
        base.Dispose(disposing);
    }

    private void OnLeave(LeaveEventArgs args)
    {
        _lastActivityTimes.Remove(args.Who);
    }

    private void OnJoin(JoinEventArgs args)
    {
        _lastActivityTimes[args.Who] = DateTime.UtcNow;
    }

    private void OnUpdate(EventArgs args)
    {
        TimerCount++;
        if (TimerCount % 60 == 0) // 每秒检测一次
        {
            DateTime now = DateTime.UtcNow;
            foreach (var kvp in _lastActivityTimes.ToList())
            {
                if ((now - kvp.Value).TotalSeconds > TimeoutSeconds)
                {
                    TSPlayer player = TShock.Players[kvp.Key];
                    if (player != null && player.Active)
                    {
                        player.Kick("你咋不动了。", true, true, null, false);
                    }
                    _lastActivityTimes.Remove(kvp.Key);
                }
            }
        }
    }

    private void OnData(GetDataEventArgs args)
    {
        if (args.MsgID == PacketTypes.PlayerUpdate)
        {
            int playerIndex = args.Msg.whoAmI;
            _lastActivityTimes[playerIndex] = DateTime.UtcNow;
        }
    }
}
