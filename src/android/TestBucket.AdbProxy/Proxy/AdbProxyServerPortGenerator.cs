using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

namespace TestBucket.AdbProxy.Proxy;
public class AdbProxyServerPortGenerator : IPortGenerator
{
    private readonly AdbProxyOptions _options;
    private int _nextPort = 0;

    public AdbProxyServerPortGenerator(IOptions<AdbProxyOptions> options)
    {
        _options = options.Value;
        _nextPort = options.Value.ListenPort;
    }

    public int GetNextPort()
    {
        var port = _nextPort;

        _nextPort++;
        if (_nextPort > _options.MaxListenPort)
        {
            _nextPort = _options.ListenPort;
        }

        return port;
    }
}
