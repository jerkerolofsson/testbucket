using System.Net;
using System.Net.Sockets;

namespace TestBucket.ResourceServer.Utilities;
public class PublicHostnameDetector
{
    public static string? GetPublicPort()
    {
        return Environment.GetEnvironmentVariable("TB_PUBLIC_PORT");
    }

    public static string GetPublicHostname()
    {
        // If running a docker container, use environment variable
        string? hostname = Environment.GetEnvironmentVariable("TB_PUBLIC_IP");
        if (hostname is null)
        {
            foreach (var address in Dns.GetHostAddresses(Dns.GetHostName(), AddressFamily.InterNetwork))
            {
                if (!IPAddress.IsLoopback(address))
                {
                    hostname = address.ToString();
                    break;
                }
            }
        }

        return hostname ?? Environment.MachineName;
    }
}
