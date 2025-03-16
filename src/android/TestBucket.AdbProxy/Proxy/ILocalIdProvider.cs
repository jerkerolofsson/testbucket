using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Proxy;
public interface ILocalIdProvider
{
    /// <summary>
    /// Generates a new local ID for a new stream
    /// </summary>
    /// <returns></returns>
    int IncrementAndGetLocalId();
}
