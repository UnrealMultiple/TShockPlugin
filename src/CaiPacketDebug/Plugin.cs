using LazyAPI;
using On.Terraria.Chat;
using On.Terraria.Net;
using System.Reflection;
using Terraria;
using Terraria.Net.Sockets;
using TerrariaApi.Server;
using TShockAPI;
using TrProtocol;
using ChatMessage = Terraria.Chat.ChatMessage;
using Hooks = On.OTAPI.Hooks;
using NetPacket = Terraria.Net.NetPacket;

namespace CaiPacketDebug;

[ApiVersion(2, 1)]
public class CaiPacketDebug : LazyPlugin
{
    private readonly PacketSerializer _clientPacketSerializer = new(true);

    private readonly PacketSerializer _serverPacketSerializer = new(false);

    private bool _clientToServerDebug;
    private bool _serverToClientDebug;

    public CaiPacketDebug(Main game) : base(game)
    {
        this.Order = int.MinValue;
    }

    public override string Author => "Cai";
    public override string Description => GetString("用于调试数据包的插件捏~");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; 
    public override Version Version => new Version(2025, 1, 25, 2);


    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("CaiPacketDebug.Use", this.CpdCmd, "cpd"));
        Hooks.MessageBuffer.InvokeGetData += this.MessageBufferOnInvokeGetData;
        Hooks.NetMessage.InvokeSendBytes += this.NetMessageOnInvokeSendBytes;
        NetManager.SendData += this.NetManagerOnSendData;
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Hooks.MessageBuffer.InvokeGetData -= this.MessageBufferOnInvokeGetData;
            Hooks.NetMessage.InvokeSendBytes -= this.NetMessageOnInvokeSendBytes;
            NetManager.SendData -= this.NetManagerOnSendData;
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.CpdCmd);
        }

        base.Dispose(disposing);
    }


    private void CpdCmd(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            args.Player.SendInfoMessage("[CaiPacketDebug]\n" +
                                        $"C->S: {(this._clientToServerDebug ? GetString("[c/00FF00:已启用]") : GetString("[c/FF0000:已禁用]"))}\n" +
                                        $"S->C: {(this._serverToClientDebug ? GetString("[c/00FF00:已启用]") : GetString("[c/FF0000:已禁用]"))}");
            args.Player.SendWarningMessage(GetString("*/cpd cts|stc 开关数据包调试"));
            return;
        }

        switch (args.Parameters[0])
        {
            case "cs":
            case "cts":
                this._clientToServerDebug = !this._clientToServerDebug;
                args.Player.SendInfoMessage("[CaiPacketDebug]\n" +
                                            $"C->S: {(this._clientToServerDebug ? GetString("[c/00FF00:已启用]") : GetString("[c/FF0000:已禁用]"))}");
                return;
            case "sc":
            case "stc":
                this._serverToClientDebug = !this._serverToClientDebug;
                args.Player.SendInfoMessage("[CaiPacketDebug]\n" +
                                            $"S->C: {(this._serverToClientDebug ? GetString("[c/00FF00:已启用]") : GetString("[c/FF0000:已禁用]"))}");
                return;
        }
    }
    private void NetManagerOnSendData(NetManager.orig_SendData orig, Terraria.Net.NetManager self, ISocket socket, NetPacket netPacket)
    {   
        
        orig(self, socket, netPacket);
        var index = Netplay.Clients.First(x=>x.Socket == socket).Id;
        if (this._serverToClientDebug)
        {
            using MemoryStream memoryStream = new(netPacket.Buffer.Data);
            using BinaryReader reader = new(memoryStream);
            Packet packet;
            try
            {
                packet = this._clientPacketSerializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(GetString($"[S->C({index})] 解析数据包时出错:{ex}"));
                Console.ResetColor();
                return;
            }
            if (Config.Instance.ServerToClient.ExcludePackets.Contains((int) packet.Type))
            {
                return;
            }

            if (Config.Instance.ServerToClient.WhiteListMode &&
                !Config.Instance.ServerToClient.WhiteListPackets.Contains((int) packet.Type))
            {
                return;
            }

            PrintPacket($"[S->C({index})]", ConsoleColor.Blue, packet);
        }
    }
    private void NetMessageOnInvokeSendBytes(Hooks.NetMessage.orig_InvokeSendBytes orig, ISocket socket, byte[] data,
        int offset, int size, SocketSendCallback callback, object state, int remoteclient)
    {
        orig.Invoke(socket, data, offset, size, callback, state, remoteclient);

        if (this._serverToClientDebug)
        {
            using MemoryStream memoryStream = new(data);
            using BinaryReader reader = new(memoryStream);
            Packet packet;
            try
            {
                packet = this._clientPacketSerializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(GetString($"[S->C({remoteclient})] 解析数据包时出错:{ex}"));
                Console.ResetColor();
                return;
            }
            if (Config.Instance.ServerToClient.ExcludePackets.Contains((int) packet.Type))
            {
                return;
            }

            if (Config.Instance.ServerToClient.WhiteListMode &&
                !Config.Instance.ServerToClient.WhiteListPackets.Contains((int) packet.Type))
            {
                return;
            }

            PrintPacket($"[S->C({remoteclient})]", ConsoleColor.Blue, packet);
        }
    }


    private bool MessageBufferOnInvokeGetData(Hooks.MessageBuffer.orig_InvokeGetData orig, MessageBuffer instance,
        ref byte packetid, ref int readoffset, ref int start, ref int length, ref int messagetype, int maxpackets)
    {
        var result = orig.Invoke(instance, ref packetid, ref readoffset, ref start, ref length, ref messagetype,
            maxpackets);
        if (this._clientToServerDebug)
        {
            instance.ResetReader();
            Packet packet;
            try
            {
                packet = this._serverPacketSerializer.Deserialize(instance.reader);
            }
            
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(GetString($"[C({instance.whoAmI})->S] 解析数据包时出错:{ex}"));
                Console.ResetColor();
                return result;
            }
            if (Config.Instance.ClientToServer.ExcludePackets.Contains((int) packet.Type))
            {
                return result;
            }

            if (Config.Instance.ClientToServer.WhiteListMode &&
                !Config.Instance.ClientToServer.WhiteListPackets.Contains((int) packet.Type))
            {
                return result;
            }

            PrintPacket($"[C({instance.whoAmI})->S]", ConsoleColor.Red, packet);
        }

        return result;
    }


    public static void PrintPacket(string prefix, ConsoleColor prefixColor, Packet packet)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = prefixColor;
        Console.Write($"{prefix} ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"[{packet.GetType().Name}({(int) packet.Type})] ");
        var properties = packet.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var prop in properties.Where(prop => prop.Name != nameof(Type)))
        {
            var value = prop.GetValue(packet)!;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"{prop.Name}");
            Console.ForegroundColor = originalColor;
            Console.Write(": ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (value is byte[] byteArray)
            {
                Console.Write($"[{string.Join(", ", byteArray)}]");
            }
            else
            {
                Console.Write(value);
            }

            if (prop != properties.Last(prop => prop.Name != nameof(Type)))
            {
                Console.ForegroundColor = originalColor;
                Console.Write(", ");
            }
        }

        Console.WriteLine();
        Console.ForegroundColor = originalColor;
    }
}