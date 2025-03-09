using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Formats.Dtos
{
    public class AttachmentDto
    {
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public byte[]? Data { get; set; }
    }
}
