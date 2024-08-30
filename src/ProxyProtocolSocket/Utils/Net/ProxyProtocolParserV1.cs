using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ProxyProtocolSocket.Utils.Net;

public class ProxyProtocolParserV1 : IProxyProtocolParser
{
    #region Constants
    private const string DELIMITER = "\r\n";
    private const char SEPARATOR = ' ';
    private const int MAX_HEADER_SIZE = 107;
    #endregion

    #region Members
    private readonly NetworkStream _stream;
    private readonly IPEndPoint _remoteEndpoint;
    private readonly byte[] _buffer;
    private int _bufferSize;

    private bool _hasParsed;
    private AddressFamily _addressFamily = AddressFamily.Unknown;
    private ProxyProtocolCommand _protocolCommand = ProxyProtocolCommand.Unknown;
    private IPEndPoint? _sourceEndpoint;
    private IPEndPoint? _destEndpoint;
    #endregion

    public ProxyProtocolParserV1(NetworkStream stream, IPEndPoint remoteEndpoint, byte[] buffer, int bufferSize)
    {
        #region Args checking
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (stream.CanRead != true)
        {
            throw new ArgumentException($"argument 'stream' is unreadable");
        }

        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (bufferSize > buffer.Length)
        {
            throw new ArgumentException($"argument '{nameof(bufferSize)}' is larger than '{nameof(buffer)}.Length'");
        }
        #endregion

        #region Filling members
        this._stream = stream;
        this._remoteEndpoint = remoteEndpoint ?? throw new ArgumentNullException(nameof(remoteEndpoint));
        this._buffer = buffer;
        this._bufferSize = bufferSize;
        #endregion
    }

    #region Public methods
    public async Task Parse()
    {
        if (this._hasParsed)
        {
            return;
        }

        this._hasParsed = true;
        Logger.Log($"[{this._remoteEndpoint}] parsing header...");

        #region Getting full header and do first check

        await this.GetFullHeader();
        if (ProxyProtocolSocketPlugin.Config.Settings.LogLevel == LogLevel.Debug)
        {
            Logger.Log($"[{this._remoteEndpoint}] header content: {Convert.ToHexString(this._buffer[..this._bufferSize])}");
        }

        var tokens = Encoding.ASCII.GetString(this._buffer[..(this._bufferSize - 2)]).Split(SEPARATOR);
        if (tokens.Length < 2)
        {
            throw new Exception("Unable to read AddressFamily and protocol");
        }

        #endregion

        #region Parse address family

        Logger.Log($"[{this._remoteEndpoint}] parsing address family...");
        var addressFamily = tokens[1] switch
        {
            "TCP4" => AddressFamily.InterNetwork,
            "TCP6" => AddressFamily.InterNetworkV6,
            "UNKNOWN" => AddressFamily.Unspecified,
            _ => throw new Exception("Invalid address family")
        };

        #endregion

        #region Do second check

        if (addressFamily == AddressFamily.Unspecified)
        {
            this._protocolCommand = ProxyProtocolCommand.Local;
            this._sourceEndpoint = this._remoteEndpoint;
            this._hasParsed = true;
            return;
        }

        if (tokens.Length < 6)
        {
            throw new Exception("Impossible to read ip addresses and ports as the number of tokens is less than 6");
        }

        #endregion

        #region Parse source and dest end point

        Logger.Log($"[{this._remoteEndpoint}] parsing endpoints...");
        IPEndPoint sourceEp;
        IPEndPoint destEp;
        try
        {
            // TODO: IP format validation
            var sourceAddr = IPAddress.Parse(tokens[2]);
            var destAddr = IPAddress.Parse(tokens[3]);
            var sourcePort = Convert.ToInt32(tokens[4]);
            var destPort = Convert.ToInt32(tokens[5]);
            sourceEp = new IPEndPoint(sourceAddr, sourcePort);
            destEp = new IPEndPoint(destAddr, destPort);
        }
        catch (Exception ex)
        {
            throw new Exception("Unable to parse ip addresses and ports", ex);
        }

        #endregion

        this._addressFamily = addressFamily;
        this._protocolCommand = ProxyProtocolCommand.Proxy;
        this._sourceEndpoint = sourceEp;
        this._destEndpoint = destEp;
    }

    public async Task<IPEndPoint?> GetSourceEndpoint()
    {
        await this.Parse();
        return this._sourceEndpoint;
    }

    public async Task<IPEndPoint?> GetDestEndpoint()
    {
        await this.Parse();
        return this._destEndpoint;
    }

    public async Task<AddressFamily> GetAddressFamily()
    {
        await this.Parse();
        return this._addressFamily;
    }

    public async Task<ProxyProtocolCommand> GetCommand()
    {
        await this.Parse();
        return this._protocolCommand;
    }
    #endregion

    #region Private methods
    private async Task GetFullHeader()
    {
        Logger.Log($"[{this._remoteEndpoint}] getting full header");
        for (var i = 7; i < MAX_HEADER_SIZE; i++) // Search after "PROXY" signature
        {
            if (await this.GetOneByteAtPosition(i) != DELIMITER[1])
            {
                continue;
            }

            if (await this.GetOneByteAtPosition(i - 1) != DELIMITER[0])
            {
                throw new Exception("Header must end with CRLF");
            }

            return;
        }
        throw new Exception("Failed to find any delimiter within the maximum header size of version 1");
    }

    private async Task GetBytesTillBufferSize(int size)
    {
        if (size <= this._bufferSize)
        {
            return;
        }

        await this.GetBytesFromStream(size - this._bufferSize);
    }

    private async Task GetBytesFromStream(int length)
    {
        if (this._bufferSize + length > this._buffer.Length)
        {
            throw new InternalBufferOverflowException();
        }

        while (length > 0)
        {
            if (!this._stream.DataAvailable)
            {
                throw new EndOfStreamException();
            }

            var count = await this._stream.ReadAsync(this._buffer.AsMemory(this._bufferSize, length));
            length -= count;
            this._bufferSize += count;
        }
    }

    private async Task<byte> GetOneByteAtPosition(int position)
    {
        await this.GetBytesTillBufferSize(position + 1);
        return this._buffer[position];
    }

    #endregion
}