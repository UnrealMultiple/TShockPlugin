using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ProxyProtocolSocket.Utils.Net;

#region Enums
public enum ProxyProtocolVersion
{
    V1,
    V2,
    Unknown
}

public enum ProxyProtocolCommand
{
    Local,
    Proxy,
    Unknown
}
#endregion

public class ProxyProtocol
{
    #region Constants
    private static readonly byte[] V1_SIGNATURE = Encoding.ASCII.GetBytes("PROXY");
    private static readonly byte[] V2_OR_ABOVE_SIGNATURE = { 0x0D, 0x0A, 0x0D, 0x0A, 0x00, 0x0D, 0x0A, 0x51, 0x55, 0x49, 0x54, 0x0A };
    private const int V2_OR_ABOVE_ID_LENGTH = 16;
    private const int MAX_BUFFER_SIZE = 232;
    #endregion

    #region Members
    private readonly NetworkStream _stream;
    private readonly IPEndPoint _remoteEndpoint;

    private readonly byte[] _buffer = new byte[MAX_BUFFER_SIZE];
    // i.e. the end of _buffer
    // _buffer[_bufferSize - 1] is the last byte read from the stream
    private int _bufferSize = 0;

    private bool _isParserCached = false;
    private IProxyProtocolParser? _cachedParser = null;
    private ProxyProtocolVersion? _protocolVersion = null;
    #endregion

    public ProxyProtocol(NetworkStream stream, IPEndPoint remoteEndpoint)
    {
        #region Args checking
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (stream.CanRead != true)
        {
            throw new ArgumentException("argument 'stream' is unreadable");
        }
        #endregion

        #region Filling members
        this._stream = stream;
        this._remoteEndpoint = remoteEndpoint ?? throw new ArgumentNullException(nameof(remoteEndpoint));
        #endregion
    }

    #region Public methods
    public async Task Parse()
    {
        var parser = await this.GetParser();
        if (parser == null)
        {
            return;
        }

        Logger.Log($"[{this._remoteEndpoint}] calling {parser.GetType().Name}.Parse()");
        await parser.Parse();
    }

    public async Task<IPEndPoint?> GetSourceEndpoint()
    {
        var parser = await this.GetParser();
        return parser == null ? null : await parser.GetSourceEndpoint();
    }

    public async Task<IPEndPoint?> GetDestEndpoint()
    {
        var parser = await this.GetParser();
        return parser == null ? null : await parser.GetDestEndpoint();
    }

    public async Task<AddressFamily> GetAddressFamily()
    {
        var parser = await this.GetParser();
        // return Unknown when the header is unparseable
        // return Unspecified when the address family written in the header is "UNKNOWN"
        return parser == null ? AddressFamily.Unknown : await parser.GetAddressFamily();
    }

    public async Task<ProxyProtocolCommand> GetCommand()
    {
        var parser = await this.GetParser();
        return parser == null ? ProxyProtocolCommand.Unknown : await parser.GetCommand();
    }

    public async Task<IProxyProtocolParser?> GetParser()
    {
        // Read from cache
        if (this._isParserCached)
        {
            return this._cachedParser;
        }

        Logger.Log($"[{this._remoteEndpoint}] selecting parser...");
        // Get parser corresponding to version
        this._cachedParser = await this.GetVersion() switch
        {
            ProxyProtocolVersion.V1 => new ProxyProtocolParserV1(this._stream, this._remoteEndpoint, this._buffer, this._bufferSize),
            ProxyProtocolVersion.V2 => new ProxyProtocolParserV2(this._stream, this._remoteEndpoint, this._buffer, this._bufferSize),
            _ => null
        };
        this._isParserCached = true;
        return this._cachedParser;
    }

    public async Task<ProxyProtocolVersion> GetVersion()
    {
        // Read from cache
        if (this._protocolVersion != null)
        {
            return (ProxyProtocolVersion) this._protocolVersion;
        }

        Logger.Log($"[{this._remoteEndpoint}] interpreting protocol version...");

        this._protocolVersion = ProxyProtocolVersion.Unknown;
        // Check if is version 1
        await this.GetBytesTillBufferSize(V1_SIGNATURE.Length);
        if (IsVersion1(this._buffer))
        {
            this._protocolVersion = ProxyProtocolVersion.V1;
        }
        else
        {
            // Check if is version 2 or above
            await this.GetBytesTillBufferSize(V2_OR_ABOVE_ID_LENGTH);
            if (IsVersion2OrAbove(this._buffer))
            {
                // Check versions
                if (IsVersion2(this._buffer, false))
                {
                    this._protocolVersion = ProxyProtocolVersion.V2;
                }
            }
        }

        return (ProxyProtocolVersion) this._protocolVersion;
    }
    #endregion

    #region Private methods
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
        if ((this._bufferSize + length) > this._buffer.Length)
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
    #endregion

    #region Public static methods

    // ReSharper disable once MemberCanBePrivate.Global
    public static bool IsVersion1(ReadOnlySpan<byte> header)
    {
        return header[..V1_SIGNATURE.Length].SequenceEqual(V1_SIGNATURE);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static bool IsVersion2OrAbove(ReadOnlySpan<byte> header)
    {
        return header[..V2_OR_ABOVE_SIGNATURE.Length].SequenceEqual(V2_OR_ABOVE_SIGNATURE);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static bool IsVersion2(ReadOnlySpan<byte> header, bool checkSignature = true)
    {
        return (!checkSignature || IsVersion2OrAbove(header)) &&
        (header[12] & 0xF0) == 0x20;
    }
    #endregion
}