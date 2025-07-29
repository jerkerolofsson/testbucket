using System.Diagnostics;
using System.Text;

namespace TestBucket.AdbProxy.Protocol
{
    public class AdbProtocolMessage
    {
        public AdbProtocolHeader Header { get; } = new();

        /// <summary>
        /// Payload bytes
        /// </summary>
        private byte[]? _payload = null;

        /// <summary>
        /// Header bytes
        /// </summary>
        private readonly byte[] _headerBytes = new byte[AdbProtocolConstants.HeaderLength];
        private int _readHeaderBytes = 0;
        private int _readPayloadBytes = 0;
        public int RemainingHeaderBytes => AdbProtocolConstants.HeaderLength - _readHeaderBytes;
        public int RemainingPayloadBytes => (int)Header.DataLength - _readPayloadBytes;
        public bool HasReadHeader => RemainingHeaderBytes == 0;

        public bool HasReadPayload => HasReadHeader ? RemainingPayloadBytes == 0 : false;

        public static AdbProtocolMessage Create(uint command, uint arg0, uint arg1, string text)
        {
            var payloadLength = Encoding.ASCII.GetByteCount(text) + 1;
            var bytes = new byte[payloadLength];
            Encoding.ASCII.TryGetBytes(text, bytes, out int _);
            bytes[^1] = 0;

            return Create(command, arg0, arg1, bytes);
        }

        public byte[]? Payload => _payload;

        public string? DecodePayloadAsString()
        {
            if (_payload is null)
            {
                return null;
            }
            return Encoding.UTF8.GetString(_payload, 0, _payload.Length);
        }
        public string? DecodePayloadAsString(int maxSize)
        {
            if (_payload is null)
            {
                return null;
            }
            return Encoding.UTF8.GetString(_payload, 0, Math.Min(maxSize,_payload.Length)).Replace("\n", "\\n");
        }

        public async Task WriteToAsync(Stream destination, CancellationToken cancellationToken)
        {
            var headerBytes = this.Header.ToByteArray();

            if (_payload is not null)
            {
                var bytes = new byte[_payload.Length + AdbProtocolConstants.HeaderLength];
                Array.Copy(headerBytes, 0, bytes, 0, headerBytes.Length);
                Array.Copy(_payload, 0, bytes, headerBytes.Length, _payload.Length);

                await destination.WriteAsync(bytes, cancellationToken);
            }
            else
            {
                await destination.WriteAsync(headerBytes, cancellationToken);
            }

            await destination.FlushAsync(cancellationToken);
        }

        public static AdbProtocolMessage Create(uint command, uint arg0, uint arg1, byte[] bytes)
        {
            var message = new AdbProtocolMessage();
            message.Header.Command = command;
            message.Header.Arg0 = arg0;
            message.Header.Arg1 = arg1;
            message._payload = bytes;
            message.Header.DataLength = (uint)bytes.Length;
            message.Header.DataCrc32 = AdbCrc.Crc(bytes);
            message.Header.Magic = command ^ 0xffffffff;
            return message;
        }

        /// <summary>
        /// Used when reading
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="bufferLength"></param>
        public void Append(byte[] bytes, int bufferLength)
        {
            int remainingBytes = bufferLength;

            if(!HasReadHeader)
            {
                // Copy into header buffer
                int offset = _readHeaderBytes;
                int length = Math.Min(RemainingHeaderBytes, bufferLength);
                Array.Copy(bytes, 0, _headerBytes, offset, length);

                _readHeaderBytes += length;
                remainingBytes -= length;

                // If completed, parse the header and prepare for payload
                if (HasReadHeader)
                {
                    ParseHeader();
                    _readPayloadBytes = 0;
                    int payloadBytes = (int)Header.DataLength;
                    if(_payload is null)
                    {
                        _payload = new byte[payloadBytes];
                    }
                }
            }

            // Payload
            if (HasReadHeader && remainingBytes > 0 && RemainingPayloadBytes > 0)
            {
                Debug.Assert(_payload != null, "Payload should be allocated when the complete header");
                if (_payload is not null)
                {
                    int offset = _readPayloadBytes;
                    int length = Math.Min(RemainingPayloadBytes, remainingBytes);
                    Array.Copy(bytes, 0, _payload, offset, length);
                    _readPayloadBytes += length;
                }
            }
        }

        private void ParseHeader()
        {
            Header.ReadFrom(_headerBytes);
        }
    }
}
