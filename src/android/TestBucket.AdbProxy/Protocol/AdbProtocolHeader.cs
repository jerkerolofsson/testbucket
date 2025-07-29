using System.Buffers.Binary;

namespace TestBucket.AdbProxy.Protocol
{
    public class AdbProtocolHeader
    {
        public uint Command { get; set; }
        public uint Arg0 { get; set; }
        public uint Arg1 { get; set; }
        public uint DataLength { get; set; }
        public uint DataCrc32 { get; set; }
        public uint Magic { get; set; } // Command ^ 0xffffffff

        public const int HeaderSize = 6 * 4;

        internal void ReadFrom(byte[] headerBytes)
        {
            Memory<byte> memory = new Memory<byte>(headerBytes);

            Command = BinaryPrimitives.ReadUInt32LittleEndian(memory.Slice(0, 4).Span);
            Arg0 = BinaryPrimitives.ReadUInt32LittleEndian(memory.Slice(4, 4).Span);
            Arg1 = BinaryPrimitives.ReadUInt32LittleEndian(memory.Slice(8, 4).Span);
            DataLength = BinaryPrimitives.ReadUInt32LittleEndian(memory.Slice(12, 4).Span);
            DataCrc32 = BinaryPrimitives.ReadUInt32LittleEndian(memory.Slice(16, 4).Span);
            Magic = BinaryPrimitives.ReadUInt32LittleEndian(memory.Slice(20, 4).Span);

            AdbProtocolHeaderValidator.Validate(this);

            //var commandText = Encoding.ASCII.GetString(memory.Slice(0, 4).Span);
        }

        internal byte[] ToByteArray()
        {
            var byteArray = new byte[AdbProtocolConstants.HeaderLength];
            Span<byte> bytes = byteArray;

            BinaryPrimitives.WriteUInt32LittleEndian(bytes.Slice(0, 4), Command);
            BinaryPrimitives.WriteUInt32LittleEndian(bytes.Slice(4, 4), Arg0);
            BinaryPrimitives.WriteUInt32LittleEndian(bytes.Slice(8, 4), Arg1);
            BinaryPrimitives.WriteUInt32LittleEndian(bytes.Slice(12, 4), DataLength);
            BinaryPrimitives.WriteUInt32LittleEndian(bytes.Slice(16, 4), DataCrc32);
            BinaryPrimitives.WriteUInt32LittleEndian(bytes.Slice(20, 4), Magic);

            return byteArray;
        }
    };
}
