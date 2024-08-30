using System.Net;
using System.Net.Sockets;

namespace ProxyProtocolSocket.Utils.Net
{
    public interface IProxyProtocolParser
    {
        Task Parse();
        Task<IPEndPoint?> GetSourceEndpoint();
        Task<IPEndPoint?> GetDestEndpoint();
        Task<AddressFamily> GetAddressFamily();
        Task<ProxyProtocolCommand> GetCommand();
    }
}
