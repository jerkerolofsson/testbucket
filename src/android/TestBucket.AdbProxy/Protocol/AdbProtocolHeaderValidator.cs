using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Protocol
{
    internal class AdbProtocolHeaderValidator
    {
        public static void Validate(AdbProtocolHeader header)
        {
            if (header.DataLength > int.MaxValue)
            {
                throw new InvalidDataException("ADB protocol data length cannot exceed int.MaxValue");
            }
            if (header.DataLength > AdbProtocolConstants.MaxDataLength)
            {
                throw new InvalidDataException("ADB protocol data length exceeds MaxDataLength");
            }

            // Todo: Verify CRC
            var expectedMagic = header.Command ^ 0xffffffff;
            if (expectedMagic != header.Magic)
            {
                throw new InvalidDataException($"Magic mismatch, got: {header.Magic.ToString("x8")}, expected: {expectedMagic.ToString("x8")}");
            }
        }
    }
}
