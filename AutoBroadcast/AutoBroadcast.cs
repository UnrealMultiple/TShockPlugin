using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using static Org.BouncyCastle.Math.EC.ECCurve;

[ApiVersion(2, 1)]
public class AutoBroadcast : TerrariaPlugin
{
    public ABConfig Config = new ABConfig();

    public static bool ULock = false;

    public override string Name => "自动广播";

    public override string Author => "Zaicon,GK 小改良，肝帝熙恩更新至1449";

    public override string Description => "每隔N秒自动广播一条消息或命令";

    public override Version Version => new Version(1, 0, 4);

    public string ConfigPath => Path.Combine(TShock.SavePath, "AutoBroadcastConfig.json");

    public AutoBroadcast(Main Game)
        : base(Game)
    {
    }

    public override void Initialize()
    {
        ServerApi.Hooks.GameInitialize.Register(this, OnInitialize, -5);
        ServerApi.Hooks.ServerChat.Register(this, OnChat);
        RegionHooks.RegionEntered += OnRegionEnter;
        GeneralHooks.ReloadEvent += AutoBC;
    }

    protected override void Dispose(bool Disposing)
    {
        if (Disposing)
        {
            ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
            RegionHooks.RegionEntered -= OnRegionEnter;
            GeneralHooks.ReloadEvent -= AutoBC;

        }
        base.Dispose(Disposing);
    }

    public void OnInitialize(EventArgs args)
    {
        try
        {
            Config = ABConfig.Read(ConfigPath).Write(ConfigPath);
        }
        catch (Exception ex)
        {
            Config = new ABConfig();
            TShock.Log.ConsoleError("[自动广播] 分析AutoBroadcast配置时发生异常！\n{0}".SFormat(ex.ToString()));
        }

    }

    public void AutoBC(ReloadEventArgs args)
    {
        try
        {
            Config = ABConfig.Read(ConfigPath).Write(ConfigPath);
            TShock.Log.Info("已成功重新加载AutoBroadcast配置！");
        }
        catch (Exception ex)
        {
            Config = new ABConfig();
            args.Player.SendWarningMessage("分析AutoBroadcast配置时发生异常！查看日志了解更多详细信息！");
            TShock.Log.ConsoleError("[自动广播] 分析AutoBroadcast配置时发生异常！\n{0}".SFormat(ex.ToString()));
        }
    }

    public void OnChat(ServerChatEventArgs args)
    {
        DateTime now = DateTime.Now;
        if (TShock.Players[args.Who] == null || TShock.Players[args.Who].Group == null)
        {
            return; // 如果玩家对象或玩家组为null，则直接返回，避免空引用异常
        }
        string name = TShock.Players[args.Who].Group.Name;
        lock (Config.Broadcasts)
        {
            ABConfig.Broadcast[] broadcasts = Config.Broadcasts;
            foreach (ABConfig.Broadcast broadcast in broadcasts)
            {
                string[] array = Array.Empty<string>();
                string[] messages = new string[broadcast.Messages.Length];
                float[] colour = Array.Empty<float>();
                if (Timeout(now))
                {
                    break;
                }
                if (broadcast == null || !broadcast.Enabled || (!broadcast.Groups.Contains(name) && !broadcast.Groups.Contains("*")))
                {
                    continue;
                }
                //string[] messages2 = broadcast.Messages;
                for (int j = 0; j < broadcast.Messages.Length; j++)
                {
                    messages[j] = broadcast.Messages[j].Replace("{player}", TShock.Players[args.Who].Name);
                }
                string[] triggerWords = broadcast.TriggerWords;
                foreach (string value in triggerWords)
                {
                    if (Timeout(now))
                    {
                        return;
                    }
                    if (args.Text.Contains(value))
                    {
                        if (broadcast.TriggerToWholeGroup && broadcast.Groups.Length != 0)
                        {
                            array = broadcast.Groups;
                        }
                        //messages = broadcast.Messages;
                        colour = broadcast.ColorRGB;
                        break;
                    }
                }
                bool flag = false;
                string[] array2 = array;
                foreach (string text in array2)
                {
                    if (text == "*")
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    array = new string[1] { "*" };
                }
                if (array.Length != 0)
                {
                    BroadcastToGroups(array, messages, colour);
                }
                else
                {
                    BroadcastToPlayer(args.Who, messages, colour);
                }
            }
        }
    }

    public void OnRegionEnter(RegionHooks.RegionEnteredEventArgs args)
    {
        DateTime now = DateTime.Now;
        string name = args.Player.Group.Name;
        lock (Config.Broadcasts)
        {
            ABConfig.Broadcast[] broadcasts = Config.Broadcasts;
            foreach (ABConfig.Broadcast broadcast in broadcasts)
            {
                if (Timeout(now))
                {
                    break;
                }
                if (broadcast == null || !broadcast.Enabled || !broadcast.Groups.Contains(name))
                {
                    continue;
                }
                string[] messages = broadcast.Messages;
                for (int j = 0; j < messages.Length; j++)
                {
                    messages[j] = messages[j].Replace("{player}", args.Player.Name);
                    messages[j] = messages[j].Replace("{region}", args.Player.CurrentRegion.Name);
                }
                string[] triggerRegions = broadcast.TriggerRegions;
                foreach (string text in triggerRegions)
                {
                    if (args.Player.CurrentRegion.Name == text)
                    {
                        if (broadcast.RegionTrigger == "all")
                        {
                            BroadcastToAll(messages, broadcast.ColorRGB);
                        }
                        else if (broadcast.RegionTrigger == "region")
                        {
                            BroadcastToRegion(text, messages, broadcast.ColorRGB);
                        }
                        else if (broadcast.RegionTrigger == "self")
                        {
                            BroadcastToPlayer(args.Player.Index, messages, broadcast.ColorRGB);
                        }
                    }
                }
            }
        }
    }

    public void OnUpdate(object Sender, EventArgs e)
    {
        if (Main.worldID == 0 || ULock)
        {
            return;
        }
        ULock = true;
        DateTime now = DateTime.Now;
        int num = 0;
        lock (Config.Broadcasts)
        {
            num = Config.Broadcasts.Length;
        }
        for (int i = 0; i < num; i++)
        {
            if (Timeout(now))
            {
                return;
            }
            string[] array = new string[0];
            string[] messages = new string[0];
            float[] colour = new float[0];
            lock (Config.Broadcasts)
            {
                if (Config.Broadcasts[i] != null && Config.Broadcasts[i].Enabled && Config.Broadcasts[i].Interval >= 1)
                {
                    if (Config.Broadcasts[i].StartDelay <= 0)
                    {
                        Config.Broadcasts[i].StartDelay = Config.Broadcasts[i].Interval;
                        array = Config.Broadcasts[i].Groups;
                        messages = Config.Broadcasts[i].Messages;
                        colour = Config.Broadcasts[i].ColorRGB;
                        goto IL_01b4;
                    }
                    Config.Broadcasts[i].StartDelay--;
                }
            }
            continue;
        IL_01b4:
            bool flag = false;
            string[] array2 = array;
            foreach (string text in array2)
            {
                if (text == "*")
                {
                    flag = true;
                }
            }
            if (flag)
            {
                array = new string[1] { "*" };
            }
            if (array.Length != 0)
            {
                BroadcastToGroups(array, messages, colour);
            }
            else
            {
                BroadcastToAll(messages, colour);
            }
        }
        ULock = false;
    }

    public static void BroadcastToGroups(string[] Groups, string[] Messages, float[] Colour)
    {
        foreach (string text in Messages)
        {
            if (text.StartsWith(TShock.Config.Settings.CommandSpecifier) || text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier))
            {
                Commands.HandleCommand(TSPlayer.Server, text);
                continue;
            }
            lock (TShock.Players)
            {
                TSPlayer[] players = TShock.Players;
                foreach (TSPlayer tSPlayer in players)
                {
                    if (tSPlayer != null && (Groups.Contains(tSPlayer.Group.Name) || Groups[0] == "*"))
                    {
                        string text2 = text;
                        text2 = text2.Replace("{player}", tSPlayer.Name);
                        tSPlayer.SendMessage(text2, (byte)Colour[0], (byte)Colour[1], (byte)Colour[2]);
                    }
                }
            }
        }
    }

    public static void BroadcastToRegion(string region, string[] Messages, float[] Colour)
    {
        foreach (string text in Messages)
        {
            if (text.StartsWith(TShock.Config.Settings.CommandSpecifier) || text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier))
            {
                Commands.HandleCommand(TSPlayer.Server, text);
                continue;
            }
            IEnumerable<TSPlayer> enumerable = from TSPlayer plr in TShock.Players
                                               where plr != null && plr.CurrentRegion != null && plr.CurrentRegion.Name == region
                                               select plr;
            foreach (TSPlayer item in enumerable)
            {
                item.SendMessage(text, (byte)Colour[0], (byte)Colour[1], (byte)Colour[2]);
            }
        }
    }

    public static void BroadcastToAll(string[] Messages, float[] Colour)
    {
        foreach (string text in Messages)
        {
            if (text.StartsWith(TShock.Config.Settings.CommandSpecifier) || text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier))
            {
                Commands.HandleCommand(TSPlayer.Server, text);
                continue;
            }
            TSPlayer[] players = TShock.Players;
            foreach (TSPlayer tSPlayer in players)
            {
                if (tSPlayer != null)
                {
                    string text2 = text;
                    text2 = text2.Replace("{player}", tSPlayer.Name);
                    tSPlayer.SendMessage(text2, (byte)Colour[0], (byte)Colour[1], (byte)Colour[2]);
                }
            }
        }
    }

    public static void BroadcastToPlayer(int plr, string[] Messages, float[] Colour)
    {
        foreach (string text in Messages)
        {
            if (text.StartsWith(TShock.Config.Settings.CommandSpecifier) || text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier))
            {
                Commands.HandleCommand(TSPlayer.Server, text);
                continue;
            }
            lock (TShock.Players)
            {
                string text2 = text;
                text2 = text2.Replace("{player}", TShock.Players[plr].Name);
                TShock.Players[plr].SendMessage(text2, (byte)Colour[0], (byte)Colour[1], (byte)Colour[2]);
            }
        }
    }

    public static bool Timeout(DateTime Start, int ms = 500, bool warn = true)
    {
        bool flag = (DateTime.Now - Start).TotalMilliseconds >= (double)ms;
        if (ms == 500 && flag)
        {
            ULock = false;
        }
        if (warn && flag)
        {
            Console.WriteLine("在AutoBroadcast中检测到挂钩超时。你可能想报告这件事。");
            TShock.Log.Error("在AutoBroadcast中检测到挂钩超时。你可能想报告这件事。");
        }
        return flag;
    }
}
