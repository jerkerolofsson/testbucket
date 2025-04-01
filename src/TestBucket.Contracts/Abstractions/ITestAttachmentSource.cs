using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Formats.Dtos;

namespace TestBucket.Contracts.Abstractions
{
    public interface ITestAttachmentSource
    {
        List<AttachmentDto> Attachments { get; set; }
    }
}
