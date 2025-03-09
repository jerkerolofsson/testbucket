using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Formats.Abstractions
{
    public interface ITestAttachmentSource
    {
        List<AttachmentDto> Attachments { get; set; }
    }
}
