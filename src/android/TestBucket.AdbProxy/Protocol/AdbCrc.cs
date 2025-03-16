using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Protocol
{
    internal class AdbCrc
    {
        public static uint Crc(byte[] data)
        {
            return Crc(data, 0, data.Length);
        }
        public static uint Crc(byte[] data, int offset, int length)
        {
            if(data.Length == 0)
            {
                return 0;
            }

            uint crc = 0;
            for(int i=offset; i<offset+length; i++)
            {
                uint b = data[i];
                crc = (crc + data[i]) & 0xffffffff;
            }
            return crc;
        }
    }
}
