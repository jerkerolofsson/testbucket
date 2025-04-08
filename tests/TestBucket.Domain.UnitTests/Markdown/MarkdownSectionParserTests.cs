using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Markdown;
using TestBucket.Domain.Requirements.Import;
using TestBucket.Domain.UnitTests.TestHelpers;
using TUnit.Assertions.Extensions;

namespace TestBucket.Domain.UnitTests.Markdown
{
    [UnitTest]
    public class MarkdownSectionParserTests
    {
        [Test]
        public async Task ReadSections_WithSingleLine_OneSection()
        {
            var sections = MarkdownSectionParser.ReadSections("Hello World", default).ToList();
            await Assert.That(sections.Count).IsEqualTo(1);
        }


        [Test]
        public async Task ReadSections_WithMultipleLines_OneSection()
        {
            var sections = MarkdownSectionParser.ReadSections("Hello World\nThis is still the same section", default).ToList();
            await Assert.That(sections.Count).IsEqualTo(1);
        }

        [Test]
        public async Task ReadSections_WithTwoHeaders_TwoSectionsReturned()
        {
            var sections = MarkdownSectionParser.ReadSections("# Section 1\n\n#Section 2", default).ToList();
            await Assert.That(sections.Count).IsEqualTo(2);
        }


        [Test]
        public async Task ReadSections_WithHeaderHierarchy_PathSetFromHierarchy()
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

            var sections = MarkdownSectionParser.ReadSections(markdown, default).ToList();
            await Assert.That(sections.Count).IsEqualTo(7);

            await Assert.That(sections[0].Path.Length).IsEqualTo(0);
            await Assert.That(sections[1].Path.Length).IsEqualTo(1);
            await Assert.That(sections[2].Path.Length).IsEqualTo(2);
            await Assert.That(sections[3].Path.Length).IsEqualTo(2);
            await Assert.That(sections[4].Path.Length).IsEqualTo(1);
            await Assert.That(sections[5].Path.Length).IsEqualTo(0);
            await Assert.That(sections[6].Path.Length).IsEqualTo(1);

            await Assert.That(sections[1].Path[0]).IsEqualTo("Header-1");
            await Assert.That(sections[2].Path[0]).IsEqualTo("Header-1");
            await Assert.That(sections[2].Path[1]).IsEqualTo("Header-1.1");
            await Assert.That(sections[3].Path[0]).IsEqualTo("Header-1");
            await Assert.That(sections[3].Path[1]).IsEqualTo("Header-1.1");

            await Assert.That(sections[4].Path[0]).IsEqualTo("Header-1");
            await Assert.That(sections[6].Path[0]).IsEqualTo("Header-2");
        }


        [Test]
        public async Task ReadSections_WithSimpleHeaderHierarchy_PathPropertySet()
        {
            // lang=markdown
            var markdown = """
                # 1. TITLE

                ## 1.1 Requirement
                Some text here

                ## 1.2 Requirement
                Other text here
                """;

            var sections = MarkdownSectionParser.ReadSections(markdown, default).ToList();
            await Assert.That(sections.Count).IsEqualTo(3);

            await Assert.That(sections[0].Path.Length).IsEqualTo(0);
            await Assert.That(sections[1].Path.Length).IsEqualTo(1);
            await Assert.That(sections[2].Path.Length).IsEqualTo(1);
        }

        [Test]
        public async Task ReadSections_WithHeaderHierarchy_HeaderPropertySet()
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

            var sections = MarkdownSectionParser.ReadSections(markdown, default).ToList();
            await Assert.That(sections.Count).IsEqualTo(7);

            await Assert.That(sections[0].Heading).IsEqualTo("Header-1");
            await Assert.That(sections[1].Heading).IsEqualTo("Header-1.1");
            await Assert.That(sections[2].Heading).IsEqualTo("Header-1.1.1");
            await Assert.That(sections[3].Heading).IsEqualTo("Header-1.1.2");
            await Assert.That(sections[4].Heading).IsEqualTo("Header-1.2");
            await Assert.That(sections[5].Heading).IsEqualTo("Header-2");
            await Assert.That(sections[6].Heading).IsEqualTo("Header-2.1");
        }
    }
}
