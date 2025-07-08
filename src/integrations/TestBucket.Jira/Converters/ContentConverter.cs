using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Jira.Models;

namespace TestBucket.Jira.Converters;

/// <summary>
/// Converts between markdown and jira content
/// </summary>
internal class ContentConverter
{
    public static Content FromMarkdown(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return new Content()
            {
                type = "doc",
                content = []
            };
        }

        var content = new Content()
        {
            type = "doc",
            content = []
        };

        var contentList = new List<Content>();
        var lines = markdown.Split('\n');
        var i = 0;

        while (i < lines.Length)
        {
            var line = lines[i].TrimEnd('\r');
            i++;

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            // Parse headings
            if (line.TrimStart().StartsWith('#'))
            {
                var heading = ParseHeading(line);
                contentList.Add(heading);
            }
            // Parse blockquotes
            else if (line.TrimStart().StartsWith("> "))
            {
                var blockquote = ParseBlockquote(line);
                contentList.Add(blockquote);
            }
            // Parse code blocks
            else if (line.Trim().StartsWith("```"))
            {
                var (codeBlock, newIndex) = ParseCodeBlock(lines, i - 1);
                contentList.Add(codeBlock);
                i = newIndex;
            }
            // Parse horizontal rule
            else if (line.Trim() == "---")
            {
                contentList.Add(new Content { type = "rule" });
            }
            // Parse bullet lists
            else if (line.TrimStart().StartsWith("- "))
            {
                var (list, newIndex) = ParseBulletList(lines, i - 1);
                contentList.Add(list);
                i = newIndex;
            }
            // Parse ordered lists
            else if (System.Text.RegularExpressions.Regex.IsMatch(line.TrimStart(), @"^\d+\.\s"))
            {
                var (list, newIndex) = ParseOrderedList(lines, i - 1);
                contentList.Add(list);
                i = newIndex;
            }
            // Parse tables
            else if (line.Contains('|'))
            {
                var (table, newIndex) = ParseTable(lines, i - 1);
                if (table != null)
                {
                    contentList.Add(table);
                    i = newIndex;
                }
                else
                {
                    // Not a table, treat as paragraph
                    var paragraph = ParseParagraph(line);
                    contentList.Add(paragraph);
                }
            }
            // Parse regular paragraphs
            else
            {
                var (paragraph, newIndex) = ParseParagraphBlock(lines, i - 1);
                contentList.Add(paragraph);
                i = newIndex;
            }
        }

        content.content = contentList.ToArray();
        return content;
    }

    private static Content ParseHeading(string line)
    {
        var trimmed = line.TrimStart();
        var level = 0;
        
        for (int i = 0; i < trimmed.Length && trimmed[i] == '#'; i++)
        {
            level++;
        }
        
        var text = trimmed.Substring(level).Trim();
        
        return new Content
        {
            type = "heading",
            attrs = new Dictionary<string, object> { ["level"] = level.ToString() },
            content = [new Content { type = "text", text = text }]
        };
    }

    private static Content ParseBlockquote(string line)
    {
        var text = line.TrimStart().Substring(2); // Remove "> "
        
        return new Content
        {
            type = "blockquote",
            content = [new Content 
            { 
                type = "paragraph",
                content = [new Content { type = "text", text = text }]
            }]
        };
    }

    private static (Content, int) ParseCodeBlock(string[] lines, int startIndex)
    {
        var openingLine = lines[startIndex].Trim();
        string? language = null;

        // Extract language from opening line (e.g., "```csharp" -> "csharp")
        if (openingLine.Length > 3)
        {
            language = openingLine.Substring(3).Trim();
            if (string.IsNullOrWhiteSpace(language))
            {
                language = null;
            }
        }

        var codeLines = new List<string>();
        var i = startIndex + 1; // Start after the opening ```

        while (i < lines.Length && lines[i].Trim() != "```")
        {
            codeLines.Add(lines[i].TrimEnd('\r'));
            i++;
        }

        if (i < lines.Length) i++; // Skip closing ```

        var codeText = string.Join("\n", codeLines);

        var codeBlock = new Content
        {
            type = "codeblock",
            content = [new Content { type = "text", text = codeText }]
        };

        // Add language to attrs if specified
        if (!string.IsNullOrEmpty(language))
        {
            codeBlock.attrs = new Dictionary<string, object> { ["language"] = language };
        }

        return (codeBlock, i);
    }

    private static (Content, int) ParseBulletList(string[] lines, int startIndex)
    {
        var listItems = new List<Content>();
        var i = startIndex;
        
        while (i < lines.Length)
        {
            var line = lines[i].TrimEnd('\r');
            if (string.IsNullOrWhiteSpace(line))
            {
                i++;
                continue;
            }
            
            if (!line.TrimStart().StartsWith("- "))
            {
                break;
            }
            
            var text = line.TrimStart().Substring(2); // Remove "- "
            listItems.Add(new Content
            {
                type = "listitem",
                content = [new Content 
                { 
                    type = "paragraph",
                    content = ParseInlineText(text)
                }]
            });
            
            i++;
        }
        
        return (new Content
        {
            type = "bulletlist",
            content = listItems.ToArray()
        }, i);
    }

    private static (Content, int) ParseOrderedList(string[] lines, int startIndex)
    {
        var listItems = new List<Content>();
        var i = startIndex;
        
        while (i < lines.Length)
        {
            var line = lines[i].TrimEnd('\r');
            if (string.IsNullOrWhiteSpace(line))
            {
                i++;
                continue;
            }
            
            var trimmed = line.TrimStart();
            var match = System.Text.RegularExpressions.Regex.Match(trimmed, @"^\d+\.\s(.*)");
            if (!match.Success)
            {
                break;
            }
            
            var text = match.Groups[1].Value;
            listItems.Add(new Content
            {
                type = "listitem",
                content = [new Content 
                { 
                    type = "paragraph",
                    content = ParseInlineText(text)
                }]
            });
            
            i++;
        }
        
        return (new Content
        {
            type = "orderedlist",
            content = listItems.ToArray()
        }, i);
    }

    private static (Content?, int) ParseTable(string[] lines, int startIndex)
    {
        var i = startIndex;
        var tableRows = new List<Content>();
        bool isFirstRow = true;
        
        while (i < lines.Length)
        {
            var line = lines[i].TrimEnd('\r');
            if (string.IsNullOrWhiteSpace(line) || !line.Contains('|'))
            {
                break;
            }
            
            // Skip separator row (e.g., "| --- | --- |")
            if (line.Trim().StartsWith('|') && line.Contains("---"))
            {
                i++;
                continue;
            }
            
            var cells = line.Split('|')
                .Where(cell => !string.IsNullOrWhiteSpace(cell))
                .Select(cell => cell.Trim())
                .ToArray();
            
            if (cells.Length == 0)
            {
                break;
            }
            
            var tableCells = cells.Select(cellText => new Content
            {
                type = isFirstRow ? "tableheader" : "tablecell",
                content = ParseInlineText(cellText)
            }).ToArray();
            
            tableRows.Add(new Content
            {
                type = "tablerow",
                content = tableCells
            });
            
            isFirstRow = false;
            i++;
        }
        
        if (tableRows.Count == 0)
        {
            return (null, i);
        }
        
        return (new Content
        {
            type = "table",
            content = tableRows.ToArray()
        }, i);
    }

    private static Content ParseParagraph(string text)
    {
        return new Content
        {
            type = "paragraph",
            content = ParseInlineText(text)
        };
    }

    private static (Content, int) ParseParagraphBlock(string[] lines, int startIndex)
    {
        var paragraphLines = new List<string>();
        var i = startIndex;
        
        while (i < lines.Length)
        {
            var line = lines[i].TrimEnd('\r');
            
            // Empty line ends paragraph
            if (string.IsNullOrWhiteSpace(line))
            {
                i++;
                break;
            }
            
            // Special markdown syntax starts new block
            if (line.TrimStart().StartsWith('#') || 
                line.TrimStart().StartsWith("> ") ||
                line.Trim() == "```" ||
                line.Trim() == "---" ||
                line.TrimStart().StartsWith("- ") ||
                System.Text.RegularExpressions.Regex.IsMatch(line.TrimStart(), @"^\d+\.\s"))
            {
                break;
            }
            
            paragraphLines.Add(line);
            i++;
        }
        
        var paragraphText = string.Join(" ", paragraphLines);
        
        return (new Content
        {
            type = "paragraph",
            content = ParseInlineText(paragraphText)
        }, i);
    }

    private static Content[] ParseInlineText(string text)
    {
        var parts = new List<Content>();
        var currentText = new StringBuilder();
        var i = 0;
        
        while (i < text.Length)
        {
            // Parse bold (**text**)
            if (i < text.Length - 1 && text[i] == '*' && text[i + 1] == '*')
            {
                if (currentText.Length > 0)
                {
                    parts.Add(new Content { type = "text", text = currentText.ToString() });
                    currentText.Clear();
                }
                
                var (boldText, newIndex) = ExtractMarkedText(text, i, "**", "**");
                if (boldText != null)
                {
                    parts.Add(new Content 
                    { 
                        type = "text", 
                        text = boldText,
                        marks = [new ContentMark { type = "strong" }]
                    });
                    i = newIndex;
                    continue;
                }
            }
            // Parse italic (*text*)
            else if (text[i] == '*')
            {
                if (currentText.Length > 0)
                {
                    parts.Add(new Content { type = "text", text = currentText.ToString() });
                    currentText.Clear();
                }
                
                var (italicText, newIndex) = ExtractMarkedText(text, i, "*", "*");
                if (italicText != null)
                {
                    parts.Add(new Content 
                    { 
                        type = "text", 
                        text = italicText,
                        marks = [new ContentMark { type = "em" }]
                    });
                    i = newIndex;
                    continue;
                }
            }
            // Parse strikethrough (~~text~~)
            else if (i < text.Length - 1 && text[i] == '~' && text[i + 1] == '~')
            {
                if (currentText.Length > 0)
                {
                    parts.Add(new Content { type = "text", text = currentText.ToString() });
                    currentText.Clear();
                }
                
                var (strikeText, newIndex) = ExtractMarkedText(text, i, "~~", "~~");
                if (strikeText != null)
                {
                    parts.Add(new Content 
                    { 
                        type = "text", 
                        text = strikeText,
                        marks = [new ContentMark { type = "strike" }]
                    });
                    i = newIndex;
                    continue;
                }
            }
            // Parse code (`text`)
            else if (text[i] == '`')
            {
                if (currentText.Length > 0)
                {
                    parts.Add(new Content { type = "text", text = currentText.ToString() });
                    currentText.Clear();
                }
                
                var (codeText, newIndex) = ExtractMarkedText(text, i, "`", "`");
                if (codeText != null)
                {
                    parts.Add(new Content 
                    { 
                        type = "text", 
                        text = codeText,
                        marks = [new ContentMark { type = "code" }]
                    });
                    i = newIndex;
                    continue;
                }
            }
            // Parse links ([text](url))
            else if (text[i] == '[')
            {
                var linkEndIndex = text.IndexOf(']', i);
                if (linkEndIndex != -1 && linkEndIndex + 1 < text.Length && text[linkEndIndex + 1] == '(')
                {
                    var urlEndIndex = text.IndexOf(')', linkEndIndex + 2);
                    if (urlEndIndex != -1)
                    {
                        if (currentText.Length > 0)
                        {
                            parts.Add(new Content { type = "text", text = currentText.ToString() });
                            currentText.Clear();
                        }

                        var linkText = text.Substring(i + 1, linkEndIndex - i - 1);
                        var url = text.Substring(linkEndIndex + 2, urlEndIndex - linkEndIndex - 2);

                        parts.Add(new Content
                        {
                            type = "text",
                            text = linkText,
                            marks = [new ContentMark
                            {
                                type = "link",
                                attrs = [new Dictionary<string, object> { ["href"] = url }]
                            }]
                        });
                        
                        i = urlEndIndex + 1;
                        continue;
                    }
                }
            }
            
            currentText.Append(text[i]);
            i++;
        }
        
        if (currentText.Length > 0)
        {
            parts.Add(new Content { type = "text", text = currentText.ToString() });
        }
        
        return parts.Count > 0 ? parts.ToArray() : [new Content { type = "text", text = "" }];
    }

    private static (string?, int) ExtractMarkedText(string text, int startIndex, string openMarker, string closeMarker)
    {
        var endIndex = text.IndexOf(closeMarker, startIndex + openMarker.Length);
        if (endIndex == -1)
        {
            return (null, startIndex + 1);
        }
        
        var markedText = text.Substring(startIndex + openMarker.Length, endIndex - startIndex - openMarker.Length);
        return (markedText, endIndex + closeMarker.Length);
    }

    public static string ToMarkdown(Content? content)
    {
        if(content is null)
        {
            return string.Empty;
        }

        var markdown = new StringBuilder();
        ConvertContentToMarkdown(content, markdown, 0, new JiraContentOptions());
        return markdown.ToString();
    }

    private static void ConvertContentToMarkdown(Content content, StringBuilder markdown, int depth, JiraContentOptions options)
    {
        if (!string.IsNullOrEmpty(content.text))
        {
            var textWithMarks = ApplyMarks(content.text, content.marks);
            markdown.Append(textWithMarks);
        }

        switch (content.type?.ToLowerInvariant())
        {
            case "doc":
                // Root document - process children
                if (content.content?.Length > 0)
                {
                    ProcessChildContent(content, markdown, depth, options);
                }
                break;

            case "paragraph":
                if (content.content?.Length > 0)
                {
                    ProcessChildContent(content, markdown, depth, options);
                }
                if ((options.ParagraphHandling & ParagraphHandling.NoNewLine) != ParagraphHandling.NoNewLine)
                {
                    markdown.Append('\n');
                }
                break;

            //case "text":
            //    if (!string.IsNullOrEmpty(content.text))
            //    {
            //        var textWithMarks = ApplyMarks(content.text, content.marks);
            //        markdown.Append(textWithMarks);
            //    }
            //    break;

            case "hardbreak":
                markdown.Append("  \n");
                break;

            case "heading":
                var level = GetHeadingLevel(content);
                markdown.Append(new string('#', level)).Append(' ');
                ProcessChildContent(content, markdown, depth, options);
                markdown.Append('\n');
                break;

            case "blockquote":
                markdown.Append("> ");
                ProcessChildContent(content, markdown, depth, options);
                markdown.Append('\n');
                break;

            case "codeblock":
                if (content.attrs is not null && content.attrs.Count > 0 && content.attrs.TryGetValue("language", out var language))
                {
                    markdown.Append($"```{language}\n");
                }
                else
                {
                    markdown.Append("```\n");
                }
                if (content.content?.Length > 0)
                {
                    ProcessChildContent(content, markdown, depth, options);
                    if (content.content[0].type == "text")
                    {
                        markdown.Append("\n");
                    }
                }

                markdown.Append("```\n");
                break;

            case "bulletlist":
                ProcessListItems(content, markdown, depth, "- ", options);
                break;

            case "orderedlist":
                ProcessOrderedListItems(content, markdown, depth, options);
                break;

            case "listitem":
                markdown.Append(new string(' ', depth * 2));
                ProcessChildContent(content, markdown, depth, options);
                markdown.Append('\n');
                break;

            case "mention":
                var displayName = GetMentionDisplayName(content);
                markdown.Append(displayName);
                break;

            case "emoji":
                var emojiText = GetEmojiText(content);
                markdown.Append(emojiText);
                break;

            case "inlinecard":
                var url = GetInlineCardUrl(content);
                markdown.Append("[").Append(url).Append("](").Append(url).Append(")");
                break;

            case "table":
                ProcessTable(content, markdown, depth, options);
                break;

            case "tablerow":
                ProcessTableRow(content, markdown, depth, options);
                break;

            case "tableheader":
            case "tablecell":
                markdown.Append("| ");
                options.ParagraphHandling = ParagraphHandling.NoNewLine;
                ProcessChildContent(content, markdown, depth, options);
                options.ParagraphHandling = ParagraphHandling.Default;
                markdown.Append(" ");
                break;

            case "rule":
                markdown.Append("---\n");
                break;

            default:
                // For unknown types, try to process children or use text content
                if (content.content?.Length > 0)
                {
                    ProcessChildContent(content, markdown, depth, options);
                }
                break;
        }
    }

    private static string ApplyMarks(string text, ContentMark[]? marks)
    {
        if (marks == null || string.IsNullOrEmpty(text))
        {
            return text;
        }

        foreach (var mark in marks)
        {
            // Handle trailing and leading spaces for inline marks
            // For example if text is "bold ", it need to be converted to "**bold** ", not "**bold **"

            int leadingSpaces = CountLeadingSpaces(text);
            int trailingSpaces = CountTrailingSpaces(text);
            text = text.Trim();

            text = mark.type?.ToLowerInvariant() switch
            {
                "strong" => $"**{text}**",
                "em" => $"*{text}*",
                "code" => $"`{text}`",
                "strike" => $"~~{text}~~",
                "underline" => $"<u>{text}</u>", // Markdown doesn't have native underline, use HTML
                "link" => $"[{text}]({GetMarkLinkHref(mark)})",
                "textcolor" => text, // Text color not supported in standard markdown
                "border" => text, // Border not supported in standard markdown
                "subsup" => text, // Subscript/superscript not directly supported in standard markdown
                _ => text
            };

            if (leadingSpaces > 0)
            {
                text = new string(' ', leadingSpaces) + text;
            }
            if (trailingSpaces > 0)
            {
                text = text + new string(' ', trailingSpaces);
            }
        }
        return text;
    }

    private static int CountTrailingSpaces(string text)
    {
        if (text.Length == 0) return 0;

        int index = text.Length - 1;
        int count = 0;
        while (index >= 0)
        {
            if (text[index] != ' ')
            {
                break;
            }
            index--;
            count++;
        }
        return count;
    }

    private static int CountLeadingSpaces(string text)
    {
        if (text.Length == 0) return 0;

        int count = 0;
        while(text.Length > count)
        {
            if (text[count] != ' ')
            {
                break;
            }
            count++;
        }

        return count;
    }

    private static void ProcessChildContent(Content content, StringBuilder markdown, int depth, JiraContentOptions options)
    {
        if (content.content != null)
        {
            foreach (var child in content.content)
            {
                ConvertContentToMarkdown(child, markdown, depth, options);
            }
        }
    }

    private static void ProcessListItems(Content content, StringBuilder markdown, int depth, string listMarker, JiraContentOptions options)
    {
        if (content.content != null)
        {
            foreach (var item in content.content)
            {
                if (item.type?.ToLowerInvariant() == "listitem")
                {
                    markdown.Append(new string(' ', depth * 2)).Append(listMarker);
                    ProcessChildContent(item, markdown, depth + 1, options);
                    markdown.Append('\n');
                }
            }
        }
    }

    private static void ProcessOrderedListItems(Content content, StringBuilder markdown, int depth, JiraContentOptions options)
    {
        if (content.content != null)
        {
            for (int i = 0; i < content.content.Length; i++)
            {
                var item = content.content[i];
                if (item.type?.ToLowerInvariant() == "listitem")
                {
                    markdown.Append(new string(' ', depth * 2)).Append($"{i + 1}. ");
                    ProcessChildContent(item, markdown, depth + 1, options);
                    markdown.Append('\n');
                }
            }
        }
    }

    private static void ProcessTable(Content content, StringBuilder markdown, int depth, JiraContentOptions options)
    {
        if (content.content != null)
        {
            bool isFirstRow = true;
            foreach (var row in content.content)
            {
                if (row.type?.ToLowerInvariant() == "tablerow")
                {
                    ConvertContentToMarkdown(row, markdown, depth, options);
                    markdown.Append("|\n");
                    
                    // Add header separator after first row
                    if (isFirstRow && row.content != null)
                    {
                        markdown.Append("|");
                        for (int i = 0; i < row.content.Length; i++)
                        {
                            markdown.Append(" --- |");
                        }
                        markdown.Append('\n');
                        isFirstRow = false;
                    }
                }
            }
        }
    }

    private static void ProcessTableRow(Content content, StringBuilder markdown, int depth, JiraContentOptions options)
    {
        if (content.content != null)
        {
            foreach (var cell in content.content)
            {
                options.ParagraphHandling = ParagraphHandling.NoNewLine;
                ConvertContentToMarkdown(cell, markdown, depth, options);
                options.ParagraphHandling = ParagraphHandling.Default;
            }
        }
    }

    private static int GetHeadingLevel(Content content)
    {
        // Try to extract heading level from attributes or default to 1
        // In ADF, heading level is typically stored in attrs.level
        if (content.attrs is not null && content.attrs.TryGetValue("level" , out var levelString) && int.TryParse(levelString?.ToString(), out var level))
        {
            return level;
        }

        return 1; // Default heading level
    }

    //private static string GetLinkHref(Content content)
    //{
    //    // In ADF, link href is typically stored in attrs.href
    //    if (content.attrs is not null && content.attrs.TryGetValue("href", out var href) && href is not null)
    //    {
    //        return href.ToString();
    //    }
    //    return "#"; // Default placeholder
    //}

    private static string GetMarkLinkHref(ContentMark mark)
    {
        // In ADF, link href is typically stored in mark attrs
        // In ADF, link href is typically stored in attrs.href
        if (mark.attrs is not null)
        {
            foreach(var attr in mark.attrs)
            {
                if(attr.TryGetValue("href", out var href) && href is not null)
                {
                    return href.ToString() ?? "#";
                }
            }
        }
        return "#"; // Default placeholder
    }

    private static string GetMentionDisplayName(Content content)
    {
        // In ADF, mention display name is typically stored in attrs.text or attrs.displayName
        if (content.attrs is not null)
        {
            if (content.attrs.TryGetValue("displayName", out var displayName) && displayName is not null)
            {
                return displayName.ToString() ?? "@" + content.text ?? "@user";
            }
            if (content.attrs.TryGetValue("text", out var text) && text is not null)
            {
                return text.ToString() ?? "@" + content.text ?? "@user";
            }
        }
        return content.text ?? "@user";
    }

    private static string GetEmojiText(Content content)
    {
        // In ADF, emoji text is typically stored in attrs.text or attrs.shortName
        // text contains the actual emoji and shortName contains the code, as :grinning:
        if (content.text is null && content.attrs is not null && content.attrs.TryGetValue("shortName", out var shortName) && shortName is not null)
        {
            var ret = shortName.ToString();
            if (ret is not null)
            {
                return ret;
            }
        }

        return content.text ?? ":emoji:";
    }

    private static string GetInlineCardUrl(Content content)
    {
        // In ADF, inline card URL is typically stored in attrs.url
        return content.text ?? "#";
    }
}
