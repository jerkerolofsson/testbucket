using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Markdown
{
    public class MarkdownSection
    {
        public string[] Path { get; set; } = [];
        public string? Heading { get; set; }
        public string? Text { get; set; }
    }
}
