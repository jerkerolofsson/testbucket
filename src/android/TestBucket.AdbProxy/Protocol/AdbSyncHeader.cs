using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Protocol;

/// <summary>
/// https://android.googlesource.com/platform/system/core/+/refs/tags/android-7.1.2_r39/adb/SYNC.TXT
/// </summary>
public record class AdbSyncHeader
{
    /// <summary>
    /// 4 bytes
    /// </summary>
    public required string Command { get; set; }

    /// <summary>
    /// 4 bytes, little endian
    /// </summary>
    public required uint PacketLength { get; set; }

    public static AdbSyncHeader FromBytes(Memory<byte> bytes)
    {
        ReadOnlySpan<byte> commandBytes = bytes.Span.Slice(0, 4);
        var command = Encoding.UTF8.GetString(commandBytes);
        var packetLength = BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(4, 4).Span);

        return new AdbSyncHeader { Command = command, PacketLength = packetLength };
    }
}
