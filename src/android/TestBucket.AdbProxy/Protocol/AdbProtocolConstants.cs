using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Protocol
{
    public static class AdbProtocolConstants
    {
        public const int DefaultAdbdPort = 5037;
        public const int HeaderLength = 24;

        public const uint A_SYNC = 0x434e5953;
        public const uint A_CNXN = 0x4e584e43;
        public const uint A_OPEN = 0x4e45504f;
        public const uint A_OKAY = 0x59414b4f;
        public const uint A_CLSE = 0x45534c43;
        public const uint A_WRTE = 0x45545257;

        public const uint ProtocolVersion = 0x1000000;
        public const uint MaxDataLength   =  0x100000;

    }
}
