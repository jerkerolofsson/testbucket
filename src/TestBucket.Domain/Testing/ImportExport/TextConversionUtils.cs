using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Testing.ImportExport
{
    public static class TextConversionUtils
    {
        public static string FromUtf8(byte[] bytes, bool removeBom = true)
        {
            if(bytes.Length < 3)
            {
                return Encoding.UTF8.GetString(bytes);
            }

            // Remove the BOM if text contains a BOM
            if (bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            {
                return Encoding.UTF8.GetString(bytes, 3, bytes.Length-3);
            }
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
