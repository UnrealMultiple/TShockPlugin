using System.Linq;
using System.Security.Cryptography;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace ServerTools;

public class ModifyClientDetect
{
    private static readonly byte[] _data =
    [
        0xA3, 0x5C, 0x2E, 0x9F, 0x1A, 0x7B, 0xD4, 0xE6,
        0x38, 0xC9, 0xF2, 0x4D, 0x6B, 0x8A, 0x1C, 0x5F,
        0x9E, 0x27, 0xB0, 0x43, 0x7C, 0xFD, 0x8D, 0x52,
        0xE4, 0x3A, 0x6C, 0x18, 0x5B, 0x8F, 0xCA, 0x2D,
        0xCC, 0xD0, 0x34, 0xA4, 0xF8, 0x36, 0x4B, 0x9A,
        0x1F, 0x95, 0xC8, 0x73, 0x2C, 0xE7, 0x46, 0x5D,
        0xAA, 0x73, 0xCE, 0xD3, 0x5F, 0x26, 0x9B, 0x41,
        0x87, 0x7F, 0x05, 0x61, 0x30, 0x8C, 0xA5, 0xDB,
        0x00, 0x00, 0x00, 0x00, 0xC9, 0x00, 0x00, 0x00,
        0xAE, 0x40, 0xFE, 0xFF, 0x00, 0x00, 0x00, 0x00,
        0xBE, 0xB4, 0xE2, 0xFF, 0x00, 0x00, 0x00, 0x00
    ];

    private static readonly byte[] Salt;
    private static readonly byte[] TargetIntHash;
    private static readonly byte[] TargetCoordHash;

    static ModifyClientDetect()
    {
        Salt = new byte[32];
        for (var i = 0; i < 32; i++)
        {
            Salt[i] = (byte) (_data[i] ^ _data[i + 32]);
        }

        var a = BitConverter.ToInt32(_data, 64);
        var b = BitConverter.ToInt32(_data, 68);
        var c = (a ^ b) & 0xFF;

        var xRaw = BitConverter.ToInt32(_data, 72);
        var xMask = BitConverter.ToInt32(_data, 76);
        var d = xRaw ^ xMask;
        var yRaw = BitConverter.ToInt32(_data, 80);
        var yMask = BitConverter.ToInt32(_data, 84);
        var e = yRaw ^ yMask;

        TargetIntHash = ComputeHash(BitConverter.GetBytes(c));
        var coordData = new byte[8];
        Buffer.BlockCopy(BitConverter.GetBytes(d), 0, coordData, 0, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(e), 0, coordData, 4, 4);
        TargetCoordHash = ComputeHash(coordData);
    }

    public static byte[] ComputeHash(byte[] data)
    {
        using var hmac = new HMACSHA256(Salt);
        return hmac.ComputeHash(data);
    }

    private static bool IsIntMatch(int value, byte[] targetHash)
    {
        return ComputeHash(BitConverter.GetBytes(value)).SequenceEqual(targetHash);
    }

    private static bool IsCoordMatch(int x, int y, byte[] targetHash)
    {
        var data = new byte[8];
        Buffer.BlockCopy(BitConverter.GetBytes(x), 0, data, 0, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(y), 0, data, 4, 4);
        return ComputeHash(data).SequenceEqual(targetHash);
    }

    public static void CheckModify(MessageBuffer instance, int start, int length, int value)
    {
        var player = TShock.Players[instance.whoAmI];
        if (player == null)
        {
            return;
        }

        var isCheater = false;
        if (value == MessageID.PlayerControls)
        {
            var oldPos = instance.reader.BaseStream.Position;
            try
            {
                instance.reader.BaseStream.Position = start + 1;
                instance.reader.BaseStream.Position += 2;
                BitsByte b25 = instance.reader.ReadByte();
                BitsByte b26 = instance.reader.ReadByte();
                BitsByte b27 = instance.reader.ReadByte();
                if (b27[5])
                {
                    var optional = (b25[2] ? 8 : 0) + (b25[7] ? 2 : 0) + (b26[6] ? 16 : 0);
                    instance.reader.BaseStream.Position += 9 + optional;
                    var nct = instance.reader.ReadVector2();
                    if (IsCoordMatch((int) nct.X, (int) nct.Y, TargetCoordHash))
                    {
                        isCheater = true;
                    }
                }
            }
            finally { instance.reader.BaseStream.Position = oldPos; }
        }

        if (!isCheater && value != MessageID.PlayerControls && IsIntMatch(value, TargetIntHash)) isCheater = true;

        if (isCheater)
        {
            var text = $"[警告]玩家 {player.Name} 使用修改后的客户端进入服务器！";
            TShock.Log.ConsoleWarn(text);
            TShock.Utils.Broadcast(text, Microsoft.Xna.Framework.Color.Red);
            if (Config.Instance.KickCheater)
            {
                player.Kick(Config.Instance.KickCheaterText);
            }
        }
    }
}
