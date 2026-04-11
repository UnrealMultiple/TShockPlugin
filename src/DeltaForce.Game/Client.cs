using DeltaForce.Protocol.Interfaces;
using DeltaForce.Protocol.Packets;
using DeltaForce.Protocol.Processing;
using DeltaForce.Protocol.Serialization;
using System.Net.Sockets;
using System.Reflection;
using TShockAPI;

namespace DeltaForce.Game;

public class Client
{
    private TcpClient _client = new();
    private NetworkStream? _stream;
    private readonly PacketSerializer _serializer = new();
    private readonly PacketProcessor _processor = new();
    private CancellationTokenSource _cts = new();

    private readonly object _lock = new();

    public Guid ClientId { get; private set; } = Guid.NewGuid();
    public string ClientName { get; set; } = "TestClient";
    public int SessionId { get; private set; }
    public bool IsConnected { get; private set; }

    private string? _serverAddress;
    private int _serverPort;

    private const int MaxReconnectAttempts = 10;
    private const int InitialReconnectDelayMs = 1000;
    private const int MaxReconnectDelayMs = 30000;

    public Client()
    {
        _processor.RegisterHandlersFromAssembly(Assembly.GetExecutingAssembly());
    }

    public async Task ConnectAsync(string address, int port)
    {
        _serverAddress = address;
        _serverPort = port;

        await ConnectInternalAsync();
    }

    private async Task ConnectInternalAsync()
    {
        try
        {
            lock (_lock)
            {
                if (_client.Connected)
                {
                    _stream?.Close();
                    _client.Close();
                }

                _client = new TcpClient();
                _cts = new CancellationTokenSource();
            }

            await _client.ConnectAsync(_serverAddress!, _serverPort);
            _stream = _client.GetStream();

            _ = ReceiveLoopAsync(_cts.Token);

            await SendClientIdentityAsync();

            IsConnected = true;
            Console.WriteLine($"[Client] 已连接到服务器 {_serverAddress}:{_serverPort}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Client] 连接失败: {ex.Message}");
            IsConnected = false;
            _ = StartReconnectAsync();
        }
    }

    private async Task StartReconnectAsync()
    {
        int attempt = 0;
        int delay = InitialReconnectDelayMs;

        while (!IsConnected && attempt < MaxReconnectAttempts)
        {
            attempt++;
            Console.WriteLine($"[Client] 第 {attempt} 次尝试重连... ({delay}ms 后)");

            await Task.Delay(delay);

            try
            {
                lock (_lock)
                {
                    if (_client.Connected)
                    {
                        _stream?.Close();
                        _client.Close();
                    }

                    _client = new TcpClient();
                    _cts = new CancellationTokenSource();
                }

                await _client.ConnectAsync(_serverAddress!, _serverPort);
                _stream = _client.GetStream();

                _ = ReceiveLoopAsync(_cts.Token);

                await SendClientIdentityAsync();

                IsConnected = true;
                Console.WriteLine($"[Client] 重连成功！已连接到服务器 {_serverAddress}:{_serverPort}");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Client] 重连失败: {ex.Message}");
                IsConnected = false;

                delay = Math.Min(delay * 2, MaxReconnectDelayMs);
            }
        }

        if (!IsConnected)
        {
            Console.WriteLine($"[Client] 重连失败，已达到最大尝试次数 ({MaxReconnectAttempts})");
        }
    }

    private async Task SendClientIdentityAsync()
    {
        var identityPacket = new ClientIdentityPacket
        {
            ClientId = ClientId,
            ClientName = ClientName
        };

        await SendPacketAsync(identityPacket);
        Console.WriteLine($"[Client] 已发送身份验证: {ClientName} ({ClientId})");
    }

    private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            var buffer = new byte[4096];

            while (!cancellationToken.IsCancellationRequested)
            {
                var lengthBytes = new byte[2];
                int read = await _stream!.ReadAsync(lengthBytes.AsMemory(0, 2), cancellationToken);
                if (read < 2)
                {
                    Console.WriteLine("[Client] 连接已断开，读取长度失败");
                    break;
                }

                var length = BitConverter.ToInt16(lengthBytes);

                var data = new byte[length];
                data[0] = lengthBytes[0];
                data[1] = lengthBytes[1];

                int totalRead = 2;
                while (totalRead < length)
                {
                    read = await _stream.ReadAsync(data.AsMemory(totalRead, length - totalRead), cancellationToken);
                    if (read == 0)
                    {
                        Console.WriteLine("[Client] 连接已断开，读取数据失败");
                        break;
                    }
                    totalRead += read;
                }

                if (totalRead < length) break;

                using var ms = new MemoryStream(data);
                using var br = new BinaryReader(ms);
                var packet = _serializer.Deserialize(br);

                if (packet == null) continue;

                Console.WriteLine($"[Client] 收到数据包: {packet.PacketID}");

                var response = _processor.Process(packet);
                if (response != null)
                {
                    await SendPacketAsync(response);
                    Console.WriteLine($"[Client] 发送响应: {response.PacketID}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("[Client] 接收循环已取消");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Client] 接收错误: {ex.Message}");
        }
        finally
        {
            IsConnected = false;
            Console.WriteLine("[Client] 连接已断开，启动重连机制...");
            _ = StartReconnectAsync();
        }
    }

    public async Task SendPacketAsync(INetPacket packet)
    {
        try
        {
            lock (_lock)
            {
                if (!IsConnected || _stream == null)
                {
                    Console.WriteLine("[Client] 无法发送数据包，未连接到服务器");
                    return;
                }
            }

            var buffer = _serializer.Serialize(packet);
            await _stream.WriteAsync(buffer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Client] 发送数据包失败: {ex.Message}");
            IsConnected = false;
            _ = StartReconnectAsync();
        }
    }

    public async Task<TResponse?> RequestAsync<TRequest, TResponse>(TRequest request, int timeoutMs = 5000)
        where TRequest : IRequestPacket
        where TResponse : class, IResponsePacket
    {
        if (!IsConnected)
        {
            Console.WriteLine("[Client] 无法发送请求，未连接到服务器");
            return null;
        }

        try
        {
            var response = await _processor.RequestAsync<TRequest, TResponse>(
                request,
                SendPacketAsync,
                timeoutMs
            );
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Client] 请求失败: {ex.Message}");
            return null;
        }
    }

    public void Disconnect()
    {
        _cts.Cancel();
        _stream?.Close();
        _client.Close();
        IsConnected = false;
        Console.WriteLine("[Client] 已断开连接");
    }
}
