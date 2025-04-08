using System.Text;

namespace TestBucket.Domain.Markdown
{
    internal class MarkdownSectionParser
    {
        public static IEnumerable<MarkdownSection> ReadSections(string text, CancellationToken cancellationToken)
        {
            Stack<string> path = new();
            foreach (var section in ReadSections(text))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                yield return ExtractSection(path, section);
            }
        }
        internal static int GetHeaderDepth(string name)
        {
            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] != '#')
                {
                    return i;
                }
            }
            return 0;
        }

        /// <summary>
        /// Reads sections where a heading marks a section
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IEnumerable<string> ReadSections(string text)
        {
            var sb = new StringBuilder();

            foreach (var line in text.Split('\n'))
            {
                bool isHeader = line.Trim().StartsWith('#');
                if (isHeader)
                {
                    if (sb.Length > 0)
                    {
                        yield return sb.ToString();
                        sb.Clear();
                    }
                }

                sb.Append(line.TrimEnd('\r'));
                sb.Append('\n');
            }

            if (sb.Length > 0)
            {
                yield return sb.ToString();
            }
        }

        private static MarkdownSection ExtractSection(Stack<string> path, string section)
        {
            var lines = section.Split('\n');
            string? header = null;
            string[] currentPath = path.Reverse().ToArray();

            if (lines.Length > 0)
            {
                var name = lines[0];

                int headerDepth = GetHeaderDepth(name);
                while(headerDepth <= path.Count && path.Count > 0)
                {
                    path.TryPop(out var _);
                }
                currentPath = path.Reverse().ToArray();
                if (headerDepth > 0)
                {
                    header = name.TrimStart('#', ' ', '\t');
                    path.Push(header);
                }
            }

            string text = section;
            if (header is not null)
            {
                text = "";
                if (lines.Length > 1)
                {
                    text = string.Join("\n", lines.Skip(1));
                }
            }

            return new MarkdownSection() { Text = text, Path = currentPath, Heading = header };
        }
    }
}
