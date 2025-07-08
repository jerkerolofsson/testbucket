using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Jira.Converters;
internal class JiraContentOptions
{
    public ParagraphHandling ParagraphHandling { get; set; } = ParagraphHandling.Default;
}
