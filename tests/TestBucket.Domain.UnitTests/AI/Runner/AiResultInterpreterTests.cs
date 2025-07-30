using TestBucket.Domain.AI.Runner;

namespace TestBucket.Domain.UnitTests.AI.Runner
{
    /// <summary>
    /// Contains unit tests for the <see cref="AiResultInterpreter"/> class, 
    /// specifically for verifying the behavior of the GetMarkdownParagraphs method.
    /// </summary>
    /// <remarks>
    /// These tests ensure that markdown input is correctly split into paragraphs,
    /// handling multi-line text, lists, and code blocks as expected.
    /// </remarks>
    [Feature("AI Runner")]
    [Component("AI")]
    [UnitTest]
    [FunctionalTest]
    [EnrichedTest]
    public class AiResultInterpreterTests
    {
        /// <summary>
        /// Verifies that <see cref="AiResultInterpreter.GetMarkdownParagraphs(string)"/> returns two paragraphs
        /// when the markdown contains two multi-line paragraphs separated by a blank line.
        /// </summary>
        [Fact]
        public void GetMarkdownParagraphs_WithTwoMultiLineParagraphs_ReturnsTwoStrings()
        {
            string markdown = """
                Paragraph 1
                Some text

                Paragraph 2
                Other text
                """;

            // Act
            var paragraphs = AiResultInterpreter.GetMarkdownParagraphs(markdown);

            Assert.Equal(2, paragraphs.Count);
            Assert.Equal("Paragraph 1\nSome text", paragraphs[0]);
            Assert.Equal("Paragraph 2\nOther text", paragraphs[1]);
        }

        /// <summary>
        /// Verifies that <see cref="AiResultInterpreter.GetMarkdownParagraphs(string)"/> returns two paragraphs
        /// when the markdown contains two single-line paragraphs separated by a blank line.
        /// </summary>
        [Fact]
        public void GetMarkdownParagraphs_WithTwoOneLineParagraphs_ReturnsTwoStrings()
        {
            string markdown = """
                Paragraph 1

                Paragraph 2
                """;

            // Act
            var paragraphs = AiResultInterpreter.GetMarkdownParagraphs(markdown);

            Assert.Equal(2, paragraphs.Count);
            Assert.Equal("Paragraph 1", paragraphs[0]);
            Assert.Equal("Paragraph 2", paragraphs[1]);
        }

        /// <summary>
        /// Verifies that <see cref="AiResultInterpreter.GetMarkdownParagraphs(string)"/> correctly handles a markdown list block,
        /// returning the list as one paragraph and the following text as another.
        /// </summary>
        [Fact]
        public void GetMarkdownParagraphs_WithListBlock_ReturnsTwoStrings()
        {
            string markdown = """
                - a
                - b
                - c
                Paragraph 2
                """;

            // Act
            var paragraphs = AiResultInterpreter.GetMarkdownParagraphs(markdown);

            Assert.Equal(2, paragraphs.Count);
            Assert.Equal("- a\n- b\n- c", paragraphs[0]);
            Assert.Equal("Paragraph 2", paragraphs[1]);
        }

        /// <summary>
        /// Verifies that <see cref="AiResultInterpreter.GetMarkdownParagraphs(string)"/> correctly handles a markdown code block,
        /// returning the code block as one paragraph and the following text as another.
        /// </summary>
        [Fact]
        public void GetMarkdownParagraphs_WithCodeBlock_ReturnsTwoStrings()
        {
            string markdown = """
                ```csharp
                Console.WriteLine("Hello, World!");
                ```
                Paragraph 2
                """;

            // Act
            var paragraphs = AiResultInterpreter.GetMarkdownParagraphs(markdown);

            Assert.Equal(2, paragraphs.Count);
            Assert.Equal("```csharp\nConsole.WriteLine(\"Hello, World!\");\n```", paragraphs[0]);
            Assert.Equal("Paragraph 2", paragraphs[1]);
        }
    }
}
