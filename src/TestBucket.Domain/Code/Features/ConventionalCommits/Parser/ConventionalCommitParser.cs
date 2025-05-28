using System.Text.RegularExpressions;

using TestBucket.Domain.Code.Features.ConventionalCommits.Models;

namespace TestBucket.Domain.Code.Features.ConventionalCommits.Parser;
internal class ConventionalCommitParser
{
    private static readonly Regex _regexBreakpointFooter = new Regex("^BREAKING CHANGE\\: ");
    private static readonly Regex _regexTypeAndMaybeScope = new Regex("^([a-zA-Z0-9-\\(\\)]+!?)\\: ");

    private enum ParserState
    {
        TypeLine,
        LongerDescription,
        Footer
    }

    private void ParseTypeLine(ConventionalCommitMessage message, string line)
    {
        var m1 = _regexTypeAndMaybeScope.Match(line);
        if (m1.Success)
        {
            ConventionalCommitType commitTypeAndScope = ParseType(message, m1, line);
            message.Types.Add(commitTypeAndScope);
            message.Description = commitTypeAndScope.Description;
        }
        else
        {
            message.Description = line;
        }
    }

    private static ConventionalCommitType ParseType(ConventionalCommitMessage message, Match m1, string line)
    {
        string type = m1.Value.TrimEnd(' ').TrimEnd(':');
        string? scope = null;

        if (type.EndsWith('!'))
        {
            message.IsBreakingChange = true;
            type = type[0..^1];
        }

        // Extract scope
        var p = type.IndexOf('(');
        if (p > 0 && type.EndsWith(')'))
        {
            scope = type[(p + 1)..^1];
            type = type[0..p];
        }

        // BREAKING-CHANGE MUST be synonymous with BREAKING CHANGE, when used as a token in a footer.
        if (type.Equals("BREAKING CHANGE", StringComparison.InvariantCultureIgnoreCase) || type.Equals("BREAKING-CHANGE", StringComparison.InvariantCultureIgnoreCase))
        {
            message.IsBreakingChange = true;
        }

        var commitTypeAndScope = new ConventionalCommitType { Type = type, Scope = scope };

        // Extract the description that follows :
        var descriptionDelimitedIndex = line.IndexOf(':');

        // Must end with ": " color and space, hence + 2.
        // This is included in the regex above
        commitTypeAndScope.Description = line[(descriptionDelimitedIndex + 2)..].TrimStart();


        return commitTypeAndScope;
    }

    private void ParseFooterLine(ConventionalCommitMessage message, string line)
    {
        var m1 = _regexTypeAndMaybeScope.Match(line);
        if (m1.Success)
        {
            ConventionalCommitType commitTypeAndScope = ParseType(message, m1, line);
            message.Footer.Add(commitTypeAndScope);
        }
        else
        {
            var m2 = _regexBreakpointFooter.Match(line);
            if (m2.Success)
            {
                ConventionalCommitType commitTypeAndScope = ParseType(message, m2, line);
                message.Footer.Add(commitTypeAndScope);
                message.IsBreakingChange = true;
            }
            else
            {
                if(message.Footer.Count > 0)
                {
                    message.Footer[^1].Description += $"\n{line}";
                }
            }
        }
    }

    public ConventionalCommitMessage Parse(string text)
    {
        var result = new ConventionalCommitMessage()
        {
            Description = "",
            LongerDescription = "",
            IsBreakingChange = false
        };

        var lines = text.Split('\n');
        var firstLine = lines[0].TrimEnd('\r');
        var state = ParserState.TypeLine;

        ParseTypeLine(result, firstLine);

        // Longer description and footer
        bool wasPrevLineNewLine = false;
        foreach (var line in lines.Skip(1).Select(x=>x.TrimEnd('\r')))
        {
            if(string.IsNullOrEmpty(line) && state == ParserState.TypeLine)
            {
                state = ParserState.LongerDescription;
                wasPrevLineNewLine = true;
                continue;
            }

            if (state == ParserState.TypeLine)
            {
                if (result.Description.Length == 0)
                {
                    result.Description = line;
                }
                else
                {
                    result.Description += $"\n{line}";
                }
            }

            // Detect state change from longer description to footer
            if (state == ParserState.LongerDescription)
            {
                if(wasPrevLineNewLine)
                {
                    // See if it is the start of the footer
                    if(_regexBreakpointFooter.IsMatch(line) || _regexTypeAndMaybeScope.IsMatch(line))
                    {
                        state = ParserState.Footer;
                    }
                }
            }
            if (state == ParserState.LongerDescription)
            {
                if (result.LongerDescription.Length == 0)
                {
                    result.LongerDescription = line;
                }
                else
                {
                    result.LongerDescription += $"\n{line}";
                }
            }
            
            if (state == ParserState.Footer)
            {
                ParseFooterLine(result, line);
            }

            // Keep track if the previous line was a new line to know if we should switch state
            wasPrevLineNewLine = string.IsNullOrEmpty(line);
        }

        return result;
    }
}
