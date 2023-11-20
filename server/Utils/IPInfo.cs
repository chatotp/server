using System.Net.NetworkInformation;

namespace server.Utils
{
    public class IPInfo
    {
        public static string? GetIPv4OfWiFiInterface()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && ni.OperationalStatus == OperationalStatus.Up);

            foreach (var networkInterface in networkInterfaces)
            {
                var properties = networkInterface.GetIPProperties();
                var ipv4Address = properties
                    .UnicastAddresses
                    .FirstOrDefault(address =>
                        address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.Address;

                if (ipv4Address != null)
                {
                    return ipv4Address.ToString();
                }
            }

            return null;
        }
    }
}
