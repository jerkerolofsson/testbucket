using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Requirements;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Markdown;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Requirements.Import.Strategies
{
    class RfcImporter : IDocumentImportStrategy
    {
        public Task ImportAsync(RequirementSpecificationDto spec, FileResource fileResource)
        {
            var text = Encoding.UTF8.GetString(fileResource.Data);

            var sb = new StringBuilder();

            var sections = TextSectionReader.ReadSections(text).ToArray();
            if(sections.Length >= 3)
            {
                spec.Name = sections[2].Replace("\r", "").Replace("\n", "").Trim();
            }

            foreach (var section in sections)
            {
                var lines = section.Split('\n');

                // If the section contains a | character, we treat it as 'pre'
                var hasTable = lines.Select(x => x.Trim()).Where(x => x.StartsWith('|')).Any();
                if(hasTable)
                {
                    sb.Append("```\n");
                }

                foreach (var line in lines)
                {
                    if (line.Length > 0)
                    {
                        // 1.  Introduction
                        if (Char.IsDigit(line[0]))
                        {
                            int numPeriods = line.Count(x => x == '.');
                            sb.Append("".PadLeft(numPeriods, '#') + " ");
                        }
                        if(line.Trim().ToLower().StartsWith("note:"))
                        {
                            sb.Append('>');
                        }
                    }

                    sb.Append(line);
                    sb.Append('\n');
                }

                if (hasTable)
                {
                    sb.Append("```\n");
                }
            }

            spec.Description = sb.ToString();
            return Task.FromResult(spec);
        }

        
    }
}
