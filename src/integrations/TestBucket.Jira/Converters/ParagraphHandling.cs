using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Jira.Converters;
public enum ParagraphHandling
{
    /// <summary>
    /// Defaults to adding line when there is a paragraph.
    /// For tables, we don't want to add new lines as that will break the table formatting in markdown
    /// </summary>
    Default = 1,
    NoNewLine = 2
}
