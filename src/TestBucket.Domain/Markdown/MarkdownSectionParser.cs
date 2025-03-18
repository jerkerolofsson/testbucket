namespace TestBucket.Domain.Markdown
{
    internal class MarkdownSectionParser
    {
        public static IEnumerable<MarkdownSection> ReadSections(string text, CancellationToken cancellationToken)
        {
            Stack<string> path = new();
            foreach (var section in TextSectionReader.ReadSections(text))
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

        private static MarkdownSection ExtractSection(Stack<string> path, string section)
        {
            var lines = section.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
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
