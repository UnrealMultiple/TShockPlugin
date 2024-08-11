using System.Net.Sockets;

namespace ProxyProtocolSocket.Utils.Exts
{
    public static class NetworkStreamExt
    {
        public static async Task WaitUntilDataAvailableAsync(this NetworkStream stream, int frequency = 25, int timeout = -1) =>
            await TaskExt.WaitUntilAsync(() => stream.DataAvailable, frequency, timeout);
    }
}
