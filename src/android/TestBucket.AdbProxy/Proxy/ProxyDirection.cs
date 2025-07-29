using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Proxy;
public enum ProxyDirection
{
    /// <summary>
    /// From adb client to adb host
    /// </summary>
    ClientToHost,

    /// <summary>
    /// From adb host to adb client
    /// </summary>
    HostToClient
}
