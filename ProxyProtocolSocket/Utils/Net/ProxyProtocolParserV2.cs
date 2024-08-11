using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;

namespace ProxyProtocolSocket.Utils.Net
{
    public class ProxyProtocolParserV2 : IProxyProtocolParser
    {
        #region Constants
        private const int SIGNATURE_LENGNTH = 16;
        private const int IPV4_ADDR_LENGTH = 4;
        private const int IPV6_ADDR_LENGTH = 16;
        #endregion

        #region Members
        private NetworkStream _stream;
        private IPEndPoint _remoteEndpoint;
        private byte[] _buffer;
        private int _bufferSize;

        private bool _hasParsed;
        private AddressFamily _addressFamily = AddressFamily.Unknown;
        private ProxyProtocolCommand _protocolCommand = ProxyProtocolCommand.Unknown;
        private IPEndPoint? _sourceEndpoint;
        private IPEndPoint? _destEndpoint;
        #endregion

        public ProxyProtocolParserV2(NetworkStream stream, IPEndPoint remoteEndpoint, byte[] buffer, int bufferSize)
        {
            #region Args checking
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (stream.CanRead != true) throw new ArgumentException($"argument '{nameof(stream)}' is unreadable");
            if (remoteEndpoint == null) throw new ArgumentNullException(nameof(remoteEndpoint));
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (bufferSize > buffer.Length) throw new ArgumentException($"argument '{nameof(bufferSize)}' is larger than '{nameof(buffer)}.Length'");
            #endregion

            #region Filling members
            _stream = stream;
            _remoteEndpoint = remoteEndpoint;
            _buffer = buffer;
            _bufferSize = bufferSize;
            #endregion
        }

        #region Public methods
        public async Task Parse()
        {
            if (_hasParsed)
                return;
            _hasParsed = true;
            Logger.Log($"[{_remoteEndpoint}] parsing header...");

            // load full signature from stream
            await GetBytesTillBufferSize(SIGNATURE_LENGNTH);
            if (ProxyProtocolSocketPlugin.Config.Settings.LogLevel == LogLevel.Debug)
                Logger.Log($"[{_remoteEndpoint}] signature content: {Convert.ToHexString(_buffer[.._bufferSize])}");

            #region Parsing command

            var command = (_buffer[12] & 0x0F) switch
            {
                0x00 => ProxyProtocolCommand.Local,
                0x01 => ProxyProtocolCommand.Proxy,
                _ => throw new Exception("Invalid command")
            };

            #endregion

            #region Parsing address family and getting min address length

            var family = (_buffer[13] & 0xF0) switch
            {
                0x00 => AddressFamily.Unspecified,
                0x10 => AddressFamily.InterNetwork,
                0x20 => AddressFamily.InterNetworkV6,
                0x30 => AddressFamily.Unix,
                _ => throw new Exception("Invalid address family")
            };
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            var minAddressLength = family switch
            {
                AddressFamily.Unspecified => 0,
                AddressFamily.InterNetwork => 12,
                AddressFamily.InterNetworkV6 => 36,
                AddressFamily.Unix => 216,
                _ => throw new Exception("Invalid address family")
            };
            
            // TODO: Implement address family UNIX
            if (family == AddressFamily.Unix)
                throw new NotImplementedException("Address family UNIX haven't been implemented yet");
            
            #endregion

            #region Parsing transport protocol
            // TODO: Parse transport protocol
            #endregion

            #region Parsing and checking address length
            
            var addressLength = GetAddressLength(_buffer);
            Logger.Log($"[{_remoteEndpoint}] Address length is {addressLength}");
            if (addressLength < minAddressLength)
                throw new Exception("Address length is too short");
            if (SIGNATURE_LENGNTH + addressLength > _buffer.Length)
                throw new Exception("Address length is too long");
            #endregion

            #region Getting address data and check if need to parse address data
            
            await GetBytesTillBufferSize(SIGNATURE_LENGNTH + addressLength);
            if (ProxyProtocolSocketPlugin.Config.Settings.LogLevel == LogLevel.Debug)
                Logger.Log($"[{_remoteEndpoint}] header content: {Convert.ToHexString(_buffer[.._bufferSize])}");
            if (command != ProxyProtocolCommand.Proxy || family == AddressFamily.Unspecified)
            {
                _protocolCommand = command;
                _addressFamily = family;
                return;
            }
            
            #endregion

            #region Parsing address data
            
            IPEndPoint sourceEp;
            IPEndPoint destEp;
            try
            {
                switch (family)
                {
                    case AddressFamily.InterNetwork:
                        sourceEp = new IPEndPoint(GetSourceAddressIPv4(_buffer), GetSourcePortIPv4(_buffer));
                        destEp = new IPEndPoint(GetDestinationAddressIPv4(_buffer), GetDestinationPortIPv4(_buffer));
                    break;

                    case AddressFamily.InterNetworkV6:
                        sourceEp = new IPEndPoint(GetSourceAddressIPv6(_buffer), GetSourcePortIPv6(_buffer));
                        destEp = new IPEndPoint(GetDestinationAddressIPv6(_buffer), GetDestinationPortIPv6(_buffer));
                        break;

                    case AddressFamily.Unix:
                        throw new NotImplementedException("Address family UNIX haven't implemented yet");

                    default:
                        throw new Exception("Unhandled address family while parsing address data");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to parse ip addresses and ports", ex);
            }
            #endregion

            _protocolCommand = command;
            _addressFamily = family;
            _sourceEndpoint = sourceEp;
            _destEndpoint = destEp;
        }

        public async Task<IPEndPoint?> GetSourceEndpoint()
        {
            await Parse();
            return _sourceEndpoint;
        }

        public async Task<IPEndPoint?> GetDestEndpoint()
        {
            await Parse();
            return _destEndpoint;
        }

        public async Task<AddressFamily> GetAddressFamily()
        {
            await Parse();
            return _addressFamily;
        }

        public async Task<ProxyProtocolCommand> GetCommand()
        {
            await Parse();
            return _protocolCommand;
        }
        #endregion

        #region Private methods
        private async Task GetBytesTillBufferSize(int size)
        {
            Logger.Log($"[{_remoteEndpoint}] extending buffer size to {size}");
            if (size <= _bufferSize)
                return;
            await GetBytesFromStream(size - _bufferSize);
        }

        private async Task GetBytesFromStream(int length)
        {
            Logger.Log($"[{_remoteEndpoint}] getting {length} bytes from stream");
            if (_bufferSize + length > _buffer.Length)
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

        #region Private static methods
        private static int GetAddressLength(ReadOnlySpan<byte> signature) =>
            BinaryPrimitives.ReadUInt16BigEndian(BufferSegment(signature, SIGNATURE_LENGNTH - 2, 2));

        #region IPv4
        private static IPAddress GetSourceAddressIPv4(ReadOnlySpan<byte> header) =>
            new IPAddress(BufferSegment(header, SIGNATURE_LENGNTH, IPV4_ADDR_LENGTH));

        private static IPAddress GetDestinationAddressIPv4(ReadOnlySpan<byte> header) =>
            new IPAddress(BufferSegment(header, SIGNATURE_LENGNTH + IPV4_ADDR_LENGTH, IPV4_ADDR_LENGTH));

        private static int GetSourcePortIPv4(ReadOnlySpan<byte> header) =>
            BinaryPrimitives.ReadUInt16BigEndian(BufferSegment(header, SIGNATURE_LENGNTH + 2*IPV4_ADDR_LENGTH, 2));

        private static int GetDestinationPortIPv4(ReadOnlySpan<byte> header) =>
            BinaryPrimitives.ReadUInt16BigEndian(BufferSegment(header, SIGNATURE_LENGNTH + 2*IPV4_ADDR_LENGTH + 2, 2));
        #endregion

        #region IPv6
        private static IPAddress GetSourceAddressIPv6(ReadOnlySpan<byte> header) =>
            new IPAddress(BufferSegment(header, SIGNATURE_LENGNTH, IPV6_ADDR_LENGTH));

        private static IPAddress GetDestinationAddressIPv6(ReadOnlySpan<byte> header) =>
            new IPAddress(BufferSegment(header, SIGNATURE_LENGNTH + IPV6_ADDR_LENGTH, IPV6_ADDR_LENGTH));

        private static int GetSourcePortIPv6(ReadOnlySpan<byte> header) =>
            BinaryPrimitives.ReadUInt16BigEndian(BufferSegment(header, SIGNATURE_LENGNTH + 2*IPV6_ADDR_LENGTH, 2));

        private static int GetDestinationPortIPv6(ReadOnlySpan<byte> header) =>
            BinaryPrimitives.ReadUInt16BigEndian(BufferSegment(header, SIGNATURE_LENGNTH + 2*IPV6_ADDR_LENGTH + 2, 2));
        #endregion

        private static ReadOnlySpan<byte> BufferSegment(ReadOnlySpan<byte> bytes, int offset, int length) =>
            bytes[offset..(offset + length)];

        #endregion
    }
}
