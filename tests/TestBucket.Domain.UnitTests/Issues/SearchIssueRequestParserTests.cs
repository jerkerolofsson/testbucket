using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Issues.Search;

namespace TestBucket.Domain.UnitTests.Issues
{
    [FunctionalTest]
    [EnrichedTest]
    [UnitTest]
    [Feature("Search")]
    public class SearchIssueRequestParserTests
    {
        [Theory]
        [InlineData("Hello World")]
        [InlineData("HelloWorld")]
        [InlineData("stateopen")]
        public void Parse_WithNoKeywords_PlainText(string text)
        {
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Equal(text, request.Text);
        }

        [Fact]
        public void Parse_WithIsIncident_TextIsNullTypeIsIssue()
        {
            string text = "is:incident";
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Null(request.Text);
            Assert.Equal("incident", request.Type);
        }
        [Fact]
        public void Parse_WithIsIssue_TextIsNullTypeIsIssue()
        {
            string text = "is:issue";
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Null(request.Text);
            Assert.Equal("issue", request.Type);
        }
        [Fact]
        public void Parse_WithOriginGithub_TextIsNullOriginIsGithub()
        {
            string text = "origin:Github";
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Null(request.Text);
            Assert.Equal("Github", request.ExternalSystemName);
        }

        [Fact]
        public void Parse_WithStateOpen_TextIsNullStateIsOpen()
        {
            string text = "state:open";
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Null(request.Text);
            Assert.Equal("open", request.State);
        }

        [Fact]
        public void Parse_WithStateOpenAndText_TextIsRemainderStateIsOpen()
        {
            string text = "state:open Hello";
            var request = SearchIssueRequestParser.Parse(1, text, []);
            Assert.Equal("Hello", request.Text);
            Assert.Equal("open", request.State);
        }


        [Fact]
        public void Parse_WithField_AddedAsField()
        {
            string text = "milestone:1.0";
            List<FieldDefinition> fieldDefinitions = [new FieldDefinition() { Name = "Milestone", Id = 123}];

            var request = SearchIssueRequestParser.Parse(1, text, fieldDefinitions);
            Assert.Single(request.Fields);
            Assert.Equal("1.0", request.Fields[0].StringValue);
            Assert.Equal(123, request.Fields[0].FilterDefinitionId);
        }
    }
}
