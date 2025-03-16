using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Proxy;
public interface IPortGenerator
{
    /// <summary>
    /// Returns a TCP port to use for a device
    /// </summary>
    /// <returns></returns>
    int GetNextPort();
}
