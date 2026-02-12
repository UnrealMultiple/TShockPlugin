using System;
using System.Collections.Generic;
using System.IO;
using CaiBotLite.Enums;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace CaiBotLite.Services;

public static class WebsocketManager
{
    public static ClientWebSocket? WebSocket;

    private const string BotServerUrl = "api.terraria.ink:22338";
    //private const string BotServerUrl = "127.0.0.1:8080";
    internal static bool IsWebsocketConnected => WebSocket?.State == WebSocketState.Open;
    private static bool _isStopWebsocket;

    public static void Init()
    {
        Task.Factory.StartNew(StartCaiApi, TaskCreationOptions.LongRunning);
        Task.Factory.StartNew(StartHeartBeat, TaskCreationOptions.LongRunning);
        _isStopWebsocket = false;
    }

    public static void StopWebsocket()
    {
        _isStopWebsocket = true;
        WebSocket?.Dispose();
    }

    private static async Task? StartHeartBeat()
    {
        while (!_isStopWebsocket)
        {
            await Task.Delay(TimeSpan.FromSeconds(60));
            try
            {
                if (WebSocket?.State == WebSocketState.Open)
                {
                    var packetWriter = new PackageWriter(PackageType.Heartbeat, false, null);
                    packetWriter.Send();
                }
            }
            catch
            {
                TShock.Log.ConsoleInfo("[CaiBotLite]心跳包发送失败!");
            }
        }
    }

    private static async Task? StartCaiApi()
    {
        while (!_isStopWebsocket)
        {
            try
            {
                WebSocket = new ClientWebSocket();
                while (string.IsNullOrEmpty(Config.Settings.Token))
                {
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    HttpClient client = new ();
                    client.Timeout = TimeSpan.FromSeconds(5.0);
                    var response = await client.GetAsync($"https://{BotServerUrl}/server/token/{CaiBotLite.InitCode}");
                    if (response.StatusCode != HttpStatusCode.OK || Config.Settings.Token != "")
                    {
                        continue;
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseBody);
                    var token = json["token"]!.ToString();
                    var groupOpenId = json["group_open_id"]!.ToString();
                    Config.Settings.Token = token;
                    Config.Settings.GroupOpenId = groupOpenId;
                    Config.Settings.Write();
                    TShock.Log.ConsoleInfo("[CaiBotLite]被动绑定成功!");
                }

                WebSocket.Options.SetRequestHeader("authorization", $"Bearer {Config.Settings.Token}");
                await WebSocket.ConnectAsync(new Uri($"wss://{BotServerUrl}/server/ws/{Config.Settings.GroupOpenId}/tshock/"), CancellationToken.None);


                new PackageWriter(PackageType.Hello, false, null)
                    .Write("server_core_version", TShock.VersionNum.ToString())
                    .Write("plugin_version", CaiBotLite.VersionNum)
                    .Write("game_version", Main.versionNumber)
                    .Write("enable_whitelist", Config.Settings.WhiteList)
                    .Write("system", RuntimeInformation.RuntimeIdentifier)
                    .Write("server_name", TShock.Config.Settings.UseServerName ? TShock.Config.Settings.ServerName : Main.worldName)
                    .Write("settings", new Dictionary<string, object>())
                    .Send();

                TShock.Log.ConsoleInfo("[CaiBotLite]Bot连接成功...");

                while (true)
                {
                    var buffer = new byte[1024];
                    var memoryStream = new MemoryStream();

                    WebSocketReceiveResult result;
                    do 
                    {
                        result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        await memoryStream.WriteAsync(buffer.AsMemory(0, result.Count));
                    } 
                    while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        // 连接关闭时获取原因
                        await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        var statusCode = (int) result.CloseStatus!;
                        switch (statusCode)
                        {
                            case 4003:
                                Config.Settings.Token = "";
                                Config.Settings.Write();
                                TShock.Log.ConsoleError("[CaiBotLite]服务器认证失败, 请重新绑定!");
                                TShock.Log.ConsoleError($"原因({statusCode}): {result.CloseStatusDescription}");
                                CaiBotLite.GenBindCode(null);
                                break;
                            default:
                                TShock.Log.ConsoleError("[CaiBotLite]Bot主动断开连接!");
                                TShock.Log.ConsoleError($"原因({statusCode}): {result.CloseStatusDescription}");
                                break;
                        }

                        break;
                    }

                    var receivedData = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (CaiBotLite.DebugMode)
                    {
                        TShock.Log.ConsoleInfo($"[CaiBotLite]收到BOT数据包: {receivedData}");
                    }

                    await CaiBotApi.HandleMessageAsync(receivedData);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleInfo("[CaiBotLite]Bot断开连接...");
                if (!_isStopWebsocket)
                {
                    TShock.Log.ConsoleError(ex.ToString());
                }
            }

            await Task.Delay(5000);
        }
    }
}