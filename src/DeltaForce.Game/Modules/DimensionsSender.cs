using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using LazyAPI.Utility;
using Terraria;
using TShockAPI;

namespace DeltaForce.Game.Modules;

public static class DimensionsSender
{
    public enum SubMessageID : short
    {
        ClientAddress = 1,
        ChangeSever = 2,
        ChangeCustomizedServer = 3,
        OnlineInfoRequest = 4,
        OnlineInfoResponse = 5
    }

    public static void SendPlayerToServer(TSPlayer player, string serverName)
    {
        SendDimensionUpdatePacket(player.Index, SubMessageID.ChangeSever, serverName, 0);
    }

    public static void SendPlayerToCustomServer(TSPlayer player, string ip, ushort port)
    {
        SendDimensionUpdatePacket(player.Index, SubMessageID.ChangeCustomizedServer, ip, port);
    }
    private static unsafe void SendDimensionUpdatePacket(int playerId, SubMessageID subType, string content, ushort port)
    {
        var socket = Netplay.Clients[playerId].Socket;
        if (socket == null)
        {
            TShock.Log.ConsoleError(GetString($"[DimensionsSender] 玩家 {playerId} 的Socket为空"));
            return;
        }

        try
        {
            var contentBytes = Encoding.UTF8.GetBytes(content ?? string.Empty);
            var contentLengthBytes = Get7BitEncodedIntSize(contentBytes.Length);

            var packetBodySize = 2 + contentLengthBytes + contentBytes.Length; // SubType + ContentLength + Content
            var hasPort = subType == SubMessageID.ChangeCustomizedServer ||
                           subType == SubMessageID.ClientAddress;

            if (hasPort)
            {
                packetBodySize += 2;
            }

            var totalSize = 2 + 1 + packetBodySize;
            var packetData = new byte[totalSize];

            fixed (byte* ptr = packetData)
            {
                var current = ptr;
                Unsafe.WriteUnaligned(current, (ushort) totalSize);
                current += 2;
                *current = 67;
                current += 1;
                Unsafe.WriteUnaligned(current, (short) subType);
                current += 2;
                Write7BitEncodedInt(ref current, contentBytes.Length);
                fixed (byte* contentPtr = contentBytes)
                {
                    Buffer.MemoryCopy(contentPtr, current, contentBytes.Length, contentBytes.Length);
                }
                current += contentBytes.Length;
                if (hasPort)
                {
                    Unsafe.WriteUnaligned(current, port);
                    current += 2;
                }
            }
            socket.AsyncSend(packetData, 0, totalSize, (sendResult) =>
            {
                if (sendResult is SocketError error && error != SocketError.Success)
                {
                    TShock.Log.ConsoleError(GetString($"[DimensionsSender] 发送数据包失败: {error}"));
                }
                else
                {
                    TShock.Log.ConsoleInfo(GetString($"[DimensionsSender] 已发送切换指令到玩家 {playerId}: {content}:{port}"));
                }
            });
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString($"[DimensionsSender] 发送数据包异常: {ex.Message}"));
        }
    }

    private static int Get7BitEncodedIntSize(int value)
    {
        var num = (uint) value;
        var size = 0;
        while (num >= 0x80)
        {
            size++;
            num >>= 7;
        }
        size++;
        return size;
    }
    private static unsafe void Write7BitEncodedInt(ref byte* destination, int value)
    {
        var num = (uint) value;
        while (num >= 0x80)
        {
            *destination++ = (byte) (num | 0x80);
            num >>= 7;
        }
        *destination++ = (byte) num;
    }
}
