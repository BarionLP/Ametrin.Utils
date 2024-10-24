using System.Net;
using System.Net.Sockets;

namespace Ametrin.Utils;

public static class SystemHelper
{
    public static Option<IPAddress> LocalIPAddress()
    {
        try
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            var endPoint = socket.LocalEndPoint as IPEndPoint;
            return endPoint!.Address;
        }
        catch
        {
            return default;
        }
    }

    public static IEnumerable<IPAddress> InterNetworkAddresses()
    {
        return Dns.GetHostAddresses(Dns.GetHostName()).Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);
    }
}
