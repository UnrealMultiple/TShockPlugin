using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoBroadcast;

[ApiVersion(2, 1)]
public class AutoBroadcast : TerrariaPlugin
{
    public override string Name => "AutoBroadcast";
    public override string Author => "Scavenger";
    public override string Description => "自动广播插件";
    public override Version Version => new Version(1, 0, 6);
    public string ConfigPath => Path.Combine(TShock.SavePath, "AutoBroadcastConfig.json");
    public ABConfig Config = new ABConfig();
    public DateTime LastCheck = DateTime.UtcNow;

    public AutoBroadcast(Main Game) : base(Game) { }

    public override void Initialize()
    {
        ServerApi.Hooks.GameInitialize.Register(this, this.OnInitialize);
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        ServerApi.Hooks.ServerChat.Register(this, this.OnChat);
    }

    protected override void Dispose(bool Disposing)
    {
        if (Disposing)
        {
            ServerApi.Hooks.GameInitialize.Deregister(this, this.OnInitialize);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
            ServerApi.Hooks.ServerChat.Deregister(this, this.OnChat);
        }
        base.Dispose(Disposing);
    }

    public void OnInitialize(EventArgs args)
    {
        this.autobc();
        TShockAPI.Hooks.GeneralHooks.ReloadEvent += (_) =>
        {
            TSPlayer.Server.SendSuccessMessage(GetString("自定义广播配置已重读!"));
            this.autobc();
        };
    }

    public void autobc()
    {
        try
        {
            this.Config = ABConfig.Read(this.ConfigPath).Write(this.ConfigPath);
        }
        catch (Exception ex)
        {
            this.Config = new ABConfig();
            TShock.Log.Error(GetString("[AutoBroadcast]配置读取发生错误!\n{0}").SFormat(ex.ToString()));
        }
    }

    #region Chat
    public void OnChat(ServerChatEventArgs args)
    {
        if (TShock.Players[args.Who] == null)
        {
            return;
        }
        var Groups = new string[0];
        var Messages = new string[0];
        var Colour = new float[0];
        var PlayerGroup = TShock.Players[args.Who].Group.Name;

        lock (this.Config.Broadcasts)
        {
            foreach (var broadcast in this.Config.Broadcasts)
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
            lock (this.Config.Broadcasts)
            {
                NumBroadcasts = this.Config.Broadcasts.Length;
            }

            for (var i = 0; i < NumBroadcasts; i++)
            {
                var Groups = new string[0];
                var Messages = new string[0];
                var Colour = new float[0];

                lock (this.Config.Broadcasts)
                {
                    if (this.Config.Broadcasts[i] == null || !this.Config.Broadcasts[i].Enabled || this.Config.Broadcasts[i].Interval < 1)
                    {
                        continue;
                    }
                    if (this.Config.Broadcasts[i].StartDelay > 0)
                    {
                        this.Config.Broadcasts[i].StartDelay--;
                        continue;
                    }
                    this.Config.Broadcasts[i].StartDelay = this.Config.Broadcasts[i].Interval;// Start Delay used as Interval Countdown
                    Groups = this.Config.Broadcasts[i].Groups;
                    Messages = this.Config.Broadcasts[i].Messages;
                    Colour = this.Config.Broadcasts[i].ColorRGB;
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