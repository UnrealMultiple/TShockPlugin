using System.Reflection;
using Terraria;
using Terraria.Net.Sockets;
using TerrariaApi.Server;
using TrProtocol;
using TShockAPI;
using TShockAPI.Hooks;
using Hooks = On.OTAPI.Hooks;

namespace CaiPacketDebug;

[ApiVersion(2, 1)]
public class CaiPacketDebug : TerrariaPlugin
{
    private readonly PacketSerializer _clientPacketSerializer = new (true);

    private readonly PacketSerializer _serverPacketSerializer = new (false);

    private bool _clientToServerDebug;
    private bool _serverToClientDebug;

    public CaiPacketDebug(Main game) : base(game)
    {
    }

    public override string Author => "Cai";
    public override string Description => "用于调试数据包的插件捏~";
    public override string Name => "CaiPacketDebug";
    public override Version Version => new (2024, 11, 10, 0);


    public override void Initialize()
    {
        Config.Read();
        this._clientToServerDebug = Config.Settings.ClientToServer.DebugAfterInit;
        this._serverToClientDebug = Config.Settings.ServerToClient.DebugAfterInit;
        Commands.ChatCommands.Add(new Command("CaiPacketDebug.Use", this.CpdCmd, "cpd"));
        Hooks.MessageBuffer.InvokeGetData += this.MessageBufferOnInvokeGetData;
        Hooks.NetMessage.InvokeSendBytes += this.NetMessageOnInvokeSendBytes;
        GeneralHooks.ReloadEvent += this.GeneralHooksOnReloadEvent;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Hooks.MessageBuffer.InvokeGetData -= this.MessageBufferOnInvokeGetData;
            Hooks.NetMessage.InvokeSendBytes -= this.NetMessageOnInvokeSendBytes;
            GeneralHooks.ReloadEvent -= this.GeneralHooksOnReloadEvent;
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.CpdCmd);
        }

        base.Dispose(disposing);
    }

    private void GeneralHooksOnReloadEvent(ReloadEventArgs e)
    {
        Config.Read();
        e.Player.SendSuccessMessage("[CaiPacketDebug]配置文件已重载喵~");
    }

    private void CpdCmd(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            args.Player.SendInfoMessage("[CaiPacketDebug]\n" +
                                        $"C->S: {(this._clientToServerDebug ? "[c/00FF00:已启用]" : "[c/FF0000:已禁用]")}\n" +
                                        $"S->C: {(this._serverToClientDebug ? "[c/00FF00:已启用]" : "[c/FF0000:已禁用]")}");
            args.Player.SendWarningMessage("*/cpd cts|stc 开关数据包调试");
            return;
        }

        switch (args.Parameters[0])
        {
            case "cs":
            case "cts":
                this._clientToServerDebug = !this._clientToServerDebug;
                args.Player.SendInfoMessage("[CaiPacketDebug]\n" +
                                            $"C->S: {(this._clientToServerDebug ? "[c/00FF00:已启用]" : "[c/FF0000:已禁用]")}");
                return;
            case "sc":
            case "stc":
                this._serverToClientDebug = !this._serverToClientDebug;
                args.Player.SendInfoMessage("[CaiPacketDebug]\n" +
                                            $"S->C: {(this._serverToClientDebug ? "[c/00FF00:已启用]" : "[c/FF0000:已禁用]")}");
                return;
        }
    }

    private void NetMessageOnInvokeSendBytes(Hooks.NetMessage.orig_InvokeSendBytes orig, ISocket socket, byte[] data,
        int offset, int size, SocketSendCallback callback, object state, int remoteclient)
    {
        orig.Invoke(socket, data, offset, size, callback, state, remoteclient);

        if (this._serverToClientDebug)
        {
            using MemoryStream memoryStream = new (data);
            using BinaryReader reader = new (memoryStream);
            var packet = this._clientPacketSerializer.Deserialize(reader);

            if (Config.Settings.ServerToClient.ExcludePackets.Contains((int) packet.Type))
            {
                return;
            }

            if (Config.Settings.ServerToClient.WhiteListMode &&
                !Config.Settings.ServerToClient.WhiteListPackets.Contains((int) packet.Type))
            {
                return;
            }

            PrintPacket("[S->C]", ConsoleColor.Blue, packet);
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

            var packet = this._serverPacketSerializer.Deserialize(instance.reader);

            if (Config.Settings.ClientToServer.ExcludePackets.Contains((int) packet.Type))
            {
                return result;
            }

            if (Config.Settings.ClientToServer.WhiteListMode &&
                !Config.Settings.ClientToServer.WhiteListPackets.Contains((int) packet.Type))
            {
                return result;
            }

            PrintPacket("[C->S]", ConsoleColor.Red, packet);
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