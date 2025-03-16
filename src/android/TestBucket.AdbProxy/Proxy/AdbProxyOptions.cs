using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Proxy
{
    public class AdbProxyOptions
    {
        /// <summary>
        /// Listen address (default "any")
        /// </summary>
        public IPAddress ListenAddress { get; set; } = IPAddress.Any;

        /// <summary>
        /// Listen port (default 15037) for the first device
        /// </summary>
        public int ListenPort { get; set; } = 15037;

        /// <summary>
        /// End range of listen port (inclusive)
        /// </summary>
        public int MaxListenPort { get; set; } = 16037;

        /// <summary>
        /// adb daemon port
        /// </summary>
        public int DestinationPort { get; set; } = 5037;
    }
}
