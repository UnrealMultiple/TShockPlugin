using Microsoft.Xna.Framework;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ID;

namespace MusicPlayer;

internal static class NetMessageSender
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsNotActive(int remoteClient)
    {
        return !Netplay.Clients[remoteClient].IsActive;
    }

    private static void SendData(int remoteClient, byte[] data)
    {
        NetMessage.buffer[remoteClient].spamCount++;
        Netplay.Clients[remoteClient].Socket.AsyncSend(data, 0, data.Length, Netplay.Clients[remoteClient].ServerWriteCallBack, null);
    }
    public static void SyncEquipment(int remoteClient, byte index, short itemIndex, short stack, byte prefix, short type)
    {
        if (IsNotActive(remoteClient))
        {
            return;
        }
        var data = new byte[11];
        var dataSpan = data.AsSpan();
        //BinaryPrimitives.WriteUInt16LittleEndian(dataSpan, 11);
        data[0] = 11;
        data[2] = MessageID.SyncEquipment;
        data[3] = index;
        BinaryPrimitives.WriteInt16LittleEndian(dataSpan[4..], itemIndex);
        BinaryPrimitives.WriteInt16LittleEndian(dataSpan[6..], stack);
        data[8] = prefix;
        BinaryPrimitives.WriteInt16LittleEndian(dataSpan[9..], type);
        SendData(remoteClient, data);
    }
    public static void PlayerControls(int remoteClient, byte index, Vector2 position)
    {
        if (IsNotActive(remoteClient))
        {
            return;
        }
        var data = new byte[17];
        var dataSpan = data.AsSpan();
        //BinaryPrimitives.WriteUInt16LittleEndian(dataSpan, 17);
        data[0] = 17;
        data[2] = MessageID.PlayerControls;
        data[3] = index;
        BinaryPrimitives.WriteSingleLittleEndian(dataSpan[9..], position.X);
        BinaryPrimitives.WriteSingleLittleEndian(dataSpan[13..], position.Y);
        SendData(remoteClient, data);
    }
    public static void PlayerBuff(int remoteClient, byte index, params ushort[] buffTypes)
    {
        if (IsNotActive(remoteClient))
        {
            return;
        }
        var data = new byte[3 + 1 + (Player.maxBuffs * 2)];
        var dataSpan = data.AsSpan();
        BinaryPrimitives.WriteUInt16LittleEndian(dataSpan, (ushort) data.Length);
        data[2] = MessageID.PlayerBuffs;
        data[3] = index;
        var offset = 4;
        var count = Math.Min(Player.maxBuffs, buffTypes.Length);
        for (var i = 0; i < count; i++)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(dataSpan[offset..], buffTypes[i]);
            offset += sizeof(ushort);
        }
        SendData(remoteClient, data);
    }
    public static void PlayerActive(int remoteClient, byte index, bool active)
    {
        if (IsNotActive(remoteClient))
        {
            return;
        }
        var data = new byte[5];
        //BinaryPrimitives.WriteUInt16LittleEndian(data, 5);
        data[0] = 5;
        data[2] = MessageID.PlayerActive;
        data[3] = index;
        if (active)
        {
            data[4] = 1;
        }
        SendData(remoteClient, data);
    }
    public static void InstrumentSound(int remoteClient, byte index, float pitch)
    {
        if (IsNotActive(remoteClient))
        {
            return;
        }
        var data = new byte[8];
        //BinaryPrimitives.WriteUInt16LittleEndian(data, 8);
        data[0] = 8;
        data[2] = MessageID.InstrumentSound;
        data[3] = index;
        BinaryPrimitives.WriteSingleLittleEndian(data.AsSpan()[4..], pitch);
        SendData(remoteClient, data);
    }
}