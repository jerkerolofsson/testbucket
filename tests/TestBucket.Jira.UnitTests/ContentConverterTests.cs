using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Traits.Xunit;
using TestBucket.Jira.Converters;
using TestBucket.Jira.Models;

namespace TestBucket.Jira.UnitTests
{
    /// <summary>
    /// Contains unit tests for the <see cref="ContentConverter"/> class, verifying correct conversion between Jira's Atlassian Document Format (ADF) and Markdown format.
    /// Tests both ToMarkdown and FromMarkdown conversion methods to ensure bidirectional compatibility and proper handling of various content types.
    /// </summary>
    [EnrichedTest]
    [FunctionalTest]
    [UnitTest]
    [Component("Jira")]
    [Feature("Jira")]
    public class ContentConverterTests
    {
        #region ToMarkdown Tests

        /// <summary>
        /// Verifies that converting null or empty content to markdown returns an empty string.
        /// </summary>
        [Fact]
        public void ToMarkdown_EmptyContent_ReturnsEmptyString()
        {
            // Arrange
            Content? content = null;

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        /// <summary>
        /// Verifies that simple text content is correctly converted to markdown format with proper paragraph handling.
        /// </summary>
        [Fact]
        public void ToMarkdown_SimpleText_ReturnsMarkdown()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content
                    {
                        type = "paragraph",
                        content = [
                            new Content { type = "text", text = "Hello World" }
                        ]
                    }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal("Hello World\n", result);
        }

        /// <summary>
        /// Verifies that text with bold formatting (strong marks) is correctly converted to markdown bold syntax.
        /// </summary>
        [Fact]
        public void ToMarkdown_BoldTextWithTrailingSpace_ReturnsMarkdownWithBold()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content
                    {
                        type = "paragraph",
                        content = [
                            new Content
                            {
                                type = "text",
                                text = "bold text ",
                                marks = [new ContentMark { type = "strong" }]
                            }
                        ]
                    }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal("**bold text** \n", result);
        }

        /// <summary>
        /// Verifies that text with bold formatting (strong marks) is correctly converted to markdown bold syntax.
        /// </summary>
        [Fact]
        public void ToMarkdown_BoldText_ReturnsMarkdownWithBold()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content
                    {
                        type = "paragraph",
                        content = [
                            new Content
                            {
                                type = "text",
                                text = "bold text",
                                marks = [new ContentMark { type = "strong" }]
                            }
                        ]
                    }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal("**bold text**\n", result);
        }

        /// <summary>
        /// Verifies that text with italic formatting (em marks) is correctly converted to markdown italic syntax.
        /// </summary>
        [Fact]
        public void ToMarkdown_ItalicText_ReturnsMarkdownWithItalic()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content
                    {
                        type = "paragraph",
                        content = [
                            new Content
                            {
                                type = "text",
                                text = "italic text",
                                marks = [new ContentMark { type = "em" }]
                            }
                        ]
                    }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal("*italic text*\n", result);
        }

        /// <summary>
        /// Verifies that text with code formatting (code marks) is correctly converted to markdown inline code syntax.
        /// </summary>
        [Fact]
        public void ToMarkdown_CodeText_ReturnsMarkdownWithCode()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content
                    {
                        type = "paragraph",
                        content = [
                            new Content
                            {
                                type = "text",
                                text = "code text",
                                marks = [new ContentMark { type = "code" }]
                            }
                        ]
                    }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal("`code text`\n", result);
        }

        /// <summary>
        /// Verifies that text with strikethrough formatting (strike marks) is correctly converted to markdown strikethrough syntax.
        /// </summary>
        [Fact]
        public void ToMarkdown_StrikeThroughText_ReturnsMarkdownWithStrike()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content
                    {
                        type = "paragraph",
                        content = [
                            new Content
                            {
                                type = "text",
                                text = "strikethrough text",
                                marks = [new ContentMark { type = "strike" }]
                            }
                        ]
                    }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal("~~strikethrough text~~\n", result);
        }

        /// <summary>
        /// Verifies that heading content with level 1 is correctly converted to markdown H1 syntax.
        /// </summary>
        [Fact]
        public void ToMarkdown_HeadingLevel1_ReturnsMarkdownHeading()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content
                    {
                        type = "heading",
                        attrs = new Dictionary<string, object> { ["level"] = "1" },
                        content = [
                            new Content { type = "text", text = "Heading Text" }
                        ]
                    }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal("# Heading Text\n", result);
        }

        /// <summary>
        /// Verifies that heading content with level 2 is correctly converted to markdown H2 syntax.
        /// </summary>
        [Fact]
        public void ToMarkdown_HeadingLevel2_ReturnsMarkdownHeading()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content
                    {
                        type = "heading",
                        attrs = new Dictionary<string, object> { ["level"] = "2" },
                        content = [
                            new Content { type = "text", text = "Heading Text" }
                        ]
                    }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal("## Heading Text\n", result);
        }

        /// <summary>
        /// Verifies that heading content with level 3 is correctly converted to markdown H3 syntax.
        /// </summary>
        [Fact]
        public void ToMarkdown_HeadingLevel3_ReturnsMarkdownHeading()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content
                    {
                        type = "heading",
                        attrs = new Dictionary<string, object> { ["level"] = "3" },
                        content = [
                            new Content { type = "text", text = "Heading Text" }
                        ]
                    }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal("### Heading Text\n", result);
        }

        /// <summary>
        /// Verifies that blockquote content is correctly converted to markdown blockquote syntax.
        /// </summary>
        [Fact]
        public void ToMarkdown_Blockquote_ReturnsMarkdownBlockquote()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content
                    {
                        type = "blockquote",
                        content = [
                            new Content
                            {
                                type = "paragraph",
                                content = [
                                    new Content { type = "text", text = "This is a quote" }
                                ]
                            }
                        ]
                    }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal("> This is a quote\n\n", result);
        }

        /// <summary>
        /// Verifies that code block content with language specification is correctly converted to markdown fenced code block syntax.
        /// </summary>
        [Fact]
        public void ToMarkdown_CodeBlock_ReturnsMarkdownCodeBlock()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content
                    {
                        type = "codeblock",
                        attrs = new Dictionary<string, object> { ["language"] = "csharp" },
                        content = [
                            new Content
                            {
                                type = "paragraph",
                                content = [
                                    new Content { type = "text", text = "var x = 5;" }
                                ]
                            }
                        ]
                    }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal("```csharp\nvar x = 5;\n```\n", result);
        }


        /// <summary>
        /// Verifies that a code block with text content (no paragraph) adds a new line before the end of the code block
        /// </summary>
        [Fact]
        public void ToMarkdown_CodeBlockWithText_AddsNewLineBeforeEndOfCodeBlock()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content
                    {
                        type = "codeblock",
                        attrs = new Dictionary<string, object> { ["language"] = "csharp" },
                        content = [
                            new Content { type = "text", text = "var x = 5;" }
                        ]
                    }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal("```csharp\nvar x = 5;\n```\n", result);
        }

        /// <summary>
        /// Verifies that bullet list content is correctly converted to markdown unordered list syntax.
        /// </summary>
        [Fact]
        public void ToMarkdown_BulletList_ReturnsMarkdownList()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content
                    {
                        type = "bulletlist",
                        content = [
                            new Content
                            {
                                type = "listitem",
                                content = [
                                    new Content
                                    {
                                        type = "paragraph",
                                        content = [
                                            new Content { type = "text", text = "First item" }
                                        ]
                                    }
                                ]
                            },
                            new Content
                            {
                                type = "listitem",
                                content = [
                                    new Content
                                    {
                                        type = "paragraph",
                                        content = [
                                            new Content { type = "text", text = "Second item" }
                                        ]
                                    }
                                ]
                            }
                        ]
                    }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal("- First item\n\n- Second item\n\n", result);
        }

        /// <summary>
        /// Verifies that horizontal rule content is correctly converted to markdown horizontal rule syntax.
        /// </summary>
        [Fact]
        public void ToMarkdown_HorizontalRule_ReturnsMarkdownRule()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content { type = "rule" }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal("---\n", result);
        }

        /// <summary>
        /// Verifies that table content with headers and data rows is correctly converted to markdown table syntax.
        /// </summary>
        [Fact]
        public void ToMarkdown_Table_ReturnsMarkdownTable()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content
                    {
                        type = "table",
                        content = [
                            // Header row
                            new Content
                            {
                                type = "tablerow",
                                content = [
                                    new Content
                                    {
                                        type = "tableheader",
                                        content = [
                                            new Content { type = "text", text = "Header 1" }
                                        ]
                                    },
                                    new Content
                                    {
                                        type = "tableheader",
                                        content = [
                                            new Content { type = "text", text = "Header 2" }
                                        ]
                                    }
                                ]
                            },
                            // First data row
                            new Content
                            {
                                type = "tablerow",
                                content = [
                                    new Content
                                    {
                                        type = "tablecell",
                                        content = [
                                            new Content { type = "text", text = "Cell 1" }
                                        ]
                                    },
                                    new Content
                                    {
                                        type = "tablecell",
                                        content = [
                                            new Content { type = "text", text = "Cell 2" }
                                        ]
                                    }
                                ]
                            },
                            // Second data row
                            new Content
                            {
                                type = "tablerow",
                                content = [
                                    new Content
                                    {
                                        type = "tablecell",
                                        content = [
                                            new Content { type = "text", text = "Cell 3" }
                                        ]
                                    },
                                    new Content
                                    {
                                        type = "tablecell",
                                        content = [
                                            new Content { type = "text", text = "Cell 4" }
                                        ]
                                    }
                                ]
                            }
                        ]
                    }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            var expectedMarkdown = "| Header 1 | Header 2 |\n| --- | --- |\n| Cell 1 | Cell 2 |\n| Cell 3 | Cell 4 |\n";
            Assert.Equal(expectedMarkdown, result);
        }

        /// <summary>
        /// Verifies that mention content with attributes is correctly converted to display name format.
        /// </summary>
        [Fact]
        public void ToMarkdown_Mention_ReturnsDisplayName()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content
                    {
                        type = "paragraph",
                        content = [
                            new Content
                            {
                                type = "mention",
                                attrs = new Dictionary<string, object>
                                {
                                    ["id"] = "712020:5b260e1f-3a08-41c7-bfc1-1f159194e1a4",
                                    ["text"] = "@Jerker Olofsson",
                                    ["accessLevel"] = "",
                                    ["localId"] = "094e2253-80a7-4f2e-9929-708c134ca4e8"
                                }
                            }
                        ]
                    }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal("@Jerker Olofsson\n", result);
        }

        /// <summary>
        /// Verifies that mention content with displayName attribute uses the displayName over the text attribute.
        /// </summary>
        [Fact]
        public void ToMarkdown_MentionWithDisplayName_ReturnsDisplayName()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content
                    {
                        type = "paragraph",
                        content = [
                            new Content
                            {
                                type = "mention",
                                attrs = new Dictionary<string, object>
                                {
                                    ["id"] = "712020:5b260e1f-3a08-41c7-bfc1-1f159194e1a4",
                                    ["displayName"] = "@John Doe",
                                    ["text"] = "@Jerker Olofsson",
                                    ["accessLevel"] = "",
                                    ["localId"] = "094e2253-80a7-4f2e-9929-708c134ca4e8"
                                }
                            }
                        ]
                    }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal("@John Doe\n", result);
        }

        /// <summary>
        /// Verifies that mention content without proper attributes falls back to default user format.
        /// </summary>
        [Fact]
        public void ToMarkdown_MentionWithoutAttributes_ReturnsDefaultUser()
        {
            // Arrange
            var content = new Content
            {
                type = "doc",
                content = [
                    new Content
                    {
                        type = "paragraph",
                        content = [
                            new Content
                            {
                                type = "mention"
                            }
                        ]
                    }
                ]
            };

            // Act
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            Assert.Equal("@user\n", result);
        }

        #endregion

        #region FromMarkdown Tests

        /// <summary>
        /// Verifies that converting an empty string from markdown returns a document structure with empty content.
        /// </summary>
        [Fact]
        public void FromMarkdown_EmptyString_ReturnsEmptyDocContent()
        {
            // Arrange
            var markdown = "";

            // Act
            var result = ContentConverter.FromMarkdown(markdown);

            // Assert
            Assert.Equal("doc", result.type);
            Assert.Empty(result.content!);
        }

        /// <summary>
        /// Verifies that converting whitespace-only string from markdown returns a document structure with empty content.
        /// </summary>
        [Fact]
        public void FromMarkdown_WhitespaceOnly_ReturnsEmptyDocContent()
        {
            // Arrange
            var markdown = "   \n  \t  \n";

            // Act
            var result = ContentConverter.FromMarkdown(markdown);

            // Assert
            Assert.Equal("doc", result.type);
            Assert.Empty(result.content!);
        }

        /// <summary>
        /// Verifies that simple text markdown is correctly converted to a paragraph content structure.
        /// </summary>
        [Fact]
        public void FromMarkdown_SimpleText_ReturnsParagraphContent()
        {
            // Arrange
            var markdown = "Hello World";

            // Act
            var result = ContentConverter.FromMarkdown(markdown);

            // Assert
            Assert.Equal("doc", result.type);
            Assert.Single(result.content!);

            var paragraph = result.content![0];
            Assert.Equal("paragraph", paragraph.type);
            Assert.Single(paragraph.content!);

            var text = paragraph.content![0];
            Assert.Equal("text", text.type);
            Assert.Equal("Hello World", text.text);
        }

        /// <summary>
        /// Verifies that markdown bold syntax is correctly converted to content with strong marks.
        /// </summary>
        [Fact]
        public void FromMarkdown_BoldText_ReturnsTextWithStrongMark()
        {
            // Arrange
            var markdown = "This is **bold** text";

            // Act
            var result = ContentConverter.FromMarkdown(markdown);

            // Assert
            var paragraph = result.content![0];
            Assert.Equal(3, paragraph.content!.Length);

            var boldText = paragraph.content![1];
            Assert.Equal("bold", boldText.text);
            Assert.Single(boldText.marks!);
            Assert.Equal("strong", boldText.marks![0].type);
        }

        /// <summary>
        /// Verifies that markdown italic syntax is correctly converted to content with em marks.
        /// </summary>
        [Fact]
        public void FromMarkdown_ItalicText_ReturnsTextWithEmMark()
        {
            // Arrange
            var markdown = "This is *italic* text";

            // Act
            var result = ContentConverter.FromMarkdown(markdown);

            // Assert
            var paragraph = result.content![0];
            Assert.Equal(3, paragraph.content!.Length);

            var italicText = paragraph.content![1];
            Assert.Equal("italic", italicText.text);
            Assert.Single(italicText.marks!);
            Assert.Equal("em", italicText.marks![0].type);
        }

        /// <summary>
        /// Verifies that markdown inline code syntax is correctly converted to content with code marks.
        /// </summary>
        [Fact]
        public void FromMarkdown_CodeText_ReturnsTextWithCodeMark()
        {
            // Arrange
            var markdown = "Use `var x = 5;` for variables";

            // Act
            var result = ContentConverter.FromMarkdown(markdown);

            // Assert
            var paragraph = result.content![0];
            Assert.Equal(3, paragraph.content!.Length);

            var codeText = paragraph.content![1];
            Assert.Equal("var x = 5;", codeText.text);
            Assert.Single(codeText.marks!);
            Assert.Equal("code", codeText.marks![0].type);
        }

        /// <summary>
        /// Verifies that markdown strikethrough syntax is correctly converted to content with strike marks.
        /// </summary>
        [Fact]
        public void FromMarkdown_StrikeThroughText_ReturnsTextWithStrikeMark()
        {
            // Arrange
            var markdown = "This is ~~strikethrough~~ text";

            // Act
            var result = ContentConverter.FromMarkdown(markdown);

            // Assert
            var paragraph = result.content![0];
            Assert.Equal(3, paragraph.content!.Length);

            var strikeText = paragraph.content![1];
            Assert.Equal("strikethrough", strikeText.text);
            Assert.Single(strikeText.marks!);
            Assert.Equal("strike", strikeText.marks![0].type);
        }

        /// <summary>
        /// Verifies that markdown link syntax is correctly converted to content with link marks containing href attributes.
        /// </summary>
        [Fact]
        public void FromMarkdown_Link_ReturnsTextWithLinkMark()
        {
            // Arrange
            var markdown = "Visit [GitHub](https://github.com) for code";

            // Act
            var result = ContentConverter.FromMarkdown(markdown);

            // Assert
            var paragraph = result.content![0];
            Assert.Equal(3, paragraph.content!.Length);

            var linkText = paragraph.content![1];
            Assert.Equal("GitHub", linkText.text);
            Assert.Single(linkText.marks!);
            Assert.Equal("link", linkText.marks![0].type);
            Assert.Single(linkText.marks![0].attrs!);
            Assert.Equal("https://github.com", linkText.marks![0].attrs![0]["href"]);
        }

        /// <summary>
        /// Verifies that markdown heading syntax is correctly converted to heading content with appropriate level attributes.
        /// </summary>
        [Fact]
        public void FromMarkdown_Heading_ReturnsHeadingContent()
        {
            // Arrange
            var markdown = "## This is a heading";

            // Act
            var result = ContentConverter.FromMarkdown(markdown);

            // Assert
            Assert.Single(result.content!);

            var heading = result.content![0];
            Assert.Equal("heading", heading.type);
            Assert.Equal("2", heading.attrs!["level"]);
            Assert.Single(heading.content!);
            Assert.Equal("This is a heading", heading.content![0].text);
        }

        /// <summary>
        /// Verifies that markdown blockquote syntax is correctly converted to blockquote content structure.
        /// </summary>
        [Fact]
        public void FromMarkdown_Blockquote_ReturnsBlockquoteContent()
        {
            // Arrange
            var markdown = "> This is a quote";

            // Act
            var result = ContentConverter.FromMarkdown(markdown);

            // Assert
            Assert.Single(result.content!);

            var blockquote = result.content![0];
            Assert.Equal("blockquote", blockquote.type);
            Assert.Single(blockquote.content!);

            var paragraph = blockquote.content![0];
            Assert.Equal("paragraph", paragraph.type);
            Assert.Equal("This is a quote", paragraph.content![0].text);
        }

        /// <summary>
        /// Verifies that markdown fenced code block with language specification is correctly converted to codeblock content.
        /// </summary>
        [Fact]
        public void FromMarkdown_CodeBlock_ReturnsCodeBlockContent()
        {
            // Arrange
            var markdown = "```csharp\nvar x = 5;\nConsole.WriteLine(x);\n```";

            // Act
            var result = ContentConverter.FromMarkdown(markdown);

            // Assert
            Assert.Single(result.content!);

            var codeBlock = result.content![0];
            Assert.Equal("codeBlock", codeBlock.type);
            Assert.Equal("csharp", codeBlock.attrs!["language"]);
            Assert.Single(codeBlock.content!);
            Assert.Equal("var x = 5;\nConsole.WriteLine(x);", codeBlock.content![0].text);
        }

        /// <summary>
        /// Verifies that markdown fenced code block without language specification is correctly converted to codeblock content without language attributes.
        /// </summary>
        [Fact]
        public void FromMarkdown_CodeBlockWithoutLanguage_ReturnsCodeBlockContentWithoutLanguageAttr()
        {
            // Arrange
            var markdown = "```\nsome code\n```";

            // Act
            var result = ContentConverter.FromMarkdown(markdown);

            // Assert
            Assert.Single(result.content!);

            var codeBlock = result.content![0];
            Assert.Equal("codeBlock", codeBlock.type);
            Assert.Null(codeBlock.attrs);
            Assert.Single(codeBlock.content!);
            Assert.Equal("some code", codeBlock.content![0].text);
        }

        /// <summary>
        /// Verifies that markdown unordered list syntax is correctly converted to bulletlist content structure.
        /// </summary>
        [Fact]
        public void FromMarkdown_BulletList_ReturnsBulletListContent()
        {
            // Arrange
            var markdown = "- First item\n- Second item\n- Third item";

            // Act
            var result = ContentConverter.FromMarkdown(markdown);

            // Assert
            Assert.Single(result.content!);

            var bulletList = result.content![0];
            Assert.Equal("bulletList", bulletList.type);
            Assert.Equal(3, bulletList.content!.Length);

            foreach (var item in bulletList.content!)
            {
                Assert.Equal("listItem", item.type);
                Assert.Single(item.content!);
                Assert.Equal("paragraph", item.content![0].type);
            }

            Assert.Equal("First item", bulletList.content![0].content![0].content![0].text);
            Assert.Equal("Second item", bulletList.content![1].content![0].content![0].text);
            Assert.Equal("Third item", bulletList.content![2].content![0].content![0].text);
        }

        /// <summary>
        /// Verifies that markdown ordered list syntax is correctly converted to orderedlist content structure.
        /// </summary>
        [Fact]
        public void FromMarkdown_OrderedList_ReturnsOrderedListContent()
        {
            // Arrange
            var markdown = "1. First item\n2. Second item\n3. Third item";

            // Act
            var result = ContentConverter.FromMarkdown(markdown);

            // Assert
            Assert.Single(result.content!);

            var orderedList = result.content![0];
            Assert.Equal("orderedlist", orderedList.type);
            Assert.Equal(3, orderedList.content!.Length);

            foreach (var item in orderedList.content!)
            {
                Assert.Equal("listItem", item.type);
                Assert.Single(item.content!);
                Assert.Equal("paragraph", item.content![0].type);
            }

            Assert.Equal("First item", orderedList.content![0].content![0].content![0].text);
            Assert.Equal("Second item", orderedList.content![1].content![0].content![0].text);
            Assert.Equal("Third item", orderedList.content![2].content![0].content![0].text);
        }

        /// <summary>
        /// Verifies that markdown horizontal rule syntax is correctly converted to rule content.
        /// </summary>
        [Fact]
        public void FromMarkdown_HorizontalRule_ReturnsRuleContent()
        {
            // Arrange
            var markdown = "---";

            // Act
            var result = ContentConverter.FromMarkdown(markdown);

            // Assert
            Assert.Single(result.content!);

            var rule = result.content![0];
            Assert.Equal("rule", rule.type);
        }

        /// <summary>
        /// Verifies that markdown table syntax is correctly converted to table content structure with proper headers and cells.
        /// </summary>
        [Fact]
        public void FromMarkdown_Table_ReturnsTableContent()
        {
            // Arrange
            var markdown = """
                | Header 1 | Header 2 |
                | --- | --- |
                | Cell 1 | Cell 2 |
                | Cell 3 | Cell 4 |
                """;

            // Act
            var result = ContentConverter.FromMarkdown(markdown);

            // Assert
            Assert.Single(result.content!);

            var table = result.content![0];
            Assert.Equal("table", table.type);
            Assert.Equal(3, table.content!.Length); // Header row + 2 data rows

            // Check header row
            var headerRow = table.content![0];
            Assert.Equal("tablerow", headerRow.type);
            Assert.Equal(2, headerRow.content!.Length);
            Assert.Equal("tableheader", headerRow.content![0].type);
            Assert.Equal("tableheader", headerRow.content![1].type);

            // Check data rows
            var dataRow1 = table.content![1];
            Assert.Equal("tablerow", dataRow1.type);
            Assert.Equal("tablecell", dataRow1.content![0].type);
            Assert.Equal("tablecell", dataRow1.content![1].type);
        }

        /// <summary>
        /// Verifies that markdown with multiple paragraphs separated by blank lines is correctly converted to multiple paragraph content structures.
        /// </summary>
        [Fact]
        public void FromMarkdown_MultipleParagraphs_ReturnsMultipleParagraphContent()
        {
            // Arrange
            var markdown = "First paragraph.\n\nSecond paragraph.";

            // Act
            var result = ContentConverter.FromMarkdown(markdown);

            // Assert
            Assert.Equal(2, result.content!.Length);

            Assert.Equal("paragraph", result.content![0].type);
            Assert.Equal("First paragraph.", result.content![0].content![0].text);

            Assert.Equal("paragraph", result.content![1].type);
            Assert.Equal("Second paragraph.", result.content![1].content![0].text);
        }

        #endregion

        #region Round-trip Tests

        /// <summary>
        /// Verifies that simple markdown content can be converted to ADF format and back to markdown while preserving the original content.
        /// </summary>
        /// <param name="markdown">The markdown content to test round-trip conversion for.</param>
        [Theory]
        [InlineData("Simple text")]
        [InlineData("**Bold text**")]
        [InlineData("*Italic text*")]
        [InlineData("`Code text`")]
        [InlineData("~~Strikethrough text~~")]
        [InlineData("## Heading")]
        [InlineData("> Blockquote")]
        [InlineData("- List item")]
        [InlineData("1. Ordered item")]
        [InlineData("---")]
        public void RoundTrip_SimpleMarkdown_PreservesContent(string markdown)
        {
            // Act
            var content = ContentConverter.FromMarkdown(markdown);
            var result = ContentConverter.ToMarkdown(content).TrimEnd();

            // Assert
            Assert.Equal(markdown, result);
        }

        /// <summary>
        /// Verifies that complex markdown with multiple content types can be converted to ADF format and back while preserving the overall structure and content.
        /// </summary>
        [Fact]
        public void RoundTrip_ComplexMarkdown_PreservesStructure()
        {
            // Arrange
            var markdown = "# Main Heading\n\nThis is a paragraph with **bold** and *italic* text.\n\n## Subheading\n\n- First item\n- Second item\n\n```csharp\nvar x = 5;\n```\n\n> This is a quote\n\n---";

            // Act
            var content = ContentConverter.FromMarkdown(markdown);
            var result = ContentConverter.ToMarkdown(content);

            // Assert
            // Verify structure is preserved even if exact formatting differs slightly
            Assert.Contains("# Main Heading", result);
            Assert.Contains("**bold**", result);
            Assert.Contains("*italic*", result);
            Assert.Contains("## Subheading", result);
            Assert.Contains("- First item", result);
            Assert.Contains("- Second item", result);
            Assert.Contains("```csharp", result);
            Assert.Contains("var x = 5;", result);
            Assert.Contains("> This is a quote", result);
            Assert.Contains("---", result);
        }

        #endregion
    }
}