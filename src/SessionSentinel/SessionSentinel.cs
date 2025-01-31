using Terraria;
using TerrariaApi.Server;
using TShockAPI;

[ApiVersion(2, 1)]
public class SessionSentinel : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 3);
    public override string Author => "肝帝熙恩";
    public override string Description => GetString("处理长时间不发送数据包的玩家");

    public SessionSentinel(Main game) : base(game)
    {
    }

    private readonly Dictionary<int, DateTime> _lastActivityTimes = new Dictionary<int, DateTime>();
    private const int TimeoutSeconds = 20; // 超时时间
    private int TimerCount = 0;

    public override void Initialize()
    {
        ServerApi.Hooks.NetGetData.Register(this, this.OnData);
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        ServerApi.Hooks.ServerJoin.Register(this, this.OnJoin);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);

    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGetData.Deregister(this, this.OnData);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnJoin);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);

        }
        base.Dispose(disposing);
    }

    private void OnLeave(LeaveEventArgs args)
    {
        this._lastActivityTimes.Remove(args.Who);
    }

    private void OnJoin(JoinEventArgs args)
    {
        this._lastActivityTimes[args.Who] = DateTime.UtcNow;
    }

    private void OnUpdate(EventArgs args)
    {
        this.TimerCount++;
        if (this.TimerCount % 60 == 0) // 每秒检测一次
        {
            var now = DateTime.UtcNow;
            foreach (var kvp in this._lastActivityTimes.ToList())
            {
                if ((now - kvp.Value).TotalSeconds > TimeoutSeconds)
                {
                    var player = TShock.Players[kvp.Key];
                    if (player != null && player.Active)
                    {
                        player.Kick(GetString("你咋不动了。"), true, true, null, false);
                    }
                    this._lastActivityTimes.Remove(kvp.Key);
                }
            }
        }
    }

    private void OnData(GetDataEventArgs args)
    {
        if (args.MsgID == PacketTypes.PlayerUpdate)
        {
            var playerIndex = args.Msg.whoAmI;
            this._lastActivityTimes[playerIndex] = DateTime.UtcNow;
        }
    }
}