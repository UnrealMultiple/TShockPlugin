using System;
using CaiBotLite.Enums;
using CaiBotLite.Moulds;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using TShockAPI;

namespace CaiBotLite.Services;

[Serializable]
public class PackageWriter(PackageType packageType, bool isRequest, string? requestId)
{
    private static bool Debug => CaiBotLite.DebugMode;
    public Package Package = new (Direction.ToBot, packageType, isRequest, requestId);

    public PackageWriter Write(string key, object value)
    {
        this.Package.Payload.Add(key, value);
        return this;
    }

    public void Send()
    {
        try
        {
            var message = this.Package.ToJson();
            if (Debug)
            {
                TShock.Log.ConsoleInfo($"[CaiBotLite]发送BOT数据包：{message}");
            }

            var messageBytes = Encoding.UTF8.GetBytes(message);
            _ = WebsocketManager.WebSocket?.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true,
                CancellationToken.None);
        }
        catch (Exception e)
        {
            TShock.Log.ConsoleInfo($"[CaiBotLite]发送数据包时发生错误：{e}");
        }
    }
}