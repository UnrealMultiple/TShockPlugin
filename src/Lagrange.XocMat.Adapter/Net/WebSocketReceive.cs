using System.Net.WebSockets;
using TShockAPI;

namespace MorMorAdapter.Net;

public class WebSocketReceive
{
    public static ClientWebSocket ClientWebSocket { get; private set; }

    public static event Action<byte[]> OnMessage;

    public static event Action OnConnect;

    public static bool CanRun = true;

    public static async void SendMessage(MemoryStream stream)
    {
        await SendMessage(stream.ToArray());
    }
    public static async Task SendMessage(byte[] message)
    {
        if (ClientWebSocket.State == WebSocketState.Open)
        {
            try
            {
                await ClientWebSocket.SendAsync(message, WebSocketMessageType.Binary, true, default);
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"发送消息时出错:{ex.Message}");
            }
        }
    }



    public static void Start(string Host, int Port)
    {

        Task.Run(async () =>
        {
            var count = 1;
            while (CanRun)
            {
                try
                {
                    ClientWebSocket = new();
                    ClientWebSocket.ConnectAsync(new Uri($"ws://{Host}:{Port}/momo?name={Plugin.Config.SocketConfig.ServerName}"), CancellationToken.None).Wait();
                    OnConnect.Invoke();
                    while (true)
                    {
                        var buffer = new ArraySegment<byte>(new byte[1024]);
                        WebSocketReceiveResult result = new(0, WebSocketMessageType.Binary, false);
                        var Message = new List<byte[]>();
                        while (!result.EndOfMessage)
                        {
                            //接收消息
                            result = await ClientWebSocket.ReceiveAsync(buffer, CancellationToken.None);
                            Message.Add(buffer.Take(result.Count).ToArray());
                        }
                        var temp = new byte[0];
                        Message.ForEach(u => temp = temp.Concat(u).ToArray());
                        buffer = new ArraySegment<byte>(temp);
                        OnMessage(buffer.ToArray());
                    }

                }
                catch
                {
                    ClientWebSocket.Dispose();
                    TShock.Log.ConsoleError($"[Lagrange.XocMat.Adapter]({count}) 未连接至MorMor机器人，正在进行连接..");

                }
                count++;
                await Task.Delay(5000);
            }
        });
    }


}
