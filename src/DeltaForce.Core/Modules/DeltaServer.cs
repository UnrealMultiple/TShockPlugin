using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Reflection;
using DeltaForce.Protocol.Interfaces;
using DeltaForce.Protocol.Packets;
using DeltaForce.Protocol.Processing;
using DeltaForce.Protocol.Serialization;
using DeltaForce.Core.Enitys;
using TShockAPI;

namespace DeltaForce.Core.Modules;

public class ClientConnection
{
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public TcpClient TcpClient { get; set; } = null!;
    public NetworkStream Stream => TcpClient.GetStream();
    public DateTime ConnectedAt { get; set; } = DateTime.Now;
    public int SessionId { get; set; }
}

public class DeltaServer(ServerEnity option)
{
    private readonly TcpListener _listener = new(System.Net.IPAddress.Any, option.Port);
    private readonly PacketSerializer _serializer = new();
    private readonly PacketProcessor _processor = new();
    
    private readonly ConcurrentDictionary<Guid, ClientConnection> _clientsById = new();
    private readonly ConcurrentDictionary<int, Guid> _sessionToClientId = new();
    private int _nextSessionId = 1;

    public async Task StartAsync()
    {
        _processor.RegisterHandlersFromAssembly(Assembly.GetExecutingAssembly());
        _listener.Start();
        TShock.Log.ConsoleInfo($"[DeltaServer] Started on port {option.Port}");

        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();
            _ = HandleClientAsync(client);
        }
    }

    private async Task HandleClientAsync(TcpClient tcpClient)
    {
        var endpoint = tcpClient.Client.RemoteEndPoint?.ToString() ?? "Unknown";
        TShock.Log.ConsoleInfo($"[DeltaServer] Client {endpoint} Connected");
        
        using var stream = tcpClient.GetStream();
        ClientConnection? clientInfo = null;
        
        try
        {
            while (true)
            {
                var lengthBytes = new byte[2];
                int read = await stream.ReadAsync(lengthBytes.AsMemory(0, 2));
                if (read < 2) break;

                var length = BitConverter.ToInt16(lengthBytes);

                var data = new byte[length];
                data[0] = lengthBytes[0];
                data[1] = lengthBytes[1];

                int totalRead = 2;
                while (totalRead < length)
                {
                    read = await stream.ReadAsync(data.AsMemory(totalRead, length - totalRead));
                    if (read == 0) break;
                    totalRead += read;
                }

                if (totalRead < length) break;

                using var ms = new MemoryStream(data);
                using var br = new BinaryReader(ms);
                var packet = _serializer.Deserialize(br);

                if (packet == null) continue;

                if (packet is ClientIdentityPacket identityPacket)
                {
                    clientInfo = await HandleClientIdentityAsync(identityPacket, tcpClient);
                    continue;
                }

                var clientId = clientInfo?.ClientId ?? Guid.Empty;
                TShock.Log.ConsoleInfo($"[DeltaServer] Received {packet.PacketID} from Client {clientId}");

                var response = _processor.Process(packet);

                if (response != null)
                {
                    await SendPacketAsync(stream, response);
                    TShock.Log.ConsoleInfo($"[DeltaServer] Sent {response.PacketID} to Client {clientId}");
                }
            }
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError($"[DeltaServer] Client {clientInfo?.ClientId} Disconnected: {ex.Message}");
        }
        finally
        {
            if (clientInfo != null)
            {
                _clientsById.TryRemove(clientInfo.ClientId, out _);
                _sessionToClientId.TryRemove(clientInfo.SessionId, out _);
            }
            tcpClient.Close();
        }
    }

    private async Task<ClientConnection> HandleClientIdentityAsync(ClientIdentityPacket packet, TcpClient tcpClient)
    {
        var sessionId = _nextSessionId++;
        
        var clientInfo = new ClientConnection
        {
            ClientId = packet.ClientId,
            ClientName = packet.ClientName,
            TcpClient = tcpClient,
            SessionId = sessionId
        };

        _clientsById[packet.ClientId] = clientInfo;
        _sessionToClientId[sessionId] = packet.ClientId;

        TShock.Log.ConsoleInfo($"[DeltaServer] Client registered: {packet.ClientName} ({packet.ClientId}), Session: {sessionId}");

        var response = new ClientIdentityResponsePacket
        {
            RequestId = Guid.NewGuid(),
            Success = true,
            Message = "Client registered successfully",
            ClientId = packet.ClientId,
            SessionId = sessionId
        };

        await SendPacketAsync(clientInfo.Stream, response);

        return clientInfo;
    }

    private async Task SendPacketAsync(NetworkStream stream, INetPacket packet)
    {
        var buffer = _serializer.Serialize(packet);
        await stream.WriteAsync(buffer);
    }

    public async Task<TResponse?> RequestAsync<TRequest, TResponse>(TRequest request, int timeoutMs = 5000)
        where TRequest : IRequestPacket
        where TResponse : class, IResponsePacket
    {
        var response = await _processor.RequestAsync<TRequest, TResponse>(
            request,
            BroadcastAsync,
            timeoutMs
        );
        return response;
    }

    public async Task<bool> PushToClientAsync(Guid clientId, INetPacket packet)
    {
        if (!_clientsById.TryGetValue(clientId, out var client))
        {
            TShock.Log.ConsoleWarn($"[DeltaServer] Client {clientId} not found for push");
            return false;
        }

        try
        {
            await SendPacketAsync(client.Stream, packet);
            TShock.Log.ConsoleInfo($"[DeltaServer] Pushed {packet.PacketID} to Client {clientId}");
            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError($"[DeltaServer] Failed to push to Client {clientId}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> PushToSessionAsync(int sessionId, INetPacket packet)
    {
        if (!_sessionToClientId.TryGetValue(sessionId, out var clientId))
        {
            TShock.Log.ConsoleWarn($"[DeltaServer] Session {sessionId} not found for push");
            return false;
        }

        return await PushToClientAsync(clientId, packet);
    }


    public async Task<int> BroadcastAsync(INetPacket packet)
    {
        var tasks = _clientsById.Values.Select(client => PushToClientAsync(client.ClientId, packet)).ToArray();
        var results = await Task.WhenAll(tasks);
        return results.Count(r => r);
    }

    public IReadOnlyCollection<ClientConnection> GetConnectedClients() => [.. _clientsById.Values];


    public ClientConnection? GetClient(Guid clientId)
    {
        _clientsById.TryGetValue(clientId, out var client);
        return client;
    }

    public byte[] SerializePacket(INetPacket packet) => _serializer.Serialize(packet);
}
