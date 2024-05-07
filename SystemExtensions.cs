using Ametrin.Utils.Optional;
using System.Net;

namespace Ametrin.Utils;

public static class SystemExtensions {
    public static Option<IPAddress> LocalIPAddress() {
        var addresses = Dns.GetHostAddresses(Dns.GetHostName());
        if(addresses.Length == 0) return Option<IPAddress>.None();
        return addresses[0];
    }
    
    public static Option<IPAddress> LocalIPv4Address() {
        var addresses = LocalIPv4Addresses();
        if(!addresses.Any()) return Option<IPAddress>.None();
        return addresses.First();
    }
    public static IEnumerable<IPAddress> LocalIPv4Addresses() {
        var addresses = Dns.GetHostAddresses(Dns.GetHostName()).Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        if(!addresses.Any()) return [];

        return addresses;
    }
    
    public static async Task<Result<IPAddress>> LocalIPAddressAsync() {
        var addresses = await Dns.GetHostAddressesAsync(Dns.GetHostName());
        if(addresses.Length == 0) return ResultFlag.ConnectionFailed;
        return addresses[0];
    }
}
