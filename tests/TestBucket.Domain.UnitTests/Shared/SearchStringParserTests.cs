using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts;
using TestBucket.Domain.Shared;

namespace TestBucket.Domain.UnitTests.Shared
{
    /// <summary>
    /// Contains unit tests for the <see cref="SearchStringParser"/> class.
    /// Verifies parsing of search strings with keywords, fields, and quoted values.
    /// </summary>
    [UnitTest]
    [FunctionalTest]
    [Feature("Search")]
    public class SearchStringParserTests
    {
        /// <summary>
        /// Verifies that when no keywords are present, the raw text is returned as-is.
        /// </summary>
        [Fact]
        public void ParseString_WithoutKeywords_RawTextReturned()
        {
            Dictionary<string, string> result = new();
            List<FieldFilter> fields = [];
            List<FieldDefinition> fieldDefinitions = [];
            HashSet<string> keywords = ["is", "after", "before"];
            var text = SearchStringParser.Parse("hello world", result, fields, keywords, fieldDefinitions);

            Assert.Equal("hello world", text);
        }

        /// <summary>
        /// Verifies that when a keyword is present, it is removed from the raw text.
        /// </summary>
        [Fact]
        public void ParseString_WithKeyword_RawTextDoesNotContainKeyword()
        {
            Dictionary<string, string> result = new();
            List<FieldFilter> fields = [];
            List<FieldDefinition> fieldDefinitions = [];
            HashSet<string> keywords = ["is", "after", "before"];
            var text = SearchStringParser.Parse("hello is:issue", result, fields, keywords, fieldDefinitions);

            Assert.Equal("hello", text);
        }

        /// <summary>
        /// Verifies that a keyword and its value are added to the result dictionary.
        /// </summary>
        [Fact]
        public void ParseString_WithKeyword_KeywordOutputToResultDictionary()
        {
            Dictionary<string, string> result = new();
            List<FieldFilter> fields = [];
            List<FieldDefinition> fieldDefinitions = [];
            HashSet<string> keywords = ["is", "after", "before"];
            var text = SearchStringParser.Parse("hello is:issue", result, fields, keywords, fieldDefinitions);

            Assert.Single(result);
            Assert.True(result.ContainsKey("is"));
            Assert.Equal("issue", result["is"]);
        }

        /// <summary>
        /// Verifies that a keyword value in quotes is correctly parsed and added to the result dictionary.
        /// </summary>
        [Fact]
        public void ParseString_WithKeywordValueInQuotes_KeywordOutputToResultDictionary()
        {
            Dictionary<string, string> result = new();
            List<FieldFilter> fields = [];
            List<FieldDefinition> fieldDefinitions = [];
            HashSet<string> keywords = ["is", "after", "before"];
            var text = SearchStringParser.Parse("hello is:\"no problems\"", result, fields, keywords, fieldDefinitions);

            Assert.Single(result);
            Assert.True(result.ContainsKey("is"));
            Assert.Equal("no problems", result["is"]);
        }

        /// <summary>
        /// Verifies that a field name and value in quotes are correctly parsed and added to the filters list.
        /// </summary>
        [Fact]
        public void ParseString_WithFieldNameAndValueInQuotes_FieldInResult()
        {
            Dictionary<string, string> result = new();
            List<FieldFilter> filters = [];
            List<FieldDefinition> fieldDefinitions = [new FieldDefinition { Name = "Dark Mode", Id = 123 }];
            HashSet<string> keywords = ["is", "after", "before"];
            var text = SearchStringParser.Parse("hello \"Dark Mode\":\"Working Good!\"", result, filters, keywords, fieldDefinitions);

            Assert.Single(filters);
            Assert.Equal(123, filters[0].FilterDefinitionId);
            Assert.Equal("Working Good!", filters[0].StringValue);
        }

        /// <summary>
        /// Verifies that a field name in quotes is correctly parsed and added to the filters list.
        /// </summary>
        [Fact]
        public void ParseString_WithFieldNameInQuotes_FieldInResult()
        {
            Dictionary<string, string> result = new();
            List<FieldFilter> filters = [];
            List<FieldDefinition> fieldDefinitions = [new FieldDefinition { Name = "Dark Mode", Id = 123 }];
            HashSet<string> keywords = ["is", "after", "before"];
            var text = SearchStringParser.Parse("hello \"Dark Mode\":abc", result, filters, keywords, fieldDefinitions);

            Assert.Single(filters);
            Assert.Equal(123, filters[0].FilterDefinitionId);
            Assert.Equal("abc", filters[0].StringValue);
        }

        /// <summary>
        /// Verifies that a field value in quotes is correctly parsed and added to the filters list.
        /// </summary>
        [Fact]
        public void ParseString_WithFieldValueInQuotes_FieldInResult()
        {
            Dictionary<string, string> result = new();
            List<FieldFilter> filters = [];
            List<FieldDefinition> fieldDefinitions = [new FieldDefinition { Name = "component", Id = 123 }];
            HashSet<string> keywords = ["is", "after", "before"];
            var text = SearchStringParser.Parse("hello component:\"red color\"", result, filters, keywords, fieldDefinitions);

            Assert.Single(filters);
            Assert.Equal(123, filters[0].FilterDefinitionId);
            Assert.Equal("red color", filters[0].StringValue);
        }

        /// <summary>
        /// Verifies that a field and its value are correctly parsed and added to the filters list.
        /// </summary>
        [Fact]
        public void ParseString_WithField_FieldInResult()
        {
            Dictionary<string, string> result = new();
            List<FieldFilter> filters = [];
            List<FieldDefinition> fieldDefinitions = [new FieldDefinition { Name = "component", Id = 123 }];
            HashSet<string> keywords = ["is", "after", "before"];
            var text = SearchStringParser.Parse("hello component:keyboard", result, filters, keywords, fieldDefinitions);

            Assert.Single(filters);
            Assert.Equal(123, filters[0].FilterDefinitionId);
            Assert.Equal("keyboard", filters[0].StringValue);
        }

        /// <summary>
        /// Verifies that parsing an empty string returns null.
        /// </summary>
        [Fact]
        public void ParseString_EmptyInput_ReturnsNull()
        {
            Dictionary<string, string> result = new();
            List<FieldFilter> fields = [];
            List<FieldDefinition> fieldDefinitions = [];
            HashSet<string> keywords = ["is", "after", "before"];
            var text = SearchStringParser.Parse("", result, fields, keywords, fieldDefinitions);

            Assert.Null(text);
            Assert.Empty(result);
            Assert.Empty(fields);
        }

        /// <summary>
        /// Verifies that a string with only a keyword and value returns null for text.
        /// </summary>
        [Fact]
        public void ParseString_OnlyKeyword_ReturnsNullText()
        {
            Dictionary<string, string> result = new();
            List<FieldFilter> fields = [];
            List<FieldDefinition> fieldDefinitions = [];
            HashSet<string> keywords = ["is", "after", "before"];
            var text = SearchStringParser.Parse("is:open", result, fields, keywords, fieldDefinitions);

            Assert.Null(text);
            Assert.Single(result);
            Assert.Equal("open", result["is"]);
        }

        /// <summary>
        /// Verifies that unknown fields are left in the text.
        /// </summary>
        [Fact]
        public void ParseString_UnknownField_StaysInText()
        {
            Dictionary<string, string> result = new();
            List<FieldFilter> fields = [];
            List<FieldDefinition> fieldDefinitions = [];
            HashSet<string> keywords = ["is", "after", "before"];
            var text = SearchStringParser.Parse("hello unknownfield:value", result, fields, keywords, fieldDefinitions);

            Assert.Equal("hello unknownfield:value", text);
            Assert.Empty(result);
            Assert.Empty(fields);
        }

        /// <summary>
        /// Verifies that multiple keywords and fields are parsed correctly.
        /// </summary>
        [Fact]
        public void ParseString_MultipleKeywordsAndFields_AllParsed()
        {
            Dictionary<string, string> result = new();
            List<FieldFilter> filters = [];
            List<FieldDefinition> fieldDefinitions = [
                new FieldDefinition { Name = "component", Id = 1 },
                new FieldDefinition { Name = "priority", Id = 2 }
            ];
            HashSet<string> keywords = ["is", "after", "before"];
            var text = SearchStringParser.Parse("is:open component:ui priority:\"high\"", result, filters, keywords, fieldDefinitions);

            Assert.Null(text);
            Assert.Single(result);
            Assert.Equal("open", result["is"]);
            Assert.Equal(2, filters.Count);
            Assert.Equal(1, filters[0].FilterDefinitionId);
            Assert.Equal("ui", filters[0].StringValue);
            Assert.Equal(2, filters[1].FilterDefinitionId);
            Assert.Equal("high", filters[1].StringValue);
        }

        /// <summary>
        /// Verifies that quoted and unquoted values are handled together.
        /// </summary>
        [Fact]
        public void ParseString_MixedQuotedAndUnquoted_ParsesCorrectly()
        {
            Dictionary<string, string> result = new();
            List<FieldFilter> filters = [];
            List<FieldDefinition> fieldDefinitions = [
                new FieldDefinition { Name = "component", Id = 1 }
            ];
            HashSet<string> keywords = ["is"];
            var text = SearchStringParser.Parse("is:\"in progress\" component:keyboard", result, filters, keywords, fieldDefinitions);

            Assert.Null(text);
            Assert.Single(result);
            Assert.Equal("in progress", result["is"]);
            Assert.Single(filters);
            Assert.Equal(1, filters[0].FilterDefinitionId);
            Assert.Equal("keyboard", filters[0].StringValue);
        }
    }
}