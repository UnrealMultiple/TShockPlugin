using System.Net;
using System.Net.Sockets;
using System.Text;
using TrProtocol;

namespace Dummy;

public class TrClient
{
    private TcpClient client = null!;


    private BinaryReader? br;
    private BinaryWriter? bw;
    private readonly PacketSerializer mgr = new(true);

    public void Connect(string hostname, int port)
    {
        this.client = new TcpClient();
        this.client.Connect(hostname, port);
        this.br = new BinaryReader(this.client.GetStream());
        this.bw = new BinaryWriter(this.client.GetStream());
    }

    public void Connect(IPEndPoint server, IPEndPoint? proxy = null)
    {
        if (proxy == null)
        {
            this.client.Connect(server);
            this.br = new BinaryReader(this.client.GetStream());
            this.bw = new BinaryWriter(this.client.GetStream());
            return;
        }
        this.client = new TcpClient();
        this.client.Connect(proxy);

        //Console.WriteLine("Proxy connected to " + proxy.ToString());
        var encoding = new UTF8Encoding(false, true);
        using var sw = new StreamWriter(this.client.GetStream(), encoding, 4096, true) { NewLine = "\r\n" };
        using var sr = new StreamReader(this.client.GetStream(), encoding, false, 4096, true);
        sw.WriteLine($"CONNECT {server} HTTP/1.1");
        sw.WriteLine("User-Agent: Java/1.8.0_192");
        sw.WriteLine($"Host: {server}");
        sw.WriteLine("Accept: text/html, image/gif, image/jpeg, *; q=.2, */*; q=.2");
        sw.WriteLine("Proxy-Connection: keep-alive");
        sw.WriteLine();
        sw.Flush();

        var resp = sr.ReadLine();
        Console.WriteLine("Proxy connection; " + resp);
        if (resp is null || !resp.StartsWith("HTTP/1.1 200"))
        {
            throw new Exception();
        }

        while (true)
        {
            resp = sr.ReadLine();
            if (string.IsNullOrEmpty(resp))
            {
                break;
            }
        }
    }

    public void Close()
    {
        if (this.client.Connected)
        {
            this.client.Close();
        }

    }

    public void KillServer()
    {
        this.client.GetStream().Write(new byte[] { 0, 0 }, 0, 2);
    }
    public Packet Receive()
    {
        return this.mgr.Deserialize(this.br);
    }
    public void Send(Packet packet)
    {
        this.bw?.Write(this.mgr.Serialize(packet));
    }
}
