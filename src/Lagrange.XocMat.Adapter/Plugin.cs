using Lagrange.XocMat.Adapter.DB;
using Lagrange.XocMat.Adapter.Enumerates;
using Lagrange.XocMat.Adapter.Protocol;
using Lagrange.XocMat.Adapter.Protocol.PlayerMessage;
using Lagrange.XocMat.Adapter.Protocol.ServerMessage;
using Lagrange.XocMat.Adapter.Net;
using Lagrange.XocMat.Adapter.Protocol.Action;
using Lagrange.XocMat.Adapter.Protocol.Internet;
using Lagrange.XocMat.Adapter.Setting;
using ProtoBuf;
using Rests;
using System.Reflection;
using System.Threading.Channels;
using System.Timers;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Lagrange.XocMat.Adapter;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => GetString("Lagrange.XocMat机器人适配插件");

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 0, 7);

    internal static readonly List<TSPlayer> ServerPlayers = new();

    private long TimerCount = 0;

    private readonly System.Timers.Timer Timer = new();

    internal static Channel<int> Channeler = Channel.CreateBounded<int>(1);

    internal static readonly Dictionary<int, KillNpc> DamageBoss = [];

#nullable disable
    internal static WebSocketServices Services;

    internal static PlayerOnline Onlines;

    internal static PlayerDeath Deaths;
#nullable enable
    public Plugin(Main game) : base(game)
    {
        AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomain_AssemblyResolve;
    }

    public override void Initialize()
    {
        if (!Directory.Exists("world"))
        {
            Directory.CreateDirectory("world");
        }
        Services = new();
        Onlines = [];
        Deaths = [];
        Utils.MapingCommand();
        Utils.MapingRest();
        Services.OnConnect += this.SkocketConnect;
        Services.OnMessage += this.SocketClient_OnMessage;
        ServerApi.Hooks.GamePostInitialize.Register(this, this.OnInit);
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreet);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
        ServerApi.Hooks.ServerJoin.Register(this, this.OnJoin);
        ServerApi.Hooks.ServerChat.Register(this, this.OnChat);
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        ServerApi.Hooks.NpcSpawn.Register(this, this.OnSpawn);
        ServerApi.Hooks.NpcStrike.Register(this, this.OnStrike);
        ServerApi.Hooks.NpcKilled.Register(this, this.OnKillNpc);
        GetDataHandlers.KillMe.Register(this.OnKill);
        Config.Instance.SocketConfig.EmptyCommand.ForEach(x => Commands.ChatCommands.Add(new("", (_) => { }, x)));
        GeneralHooks.ReloadEvent += Config.Reload;
        Utils.HandleCommandLine(Environment.GetCommandLineArgs());
        this.Timer.AutoReset = true;
        this.Timer.Enabled = true;
        this.Timer.Interval = Config.Instance.SocketConfig.HeartBeatTimer;
        this.Timer.Elapsed += this.TimerUpdate;
        Services.Start();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Services.OnConnect -= this.SkocketConnect;
            Services.OnMessage -= this.SocketClient_OnMessage;
            Services.Dispose();
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.OnInit);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreet);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnJoin);
            ServerApi.Hooks.ServerChat.Deregister(this, this.OnChat);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
            ServerApi.Hooks.NpcSpawn.Deregister(this, this.OnSpawn);
            ServerApi.Hooks.NpcStrike.Deregister(this, this.OnStrike);
            ServerApi.Hooks.NpcKilled.Deregister(this, this.OnKillNpc);
            GetDataHandlers.KillMe.UnRegister(this.OnKill);
            GeneralHooks.ReloadEvent -= Config.Reload;
            this.Timer.Elapsed -= this.TimerUpdate;
            this.Timer.Stop();
            RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            this.RemoveAssemblyRest(Assembly.GetExecutingAssembly());
        }
        base.Dispose(disposing);
    }

    public static void RemoveAssemblyCommands(Assembly assembly)
    {
        Commands.ChatCommands.RemoveAll(cmd => cmd.GetType().Assembly == assembly);
    }

    public void RemoveAssemblyRest(Assembly assembly)
    {
        if (typeof(Rest)
            .GetField("commands", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(TShock.RestApi) is List<RestCommand> rests)
        {
            rests.RemoveAll(cmd => cmd.GetType().GetField("callback", BindingFlags.NonPublic)?.GetType().Assembly == assembly);
        }
    }

    private void TimerUpdate(object? sender, ElapsedEventArgs e)
    {
        var obj = new BaseMessage()
        {
            MessageType = PostMessageType.HeartBeat
        };
        Services.SendMessage(Utils.SerializeObj(obj));
    }

    private void OnKillNpc(NpcKilledEventArgs args)
    {
        if (args.npc != null && args.npc.active && args.npc.boss)
        {
            if (DamageBoss.TryGetValue(args.npc.netID, out var KillNpc))
            {
                KillNpc.IsAlive = false;
                KillNpc.KillTime = DateTime.Now;
            }
        }
    }

    private void OnStrike(NpcStrikeEventArgs args)
    {
        if (args.Npc != null && args.Npc.active && args.Npc.boss)
        {
            if (DamageBoss.TryGetValue(args.Npc.netID, out var KillNpc) && KillNpc != null)
            {
                var damage = KillNpc.Strikes.Find(x => x.Player == args.Player.name);
                if (damage != null)
                {
                    damage.Damage += args.Damage;
                }
                else
                {
                    KillNpc.Strikes.Add(new()
                    {
                        Player = args.Player.name,
                        Damage = args.Damage
                    });
                }
            }
        }
    }

    private void OnSpawn(NpcSpawnEventArgs args)
    {
        var npc = Main.npc[args.NpcId];
        if (npc != null && npc.active && npc.boss)
        {
            DamageBoss[npc.netID] = new()
            {
                Id = npc.netID,
                Name = npc.FullName,
                MaxLife = npc.lifeMax
            };
        }
    }

    private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        var resourceName = $"embedded.{new AssemblyName(args.Name).Name}.dll";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream != null)
        {
            var assemblyData = new byte[stream.Length];
            stream.ReadExactly(assemblyData);
            return Assembly.Load(assemblyData);
        }
        return null;
    }

    private void SkocketConnect()
    {
        var obj = new BaseMessage()
        {
            MessageType = PostMessageType.Connect,
        };
        var stream = Utils.SerializeObj(obj);
        Services.SendMessage(stream);
    }

    private void SocketClient_OnMessage(byte[] buffer)
    {
        try
        {
            var baseMsg = Serializer.Deserialize<BaseAction>(new ReadOnlyMemory<byte>(buffer));
            if (baseMsg.Token == Config.Instance.SocketConfig.Token || baseMsg.ActionType == ActionType.ConnectStatus)
            {
                switch (baseMsg.MessageType)
                {
                    case PostMessageType.Action:
                        ActionHandler.Adapter(baseMsg);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError($"[Lagrange.XocMat.Adapter] 解析通信时出错: {ex}");
        }
    }

    private void OnKill(object? sender, GetDataHandlers.KillMeEventArgs e)
    {
        Deaths.Add(e.Player.Name);
    }

    private void OnUpdate(EventArgs args)
    {
        this.TimerCount++;
        if (this.TimerCount % 60 == 0)
        {
            ServerPlayers.ForEach(p =>
            {
                if (p != null && p.Active)
                {
                    Onlines[p.Name] += 1;
                }
            });
        }
    }

    private void OnInit(EventArgs args)
    {
        if (Channeler.Reader.TryRead(out var mode))
        {
            Main.GameMode = mode;
            TSPlayer.All.SendData(PacketTypes.WorldInfo);
        }
        var obj = new GameInitMessage();
        Services.SendMessage(Utils.SerializeObj(obj));
    }

    private void OnChat(ServerChatEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player != null && player.Active)

        {
            if (args.Text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier)
                || args.Text.StartsWith(TShock.Config.Settings.CommandSpecifier))
            {
                var prefix = args.Text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier) ? TShock.Config.Settings.CommandSilentSpecifier : TShock.Config.Settings.CommandSpecifier;
                var obj = new PlayerCommandMessage()
                {
                    MessageType = PostMessageType.PlayerCommand,
                    Name = player.Name,
                    Group = player.Group.Name,
                    Prefix = player.Group.Prefix,
                    IsLogin = player.IsLoggedIn,
                    Command = args.Text,
                    CommandPrefix = prefix,
                    Mute = player.mute
                };
                Services.SendMessage(Utils.SerializeObj(obj));
            }
            else
            {
                var obj = new PlayerChatMessage()
                {
                    MessageType = PostMessageType.PlayerMessage,
                    Name = player.Name,
                    Group = player.Group.Name,
                    Prefix = player.Group.Prefix,
                    IsLogin = player.IsLoggedIn,
                    Text = args.Text,
                    Mute = player.mute
                };
                var stream = Utils.SerializeObj(obj);
                Services.SendMessage(stream);
            }
        }
    }

    private void OnJoin(JoinEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player != null)
        {
            if (Config.Instance.LimitJoin && TShock.UserAccounts.GetUserAccountByName(player.Name) == null)
            {
                player.Disconnect(string.Join("\n", Config.Instance.DisConnentFormat));
            }
        }
    }

    private void OnLeave(LeaveEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player != null)
        {
            if (!ServerPlayers.Contains(player))
            {
                return;
            }

            ServerPlayers.Remove(player);
            var obj = new PlayerLeaveMessage()
            {
                MessageType = PostMessageType.PlayerLeave,
                Name = player.Name,
                Group = player.Group.Name,
                Prefix = player.Group.Prefix,
                IsLogin = player.IsLoggedIn,
            };
            Services.SendMessage(Utils.SerializeObj(obj));
        }
        Onlines.UpdateAll();
    }

    private void OnGreet(GreetPlayerEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player != null && player.Active)
        {
            ServerPlayers.Add(player);
            var obj = new PlayerJoinMessage()
            {
                MessageType = PostMessageType.PlayerJoin,
                Name = player.Name,
                Group = player.Group.Name,
                Prefix = player.Group.Prefix,
                IsLogin = player.IsLoggedIn,
            };
            Services.SendMessage(Utils.SerializeObj(obj));
        }
    }
}
