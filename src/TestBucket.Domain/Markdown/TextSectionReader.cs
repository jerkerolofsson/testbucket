using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Markdown
{
    internal class TextSectionReader
    {
        public static IEnumerable<string> ReadSections(string text)
        {
            bool wasLastLineEmpty = false;

            var sb = new StringBuilder();

            foreach (var line in text.Split('\n'))
            {
                sb.Append(line);
                sb.Append('\n');

                bool isEmptyLine = string.IsNullOrWhiteSpace(line);
                if (isEmptyLine)
                {
                    if (!wasLastLineEmpty)
                    {
                        yield return sb.ToString();
                        sb.Clear();
                    }

                    wasLastLineEmpty = true;
                }
                else
                {
                    wasLastLineEmpty = false;
                }
            }

            if(sb.Length > 0)
            {
                yield return sb.ToString();
            }
        }
    }
}
