using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Host;
internal interface IAdbStreamObserver
{
    /// <summary>
    /// Invoked from the AdbHostClient when in proxy mode
    /// Contains the data read from ADB
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    Task OnDataReceivedAsync(AdbStream adbStream, Memory<byte> data);

    /// <summary>
    /// An unexpected exception was caught
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="exception"></param>
    Task OnAdbHostExceptionAsync(AdbStream adbStream, Exception exception);
}
