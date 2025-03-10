using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Formats.Dtos
{
    public class AttachmentDto
    {
        /// <summary>
        /// Name of attachment (filename)
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Media type
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        public byte[]? Data { get; set; }

        /// <summary>
        /// True if the attachment is a screen-shot
        /// </summary>
        public bool IsScreenshot { get; set; }
    }
}
