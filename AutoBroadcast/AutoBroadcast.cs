using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoBroadcast;

[ApiVersion(2, 1)]
public class AutoBroadcast : TerrariaPlugin
{

    public override string Name { get { return "AutoBroadcast"; } }
    public override string Author { get { return "Scavenger"; } }
    public override string Description { get { return "自动广播插件"; } }
    public override Version Version { get { return Assembly.GetExecutingAssembly().GetName().Version!; } }

    public string ConfigPath { get { return Path.Combine(TShock.SavePath, "AutoBroadcastConfig.json"); } }
    public ABConfig Config = new ABConfig();
    public DateTime LastCheck = DateTime.UtcNow;

    public AutoBroadcast(Main Game) : base(Game) { }

    public override void Initialize()
    {
        ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
        ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        ServerApi.Hooks.ServerChat.Register(this, OnChat);
    }

    protected override void Dispose(bool Disposing)
    {
        if (Disposing)
        {
            ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
        }
        base.Dispose(Disposing);
    }

    public void OnInitialize(EventArgs args)
    {
        autobc();
        TShockAPI.Hooks.GeneralHooks.ReloadEvent += (_) =>
        {
            TSPlayer.Server.SendSuccessMessage("自定广播配置已重读!");
            autobc();
        };
    }

    public void autobc()
    {
        try
        {
            Config = ABConfig.Read(ConfigPath).Write(ConfigPath);
        }
        catch (Exception ex)
        {
            Config = new ABConfig();
            TShock.Log.Error("[AutoBroadcast]配置读取发生错误!\n{0}".SFormat(ex.ToString()));
        }
    }

    #region Chat
    public void OnChat(ServerChatEventArgs args)
    {
        if (TShock.Players[args.Who] == null)
        {
            return;
        }
        string[] Groups = new string[0];
        string[] Messages = new string[0];
        float[] Colour = new float[0];
        var PlayerGroup = TShock.Players[args.Who].Group.Name;

        lock (Config.Broadcasts)
            foreach (var broadcast in Config.Broadcasts)
            {
                if (broadcast == null || !broadcast.Enabled ||
                   broadcast.TriggerToWholeGroup && !broadcast.Groups.Contains(PlayerGroup))
                {
                    continue;
                }

                foreach (string Word in broadcast.TriggerWords)
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
        if ((DateTime.UtcNow - LastCheck).TotalSeconds >= 1)
        {
            LastCheck = DateTime.UtcNow;
            int NumBroadcasts = 0;
            lock (Config.Broadcasts)
                NumBroadcasts = Config.Broadcasts.Length;
            for (int i = 0; i < NumBroadcasts; i++)
            {
                string[] Groups = new string[0];
                string[] Messages = new string[0];
                float[] Colour = new float[0];

                lock (Config.Broadcasts)
                {
                    if (Config.Broadcasts[i] == null || !Config.Broadcasts[i].Enabled || Config.Broadcasts[i].Interval < 1)
                    {
                        continue;
                    }
                    if (Config.Broadcasts[i].StartDelay > 0)
                    {
                        Config.Broadcasts[i].StartDelay--;
                        continue;
                    }
                    Config.Broadcasts[i].StartDelay = Config.Broadcasts[i].Interval;// Start Delay used as Interval Countdown
                    Groups = Config.Broadcasts[i].Groups;
                    Messages = Config.Broadcasts[i].Messages;
                    Colour = Config.Broadcasts[i].ColorRGB;
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
        foreach (string Line in Messages)
        {
            if (Line.StartsWith("/"))
            {
                Commands.HandleCommand(TSPlayer.Server, Line);
            }
            else
            {
                lock (TShock.Players)
                    foreach (var player in TShock.Players)
                    {
                        if (player != null && Groups.Contains(player.Group.Name))
                        {
                            player.SendMessage(Line, (byte)Colour[0], (byte)Colour[1], (byte)Colour[2]);
                        }
                    }
            }
        }
    }
    public static void BroadcastToAll(string[] Messages, float[] Colour)
    {
        foreach (string Line in Messages)
        {
            if (Line.StartsWith("/"))
            {
                Commands.HandleCommand(TSPlayer.Server, Line);
            }
            else
            {
                TSPlayer.All.SendMessage(Line, (byte)Colour[0], (byte)Colour[1], (byte)Colour[2]);
            }
        }
    }
    public static void BroadcastToPlayer(int plr, string[] Messages, float[] Colour)
    {
        foreach (string Line in Messages)
        {
            if (Line.StartsWith("/"))
            {
                Commands.HandleCommand(TSPlayer.Server, Line);
            }
            else lock (TShock.Players)
                {
                    TShock.Players[plr].SendMessage(Line, (byte)Colour[0], (byte)Colour[1], (byte)Colour[2]);
                }
        }
    }
}