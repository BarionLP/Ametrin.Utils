using System.Net.Sockets;
using System.Net;

namespace Ametrin.Utils;

public static class SystemExtensions {
    public static Result<string> LocalIPAddress() {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
        socket.Connect("8.8.8.8", 65530);
        if(socket.LocalEndPoint is not IPEndPoint endPoint) return ResultStatus.Failed;

        return endPoint.Address.ToString();
    }
}
