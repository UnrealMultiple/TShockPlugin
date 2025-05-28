using System.Net;
using TrProtocol;
using TrProtocol.Models;
using TrProtocol.Packets;
using TrProtocol.Packets.Modules;
using TShockAPI;

namespace Dummy;
internal class DummyPlayer
{
    public byte PlayerSlot { get; private set; }
    public string CurRelease = "Terraria279";

    private readonly string UUID;

    private readonly SyncPlayer PlayerInfo;
    public bool IsPlaying { get; private set; }

    public event Action<DummyPlayer, NetworkText, Color>? OnChat;
    public event Action<DummyPlayer, string>? OnMessage;
    public Func<bool> shouldExit = () => false;

    private readonly Dictionary<Type, Action<Packet>> handlers = [];

    private readonly TrClient client;

    public TSPlayer TSPlayer => TShock.Players[this.PlayerSlot];

    public bool Active { get; private set; }

    private Timer _timer = null!;

    public DummyPlayer(SyncPlayer playerInfo, string uuid)
    {
        this.PlayerInfo = playerInfo;
        this.UUID = uuid;
        this.client = new TrClient();
        this.InternalOn();
    }

    public void KillServer()
    {
        this.client.KillServer();
    }

    public void Close()
    {
        this.IsPlaying = false;
        this.Active = false;
        this.client.Close();
        this._timer.Dispose();
    }

    public void SendPacket(Packet packet)
    {
        if (packet is IPlayerSlot ips)
        {
            ips.PlayerSlot = this.PlayerSlot;
        }
        this.client.Send(packet);
    }
    public void Hello(string message)
    {
        this.SendPacket(new ClientHello { Version = message });
    }

    public void TileGetSection(int x, int y)
    {
        this.SendPacket(new RequestTileData { Position = new Position { X = x, Y = y } });
    }

    public void Spawn(short x, short y)
    {
        this.SendPacket(new SpawnPlayer
        {
            Position = new ShortPosition { X = x, Y = y },
            Context = PlayerSpawnContext.SpawningIntoWorld
        });
    }

    public void SendPlayer(string uuid)
    {
        this.SendPacket(new ClientUUID() { UUID = uuid });
        this.SendPacket(this.PlayerInfo);
        this.SendPacket(new PlayerHealth { StatLifeMax = 100, StatLife = 100 });
        for (byte i = 0; i < 73; ++i)
        {
            this.SendPacket(new SyncEquipment { ItemSlot = i });
        }
    }

    public void ChatText(string message)
    {
        this.SendPacket(new NetTextModuleC2S
        {
            Command = "Say",
            Text = message
        });
    }



    public void On<T>(Action<T> handler) where T : Packet
    {
        void Handler(Packet p) => handler((T) p);

        if (this.handlers.TryGetValue(typeof(T), out var val))
        {
            this.handlers[typeof(T)] = val + Handler;
        }
        else
        {
            this.handlers.Add(typeof(T), Handler);
        }
    }

    private void InternalOn()
    {
        this.On<StatusText>(status => OnChat?.Invoke(this, status.Text, Color.White));
        this.On<NetTextModuleS2C>(text => OnChat?.Invoke(this, text.Text, text.Color));
        this.On<SmartTextMessage>(text => OnChat?.Invoke(this, text.Text, text.Color));
        this.On<Kick>(kick =>
        {
            OnMessage?.Invoke(this, "Kicked : " + kick.Reason);
            this.Close();
        });
        this.On<LoadPlayer>(player =>
        {
            this.PlayerSlot = player.PlayerSlot;
            this.SendPlayer(this.UUID);
            this.SendPacket(new RequestWorldInfo());
        });
        this.On<WorldData>(i =>
        {
            if (!this.IsPlaying)
            {
                this.TileGetSection(i.SpawnX, i.SpawnY);
                this.Spawn(i.SpawnX, i.SpawnY);
                this.IsPlaying = true;
            }
        });
        this.On<StartPlaying>(_ => this.SendPacket(new RequestWorldInfo()));
    }

    public void GameLoop(string host, int port, string password)
    {
        this.client.Connect(host, port);
        this.GameLoopInternal(password);
    }
    public void GameLoop(IPEndPoint endPoint, string password, IPEndPoint? proxy = null)
    {
        this.client.Connect(endPoint, proxy);
        this.GameLoopInternal(password);
    }

    private void GameLoopInternal(string password)
    {
        this.Hello(this.CurRelease);
        this.On<RequestPassword>(_ => this.SendPacket(new SendPassword { Password = password }));
        this.Active = true;
        this._timer = new(this.OnFrame, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000));
        Task.Run(() =>
        {
            while (this.Active && !this.shouldExit())
            {
                var packet = this.client.Receive();
                try
                {
                    if (this.handlers.TryGetValue(packet.GetType(), out var act))
                    {
                        act(packet);
                    }
                }
                catch (Exception e)
                {
                    TShock.Log.ConsoleError($"{this.PlayerInfo.Name} Exception caught when trying to parse packet {packet.Type}\n{e}");
                }
            }
            this.Close();
        });
    }

    private void OnFrame(object? state)
    {
        if (this.Active)
        {
            this.SendPacket(new PlayerActive() { PlayerSlot = this.PlayerSlot, Active = true });
        }
    }
}

