using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Proxy;
public enum AdbProxyClientState
{
    Initial,

    /// <summary>
    /// Handshake completed
    /// </summary>
    Connected,
}
