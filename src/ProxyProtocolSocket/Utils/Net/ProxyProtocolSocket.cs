using ProxyProtocolSocket.Utils.Exts;
using System.Net;
using System.Net.Sockets;
using Terraria;
using Terraria.Localization;
using Terraria.Net;
using Terraria.Net.Sockets;

namespace ProxyProtocolSocket.Utils.Net;

public class ProxyProtocolSocket : TcpSocket, ISocket
{
    private static readonly object _clientJoinLock = new();

    public ProxyProtocolSocket()
    {
    }

    public ProxyProtocolSocket(TcpClient tcpClient) : base(tcpClient)
    {
    }

    public ProxyProtocolSocket(TcpClient tcpClient, IPEndPoint remoteEndpoint) : base(tcpClient)
    {
        this._remoteAddress = new TcpAddress(remoteEndpoint.Address, remoteEndpoint.Port);
    }

    // Override to prevent unexpected situation
    void ISocket.Connect(RemoteAddress address)
    {
        throw new NotImplementedException();
    }

    bool ISocket.StartListening(SocketConnectionAccepted callback)
    {
        var listeningAddress = IPAddress.Any;
        if (Program.LaunchParameters.TryGetValue("-ip", out var programIpParam) &&
            !IPAddress.TryParse(programIpParam, out listeningAddress))
        {
            listeningAddress = IPAddress.Any;
        }

        this._isListening = true;
        lock (_clientJoinLock)
        {
            this._listenerCallback = callback;
        }
        this._listener ??= new TcpListener(listeningAddress, Netplay.ListenPort);

        try
        {
            this._listener.Start();
        }
        catch (Exception ex)
        {
            Logger.Log($"Failed to start the TcpListener\n{ex}", LogLevel.Error);
            return false;
        }

        new Thread(this.ListenLoop)
        {
            IsBackground = true,
            Name = "Proxy Protocol Listen Thread"
        }.Start();
        return true;
    }

    public new void ListenLoop()
    {
        while (this._isListening && !Netplay.Disconnect)
        {
            try
            {
                var client = this._listener.AcceptTcpClient();
                Task.Run(async () =>
                {
                    try
                    {
                        Logger.Log($"[{client.Client.RemoteEndPoint}] proxy connection accepted!");
                        var ns = client.GetStream();

                        Logger.Log($"[{client.Client.RemoteEndPoint}] waiting for any data from client...");
                        await TaskExt.WaitUntilAsync(() => ns.DataAvailable,
                            ProxyProtocolSocketPlugin.Config.Settings.TimeOut);

                        Logger.Log($"[{client.Client.RemoteEndPoint}] data received! processing...");
                        var pp = new ProxyProtocol(ns, (IPEndPoint) client.Client.RemoteEndPoint!);

                        Logger.Log($"[{client.Client.RemoteEndPoint}] checking proxy protocol version...");
                        var version = await pp.GetVersion();
                        if (version == ProxyProtocolVersion.Unknown)
                        {
                            Logger.Log(
                                $"[{client.Client.RemoteEndPoint}] connection rejected due to an unknown version of proxy protocol",
                                LogLevel.Info);
                            client.Close();
                            return;
                        }

                        Logger.Log($"[{client.Client.RemoteEndPoint}] proxy protocol version is {version:G}");
                        Logger.Log($"[{client.Client.RemoteEndPoint}] parsing header...");
                        await pp.Parse();

                        Logger.Log($"[{client.Client.RemoteEndPoint}] getting real client ip");
                        var realClientIp = (await pp.GetSourceEndpoint())!;
                        ISocket ppSocket = new ProxyProtocolSocket(client, realClientIp);
                        lock (_clientJoinLock)
                        {
                            Logger.Log($"{realClientIp} is connecting through proxy {client.Client.RemoteEndPoint} (proxy protocol {version:G})",
                                LogLevel.Info);
                            Console.WriteLine(Language.GetTextValue("Net.ClientConnecting",
                                ppSocket.GetRemoteAddress()));
                            this._listenerCallback(ppSocket);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Connection {client.Client.RemoteEndPoint} caused\n{ex}", LogLevel.Warning);
                        client.Close();
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception occur while accepting tcp client\n{ex}", LogLevel.Warning);
            }
        }

        this._listener.Stop();
    }
}