using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Proxy
{
    public interface IConnectionObserver
    {
        /// <summary>
        /// An unexpected exception was caught
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="exception"></param>
        void OnConnectionException(AdbProxyClientHandler connection, Exception exception);

        /// <summary>
        /// CLSE received
        /// </summary>
        /// <param name="connection"></param>
        void OnClose(AdbProxyClientHandler connection);
    }
}
