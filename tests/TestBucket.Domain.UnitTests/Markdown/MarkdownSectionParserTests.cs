using TestBucket.Domain.Markdown;

#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.

namespace TestBucket.Domain.UnitTests.Markdown
{
    /// <summary>
    /// Contains unit tests for the <see cref="MarkdownSectionParser"/> class, verifying correct parsing of markdown sections and header hierarchies.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    public class MarkdownSectionParserTests
    {
        /// <summary>
        /// Verifies that a single line of markdown is parsed as one section.
        /// </summary>
        [Fact]
        public void ReadSections_WithSingleLine_OneSection()
        {
            var sections = MarkdownSectionParser.ReadSections("Hello World", TestContext.Current.CancellationToken).ToList();
            Assert.Single(sections);
        }

        /// <summary>
        /// Verifies that multiple lines without headers are parsed as a single section.
        /// </summary>
        [Fact]
        public void ReadSections_WithMultipleLines_OneSection()
        {
            var sections = MarkdownSectionParser.ReadSections("Hello World\nThis is still the same section", TestContext.Current.CancellationToken).ToList();
            Assert.Single(sections);
        }

        /// <summary>
        /// Verifies that two headers in markdown result in two sections.
        /// </summary>
        [Fact]
        public void ReadSections_WithTwoHeaders_TwoSectionsReturned()
        {
            var sections = MarkdownSectionParser.ReadSections("# Section 1\n\n#Section 2", TestContext.Current.CancellationToken).ToList();
            Assert.Equal(2, sections.Count);
        }

        /// <summary>
        /// Verifies that the path property is set correctly for sections with a header hierarchy.
        /// </summary>
        [Fact]
        public void ReadSections_WithHeaderHierarchy_PathSetFromHierarchy()
        {
            // lang=markdown
            var markdown = """
                # Header-1
                Text belong to Header-1

                ## Header-1.1

                ### Header-1.1.1
                Text belong to Header-1.1.1

                ### Header-1.1.2
                Text belong to Header-1.1.2

                ## Header-1.2
                Text belong to Header-1.2

                # Header-2
                Text belong to Header-2

                ## Header-2.1
                Text belong to Header-2.1

                """;

            var sections = MarkdownSectionParser.ReadSections(markdown, TestContext.Current.CancellationToken).ToList();

            Assert.Equal(7, sections.Count);

            Assert.Equal(0, sections[0].Path.Length);
            Assert.Equal(1, sections[1].Path.Length);
            Assert.Equal(2, sections[2].Path.Length);
            Assert.Equal(2, sections[3].Path.Length);
            Assert.Equal(1, sections[4].Path.Length);
            Assert.Equal(0, sections[5].Path.Length);
            Assert.Equal(1, sections[6].Path.Length);

            Assert.Equal("Header-1", string.Join('/', sections[1].Path));
            Assert.Equal("Header-1/Header-1.1", string.Join('/', sections[2].Path));
            Assert.Equal("Header-1/Header-1.1", string.Join('/', sections[3].Path));
            Assert.Equal("Header-1", string.Join('/', sections[4].Path));
            Assert.Equal("Header-2", string.Join('/', sections[6].Path));
        }

        /// <summary>
        /// Verifies that the path property is set correctly for a simple header hierarchy.
        /// </summary>
        [Fact]
        public void ReadSections_WithSimpleHeaderHierarchy_PathPropertySet()
        {
            // lang=markdown
            var markdown = """
                # 1. TITLE

                ## 1.1 Requirement
                Some text here

                ## 1.2 Requirement
                Other text here
                """;

            var sections = MarkdownSectionParser.ReadSections(markdown, TestContext.Current.CancellationToken).ToList();
            Assert.Equal(3, sections.Count);

            Assert.Equal(0, sections[0].Path.Length);
            Assert.Equal(1, sections[1].Path.Length);
            Assert.Equal(1, sections[2].Path.Length);

            Assert.Equal("1. TITLE", sections[1].Path[0]);
            Assert.Equal("1. TITLE", sections[2].Path[0]);
        }

        /// <summary>
        /// Verifies that the heading property is set correctly for each section in a header hierarchy.
        /// </summary>
        [Fact]
        public void ReadSections_WithHeaderHierarchy_HeaderPropertySet()
        {
            // lang=markdown
            var markdown = """
                # Header-1
                Text belong to Header-1

                ## Header-1.1

                ### Header-1.1.1
                Text belong to Header-1.1.1

                ### Header-1.1.2
                Text belong to Header-1.1.2

                ## Header-1.2
                Text belong to Header-1.2

                # Header-2
                Text belong to Header-2

                ## Header-2.1
                Text belong to Header-2.1

                """;

            var sections = MarkdownSectionParser.ReadSections(markdown, TestContext.Current.CancellationToken).ToList();
            Assert.Equal(7, sections.Count);

            Assert.Equal("Header-1", sections[0].Heading);
            Assert.Equal("Header-1.1", sections[1].Heading);
            Assert.Equal("Header-1.1.1", sections[2].Heading);
            Assert.Equal("Header-1.1.2", sections[3].Heading);
            Assert.Equal("Header-1.2", sections[4].Heading);
            Assert.Equal("Header-2", sections[5].Heading);
            Assert.Equal("Header-2.1", sections[6].Heading);
        }
    }
}