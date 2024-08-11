using System.Net;
using System.Net.Sockets;
using System.Text;
using ProxyProtocolSocket.Utils.Net;

namespace ProxyProtocolSocket.Utils.Net
{
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
        private NetworkStream _stream;
        private IPEndPoint _remoteEndpoint;

        private byte[] _buffer                          = new byte[MAX_BUFFER_SIZE];
        // i.e. the end of _buffer
        // _buffer[_bufferSize - 1] is the last byte read from the stream
        private int _bufferSize                         = 0;

        private bool _isParserCached                    = false;
        private IProxyProtocolParser? _cachedParser     = null;
        private ProxyProtocolVersion? _protocolVersion  = null;
        #endregion

        public ProxyProtocol(NetworkStream stream, IPEndPoint remoteEndpoint)
        {
            #region Args checking
            if (stream == null)         throw new ArgumentNullException(nameof(stream));
            if (stream.CanRead != true) throw new     ArgumentException("argument 'stream' is unreadable");
            if (remoteEndpoint == null) throw new ArgumentNullException(nameof(remoteEndpoint));
            #endregion

            #region Filling members
            _stream = stream;
            _remoteEndpoint = remoteEndpoint;
            #endregion
        }

        #region Public methods
        public async Task Parse()
        {
            var parser = await GetParser();
            if (parser == null)
                return;
            Logger.Log($"[{_remoteEndpoint}] calling {parser.GetType().Name}.Parse()");
            await parser.Parse();
        }

        public async Task<IPEndPoint?> GetSourceEndpoint()
        {
            var parser = await GetParser();
            if (parser == null)
                return null;
            return await parser.GetSourceEndpoint();
        }

        public async Task<IPEndPoint?> GetDestEndpoint()
        {
            var parser = await GetParser();
            if (parser == null)
                return null;
            return await parser.GetDestEndpoint();
        }

        public async Task<AddressFamily> GetAddressFamily()
        {
            var parser = await GetParser();
            // return Unknown when the header is unparseable
            // return Unspecified when the address family written in the header is "UNKNOWN"
            if (parser == null)
                return AddressFamily.Unknown;
            return await parser.GetAddressFamily();
        }

        public async Task<ProxyProtocolCommand> GetCommand()
        {
            var parser = await GetParser();
            if (parser == null)
                return ProxyProtocolCommand.Unknown;
            return await parser.GetCommand();
        }

        public async Task<IProxyProtocolParser?> GetParser()
        {
            // Read from cache
            if (_isParserCached)
                return _cachedParser;

            Logger.Log($"[{_remoteEndpoint}] selecting parser...");
            // Get parser corresponding to version
            _cachedParser = await GetVersion() switch
            {
                ProxyProtocolVersion.V1 => new ProxyProtocolParserV1(_stream, _remoteEndpoint, _buffer, _bufferSize),
                ProxyProtocolVersion.V2 => new ProxyProtocolParserV2(_stream, _remoteEndpoint, _buffer, _bufferSize),
                _ => null
            };
            _isParserCached = true;
            return _cachedParser;
        }

        public async Task<ProxyProtocolVersion> GetVersion()
        {
            // Read from cache
            if (_protocolVersion != null)
                return (ProxyProtocolVersion)_protocolVersion;

            Logger.Log($"[{_remoteEndpoint}] interpreting protocol version...");

            _protocolVersion = ProxyProtocolVersion.Unknown;
            // Check if is version 1
            await GetBytesTillBufferSize(V1_SIGNATURE.Length);
            if (IsVersion1(_buffer))
                _protocolVersion = ProxyProtocolVersion.V1;
            else
            {
                // Check if is version 2 or above
                await GetBytesTillBufferSize(V2_OR_ABOVE_ID_LENGTH);
                if (IsVersion2OrAbove(_buffer))
                {
                    // Check versions
                    if (IsVersion2(_buffer, false))
                        _protocolVersion = ProxyProtocolVersion.V2;
                }
            }

            return (ProxyProtocolVersion)_protocolVersion;
        }
        #endregion

        #region Private methods
        private async Task GetBytesTillBufferSize(int size)
        {
            if (size <= _bufferSize)
                return;
            await GetBytesFromStream(size - _bufferSize);
        }

        private async Task GetBytesFromStream(int length)
        {
            if ((_bufferSize + length) > _buffer.Length)
                throw new InternalBufferOverflowException();

            while (length > 0)
            {
                if (!_stream.DataAvailable)
                    throw new EndOfStreamException();

                var count = await _stream.ReadAsync(_buffer.AsMemory(_bufferSize, length));
                length -= count;
                _bufferSize += count;
            }
        }
        #endregion

        #region Public static methods
        
        // ReSharper disable once MemberCanBePrivate.Global
        public static bool IsVersion1(ReadOnlySpan<byte> header) =>
            header[..V1_SIGNATURE.Length].SequenceEqual(V1_SIGNATURE);
        
        // ReSharper disable once MemberCanBePrivate.Global
        public static bool IsVersion2OrAbove(ReadOnlySpan<byte> header) =>
            header[..V2_OR_ABOVE_SIGNATURE.Length].SequenceEqual(V2_OR_ABOVE_SIGNATURE);
        
        // ReSharper disable once MemberCanBePrivate.Global
        public static bool IsVersion2(ReadOnlySpan<byte> header, bool checkSignature = true) =>
            (!checkSignature || IsVersion2OrAbove(header)) &&
            (header[12] & 0xF0) == 0x20;
        #endregion
    }
}
