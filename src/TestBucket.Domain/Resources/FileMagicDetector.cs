using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Resources;
public class FileMagicDetector
{
    /// <summary>
    /// Returns media typoe
    /// </summary>
    /// <param name="fileBytes"></param>
    /// <returns></returns>
    public static string? DetectFileType(byte[] fileBytes)
    {
        // Check for common file signatures (magic numbers)
        if (fileBytes.Length >= 4 && fileBytes[0] == 0x89 && fileBytes[1] == 0x50 && fileBytes[2] == 0x4E && fileBytes[3] == 0x47) // PNG
        {
            return "image/png";
        }
        if (fileBytes.Length >= 2 && fileBytes[0] == 0xFF && fileBytes[1] == 0xD8) // JPEG
        {
            return "image/jpeg";
        }
        if (fileBytes.Length >= 4 && fileBytes[0] == 0x47 && fileBytes[1] == 0x49 && fileBytes[2] == 0x46 && fileBytes[3] == 0x38)
        {
            return "image/gif";
        }
        // 25 50 44 46 2D
        if (fileBytes.Length >= 5 && fileBytes[0] == 0x25 && fileBytes[1] == 0x50 && fileBytes[2] == 0x44 && fileBytes[3] == 0x46 && fileBytes[4] == 0x2d) // PDF
        {
            return "application/pdf";
        }

        //DF BF 34 EB CE
        if (fileBytes.Length >= 5 && fileBytes[0] == 0xDF && fileBytes[1] == 0xBF && fileBytes[2] == 0x34 && fileBytes[3] == 0xEB && fileBytes[4] == 0xCE) // PDF
        {
            return "application/pdf";
        }
        if (fileBytes.Length >= 2 && fileBytes[0] == 0x42 && fileBytes[1] == 0x4D) // BMP
        {
            return "image/bmp";
        }

        // ZIP: 50 4B 03 04 or 50 4B 05 06 or 50 4B 07 08
        if (fileBytes.Length >= 4 &&
            fileBytes[0] == 0x50 && fileBytes[1] == 0x4B &&
            (fileBytes[2] == 0x03 || fileBytes[2] == 0x05 || fileBytes[2] == 0x07) &&
            (fileBytes[3] == 0x04 || fileBytes[3] == 0x06 || fileBytes[3] == 0x08))
        {
            return "application/zip";
        }
        // 7z: 37 7A BC AF 27 1C
        if (fileBytes.Length >= 6 &&
            fileBytes[0] == 0x37 && fileBytes[1] == 0x7A && fileBytes[2] == 0xBC &&
            fileBytes[3] == 0xAF && fileBytes[4] == 0x27 && fileBytes[5] == 0x1C)
        {
            return "application/x-7z-compressed";
        }
        // RAR: 52 61 72 21 1A 07 00 (RAR v1.5 to v4.0) or 52 61 72 21 1A 07 01 00 (RAR v5.0+)
        if (fileBytes.Length >= 7 &&
            fileBytes[0] == 0x52 && fileBytes[1] == 0x61 && fileBytes[2] == 0x72 &&
            fileBytes[3] == 0x21 && fileBytes[4] == 0x1A && fileBytes[5] == 0x07 &&
            (fileBytes[6] == 0x00 || (fileBytes.Length >= 8 && fileBytes[6] == 0x01 && fileBytes[7] == 0x00)))
        {
            return "application/x-rar-compressed";
        }

        return null;
    }

}
