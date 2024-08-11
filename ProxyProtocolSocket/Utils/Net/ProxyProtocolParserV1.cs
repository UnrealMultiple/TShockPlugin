using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ProxyProtocolSocket.Utils.Net
{
    public class ProxyProtocolParserV1 : IProxyProtocolParser
    {
        #region Constants
        private const string DELIMITER = "\r\n";
        private const char SEPARATOR = ' ';
        private const int MAX_HEADER_SIZE = 107;
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

        public ProxyProtocolParserV1(NetworkStream stream, IPEndPoint remoteEndpoint, byte[] buffer, int bufferSize)
        {
            #region Args checking
            if (stream == null)                 throw new ArgumentNullException(nameof(stream));
            if (stream.CanRead != true)         throw new     ArgumentException($"argument 'stream' is unreadable");
            if (remoteEndpoint == null)         throw new ArgumentNullException(nameof(remoteEndpoint));
            if (buffer == null)                 throw new ArgumentNullException(nameof(buffer));
            if (bufferSize > buffer.Length)      throw new     ArgumentException($"argument '{nameof(bufferSize)}' is larger than '{nameof(buffer)}.Length'");
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

            #region Getting full header and do first check
            
            await GetFullHeader();
            if (ProxyProtocolSocketPlugin.Config.Settings.LogLevel == LogLevel.Debug)
                Logger.Log($"[{_remoteEndpoint}] header content: {Convert.ToHexString(_buffer[.._bufferSize])}");
            var tokens = Encoding.ASCII.GetString(_buffer[..(_bufferSize - 2)]).Split(SEPARATOR);
            if (tokens.Length < 2)
                throw new Exception("Unable to read AddressFamily and protocol");
            
            #endregion

            #region Parse address family
            
            Logger.Log($"[{_remoteEndpoint}] parsing address family...");
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
                _protocolCommand = ProxyProtocolCommand.Local;
                _sourceEndpoint = _remoteEndpoint;
                _hasParsed = true;
                return;
            }
            
            if (tokens.Length < 6)
                throw new Exception("Impossible to read ip addresses and ports as the number of tokens is less than 6");

            #endregion

            #region Parse source and dest end point
            
            Logger.Log($"[{_remoteEndpoint}] parsing endpoints...");
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

            _addressFamily      = addressFamily;
            _protocolCommand    = ProxyProtocolCommand.Proxy;
            _sourceEndpoint     = sourceEp;
            _destEndpoint       = destEp;
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
        private async Task GetFullHeader()
        {
            Logger.Log($"[{_remoteEndpoint}] getting full header");
            for (var i = 7; i < MAX_HEADER_SIZE; i++) // Search after "PROXY" signature
            {
                if (await GetOneByteAtPosition(i) != DELIMITER[1])
                    continue;
                if (await GetOneByteAtPosition(i - 1) != DELIMITER[0])
                    throw new Exception("Header must end with CRLF");
                return;
            }
            throw new Exception("Failed to find any delimiter within the maximum header size of version 1");
        }

        private async Task GetBytesTillBufferSize(int size)
        {
            if (size <= _bufferSize)
                return;
            await GetBytesFromStream(size - _bufferSize);
        }

        private async Task GetBytesFromStream(int length)
        {
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

        private async Task<byte> GetOneByteAtPosition(int position)
        {
            await GetBytesTillBufferSize(position + 1);
            return _buffer[position];
        }
        
        #endregion
    }
}
