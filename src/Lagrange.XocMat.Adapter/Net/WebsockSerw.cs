using Lagrange.XocMat.Adapter.Setting;
using System.Net.WebSockets;
using TShockAPI;

namespace Lagrange.XocMat.Adapter.Net;

public class WebSocketServices : IDisposable
{
    #nullable disable
    private ClientWebSocket _client;
    #nullable enable

    public event Action<byte[]>? OnMessage;

    public event Action? OnConnect;

    private bool _isRun;

    public async void SendMessage(MemoryStream stream, CancellationToken token = default)
    {
        await this.SendMessage(stream.ToArray(), token);
    }

    public async Task SendMessage(byte[] message, CancellationToken token = default)
    {
        if (this._client?.State == WebSocketState.Open)
        {
            try
            {
                await this._client.SendAsync(message, WebSocketMessageType.Binary, true, token);
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"发送消息时出错:{ex.Message}");
            }
        }
    }

    public void Start()
    {
        this._isRun = true;
        Task.Run(async () =>
        {
            var count = 1;
            while (this._isRun)
            {
                try
                {
                    this._client = new();
                    this._client.ConnectAsync(new Uri($"ws://{Config.Instance.SocketConfig.IP}:{Config.Instance.SocketConfig.Port}/"), CancellationToken.None).Wait();
                    if (this._client.State == WebSocketState.Open)
                    { 
                        OnConnect?.Invoke();
                        await this.ReceiveLoop();
                    }
                }
                catch
                {
                    TShock.Log.ConsoleError($"[Lagrange.XocMat.Adapter]({count}) 未连接至XocMat机器人，正在进行连接..");
                    this._client.Dispose();
                }
                count++;
                await Task.Delay(5000);
            }
        });
    }

    private async Task ReceiveLoop()
    {
        while (this._isRun)
        {
            var buffer = new ArraySegment<byte>(new byte[1024]);
            WebSocketReceiveResult result = new(0, WebSocketMessageType.Binary, false);
            var Message = new List<byte[]>();
            while (!result.EndOfMessage)
            {
                //接收消息
                result = await this._client.ReceiveAsync(buffer, CancellationToken.None);
                Message.Add([.. buffer.Take(result.Count)]);
            }
            var temp = Array.Empty<byte>();
            Message.ForEach(u => temp = [.. temp, .. u]);
            buffer = new ArraySegment<byte>(temp);
            OnMessage?.Invoke([.. buffer]);
        }
    }

    public void Dispose()
    {
        this._isRun = false;
        this._client.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
        this._client.Dispose();
        GC.SuppressFinalize(this);
    }
}