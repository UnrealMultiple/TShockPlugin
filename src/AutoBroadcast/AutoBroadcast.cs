using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoBroadcast;

[ApiVersion(2, 1)]
public class AutoBroadcast : LazyPlugin
{
    public override string Name => "AutoBroadcast";
    public override string Author => "Scavenger";
    public override string Description => "自动广播插件";
    public override Version Version => new Version(1, 0, 8);

    public DateTime LastCheck = DateTime.UtcNow;

    public AutoBroadcast(Main Game) : base(Game) { }

    public override void Initialize()
    {
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        ServerApi.Hooks.ServerChat.Register(this, this.OnChat);
    }

    protected override void Dispose(bool Disposing)
    {
        if (Disposing)
        {
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
            ServerApi.Hooks.ServerChat.Deregister(this, this.OnChat);
        }
        base.Dispose(Disposing);
    }


  
    #region Chat
    public void OnChat(ServerChatEventArgs args)
    {
        if (TShock.Players[args.Who] == null)
        {
            return;
        }
        var Groups = Array.Empty<string>();
        var Messages = Array.Empty<string>();
        var Colour = Array.Empty<float>();
        var PlayerGroup = TShock.Players[args.Who].Group.Name;

        lock (ABConfig.Instance.Broadcasts)
        {
            foreach (var broadcast in ABConfig.Instance.Broadcasts)
            {
                if (broadcast == null || !broadcast.Enabled ||
                   broadcast.TriggerToWholeGroup && !broadcast.Groups.Contains(PlayerGroup))
                {
                    continue;
                }

                foreach (var Word in broadcast.TriggerWords)
                {
                    if (args.Text.Contains(Word))
                    {
                        if (broadcast.TriggerToWholeGroup && broadcast.Groups.Length > 0)
                        {
                            Groups = broadcast.Groups;
                        }
                        Messages = broadcast.Messages;
                        Colour = broadcast.ColorRGB;
                        break;
                    }
                }
            }
        }

        if (Groups.Length > 0)
        {
            BroadcastToGroups(Groups, Messages, Colour);
        }
        else
        {
            BroadcastToPlayer(args.Who, Messages, Colour);
        }
    }
    #endregion

    #region Update
    public void OnUpdate(EventArgs args)
    {
        if ((DateTime.UtcNow - this.LastCheck).TotalSeconds >= 1)
        {
            this.LastCheck = DateTime.UtcNow;
            var NumBroadcasts = 0;
            lock (ABConfig.Instance.Broadcasts)
            {
                NumBroadcasts = ABConfig.Instance.Broadcasts.Length;
            }

            for (var i = 0; i < NumBroadcasts; i++)
            {
                var Groups = Array.Empty<string>();
                var Messages = Array.Empty<string>();
                var Colour = Array.Empty<float>();

                lock (ABConfig.Instance.Broadcasts)
                {
                    if (ABConfig.Instance.Broadcasts[i] == null || ABConfig.Instance.Broadcasts[i].Enabled || ABConfig.Instance.Broadcasts[i].Interval < 1)
                    {
                        continue;
                    }
                    if (ABConfig.Instance.Broadcasts[i].StartDelay > 0)
                    {
                        ABConfig.Instance.Broadcasts[i].StartDelay--;
                        continue;
                    }
                    ABConfig.Instance.Broadcasts[i].StartDelay = ABConfig.Instance.Broadcasts[i].Interval;// Start Delay used as Interval Countdown
                    Groups = ABConfig.Instance.Broadcasts[i].Groups;
                    Messages = ABConfig.Instance.Broadcasts[i].Messages;
                    Colour = ABConfig.Instance.Broadcasts[i].ColorRGB;
                }

                if (Groups.Length > 0)
                {
                    BroadcastToGroups(Groups, Messages, Colour);
                }
                else
                {
                    BroadcastToAll(Messages, Colour);
                }
            }
        }
    }
    #endregion

    public static void BroadcastToGroups(string[] Groups, string[] Messages, float[] Colour)
    {
        foreach (var Line in Messages)
        {
            if (Line.StartsWith("/"))
            {
                Commands.HandleCommand(TSPlayer.Server, Line);
            }
            else
            {
                lock (TShock.Players)
                {
                    foreach (var player in TShock.Players)
                    {
                        if (player != null && Groups.Contains(player.Group.Name))
                        {
                            player.SendMessage(Line, (byte) Colour[0], (byte) Colour[1], (byte) Colour[2]);
                        }
                    }
                }
            }
        }
    }
    public static void BroadcastToAll(string[] Messages, float[] Colour)
    {
        foreach (var Line in Messages)
        {
            if (Line.StartsWith("/"))
            {
                Commands.HandleCommand(TSPlayer.Server, Line);
            }
            else
            {
                TSPlayer.All.SendMessage(Line, (byte) Colour[0], (byte) Colour[1], (byte) Colour[2]);
            }
        }
    }
    public static void BroadcastToPlayer(int plr, string[] Messages, float[] Colour)
    {
        foreach (var Line in Messages)
        {
            if (Line.StartsWith("/"))
            {
                Commands.HandleCommand(TSPlayer.Server, Line);
            }
            else
            {
                lock (TShock.Players)
                {
                    TShock.Players[plr].SendMessage(Line, (byte) Colour[0], (byte) Colour[1], (byte) Colour[2]);
                }
            }
        }
    }
}