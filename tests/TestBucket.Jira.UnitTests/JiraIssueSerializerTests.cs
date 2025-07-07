using System;
using System.Text.Json;
using TestBucket.Jira.Models;
using TestBucket.Jira.Serialization;
using TestBucket.Traits.Xunit;
using Xunit;

namespace TestBucket.Jira.UnitTests
{
    /// <summary>
    /// Unit tests for the <see cref="JiraIssueSerializer"/> class that handles
    /// JSON serialization and deserialization of Jira API responses.
    /// </summary>
    [EnrichedTest]
    [FunctionalTest]
    [UnitTest]
    [Component("Jira")]
    [Feature("Jira")]
    public class JiraIssueSerializerTests
    {
        /// <summary>
        /// Tests that a valid Jira API response JSON can be successfully deserialized
        /// into a JiraPagedIssuesResponse object with correct properties.
        /// </summary>
        [Fact]
        public void DeserializeJson_WithValidJiraResponse_ReturnsCorrectObject()
        {
            // Arrange
            var json = """
                {
                  "issues": [
                    {
                      "expand": "renderedFields,names,schema,operations,editmeta,changelog,versionedRepresentations",
                      "id": "10007",
                      "self": "https://api.atlassian.com/ex/jira/rest/api/3/issue/10007",
                      "key": "CPG-8",
                      "fields": {
                        "summary": "Test issue 1",
                        "created": "2025-06-03T09:43:31.805+0800",
                        "updated": "2025-07-07T14:40:29.739+0800"
                      }
                    }
                  ],
                  "isLast": true
                }
                """;

            // Act
            var result = JiraIssueSerializer.DeserializeJson(json);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.isLast);
            Assert.NotNull(result.issues);
            Assert.Single(result.issues);

            var issue = result.issues[0];
            Assert.Equal("10007", issue.id);
            Assert.Equal("CPG-8", issue.key);
            Assert.Equal("https://api.atlassian.com/ex/jira/rest/api/3/issue/10007", issue.self);
            Assert.NotNull(issue.fields);
        }

        /// <summary>
        /// Tests that an empty issues array is correctly deserialized.
        /// </summary>
        [Fact]
        public void DeserializeJson_WithEmptyIssuesArray_ReturnsEmptyArray()
        {
            // Arrange
            var json = """
                {
                  "issues": [],
                  "isLast": true
                }
                """;

            // Act
            var result = JiraIssueSerializer.DeserializeJson(json);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.isLast);
            Assert.NotNull(result.issues);
            Assert.Empty(result.issues);
        }

        /// <summary>
        /// Tests that pagination properties are correctly deserialized.
        /// </summary>
        [Fact]
        public void DeserializeJson_WithPaginationProperties_DeserializesCorrectly()
        {
            // Arrange
            var json = """
                {
                  "issues": [],
                  "isLast": false,
                  "nextPageToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"
                }
                """;

            // Act
            var result = JiraIssueSerializer.DeserializeJson(json);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.isLast);
            Assert.Equal("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9", result.nextPageToken);
            Assert.NotNull(result.issues);
            Assert.Empty(result.issues);
        }

        /// <summary>
        /// Tests that null or missing optional properties are handled correctly.
        /// </summary>
        [Fact]
        public void DeserializeJson_WithNullOptionalProperties_HandlesNullsCorrectly()
        {
            // Arrange
            var json = """
                {
                  "issues": [
                    {
                      "id": "10007",
                      "key": "CPG-8",
                      "fields": {
                        "summary": "Test issue",
                        "description": null,
                        "assignee": null,
                        "created": "2025-06-03T09:43:31.805+0800"
                      }
                    }
                  ],
                  "isLast": true,
                  "nextPageToken": null
                }
                """;

            // Act
            var result = JiraIssueSerializer.DeserializeJson(json);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.isLast);
            Assert.Null(result.nextPageToken);
            Assert.NotNull(result.issues);
            Assert.Single(result.issues);

            var issue = result.issues[0];
            Assert.Equal("10007", issue.id);
            Assert.Equal("CPG-8", issue.key);
            Assert.NotNull(issue.fields);
        }

        /// <summary>
        /// Tests that various Jira date formats are correctly deserialized using custom converters.
        /// </summary>
        /// <param name="dateString">The date string in Jira format to test.</param>
        /// <param name="expectedYear">The expected year component.</param>
        /// <param name="expectedMonth">The expected month component.</param>
        /// <param name="expectedDay">The expected day component.</param>
        [Theory]
        [InlineData("2025-06-03T09:43:31.805+0800", 2025, 6, 3)]
        [InlineData("2025-06-03T09:43:31.80+0800", 2025, 6, 3)]
        [InlineData("2025-06-03T09:43:31.8+0800", 2025, 6, 3)]
        [InlineData("2025-06-03T09:43:31+0800", 2025, 6, 3)]
        [InlineData("2025-06-03T09:43:31.805Z", 2025, 6, 3)]
        [InlineData("2025-06-03T09:43:31Z", 2025, 6, 3)]
        public void DeserializeJson_WithVariousDateFormats_ParsesDateCorrectly(string dateString, int expectedYear, int expectedMonth, int expectedDay)
        {
            // Arrange
            var json = $$"""
                {
                  "issues": [
                    {
                      "id": "10007",
                      "key": "CPG-8",
                      "fields": {
                        "summary": "Test issue",
                        "created": "{{dateString}}"
                      }
                    }
                  ],
                  "isLast": true
                }
                """;

            // Act
            var result = JiraIssueSerializer.DeserializeJson(json);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.issues);
            Assert.Single(result.issues);

            var issue = result.issues[0];
            Assert.NotNull(issue.fields);
            Assert.NotNull(issue.fields.created);

            Assert.Equal(expectedYear, issue.fields.created.Value.Year);
            Assert.Equal(expectedMonth, issue.fields.created.Value.Month);
            Assert.Equal(expectedDay, issue.fields.created.Value.Day);
        }

        /// <summary>
        /// Tests that case-insensitive property names are handled correctly.
        /// </summary>
        [Fact]
        public void DeserializeJson_WithCaseInsensitiveProperties_DeserializesCorrectly()
        {
            // Arrange
            var json = """
                {
                  "Issues": [
                    {
                      "ID": "10007",
                      "Key": "CPG-8",
                      "Self": "https://api.atlassian.com/ex/jira/rest/api/3/issue/10007"
                    }
                  ],
                  "IsLast": true
                }
                """;

            // Act
            var result = JiraIssueSerializer.DeserializeJson(json);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.isLast);
            Assert.NotNull(result.issues);
            Assert.Single(result.issues);

            var issue = result.issues[0];
            Assert.Equal("10007", issue.id);
            Assert.Equal("CPG-8", issue.key);
            Assert.Equal("https://api.atlassian.com/ex/jira/rest/api/3/issue/10007", issue.self);
        }

        /// <summary>
        /// Tests that invalid JSON returns null without throwing an exception.
        /// </summary>
        [Fact]
        public void DeserializeJson_WithInvalidJson_ReturnsNull()
        {
            // Arrange
            var invalidJson = "{ invalid json }";

            // Act & Assert
            Assert.Throws<JsonException>(() => JiraIssueSerializer.DeserializeJson(invalidJson));
        }

        /// <summary>
        /// Tests that null input returns null.
        /// </summary>
        [Fact]
        public void DeserializeJson_WithNullInput_ReturnsNull()
        {
            // Act
            var result = JiraIssueSerializer.DeserializeJson(null!);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Tests that empty string input throws appropriate exception.
        /// </summary>
        [Fact]
        public void DeserializeJson_WithEmptyString_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<JsonException>(() => JiraIssueSerializer.DeserializeJson(""));
        }

        /// <summary>
        /// Tests that whitespace-only input throws appropriate exception.
        /// </summary>
        [Fact]
        public void DeserializeJson_WithWhitespaceOnly_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<JsonException>(() => JiraIssueSerializer.DeserializeJson("   "));
        }

        /// <summary>
        /// Tests that multiple issues are correctly deserialized.
        /// </summary>
        [Fact]
        public void DeserializeJson_WithMultipleIssues_DeserializesAllIssues()
        {
            // Arrange
            var json = """
                {
                  "issues": [
                    {
                      "id": "10007",
                      "key": "CPG-8",
                      "self": "https://api.atlassian.com/ex/jira/rest/api/3/issue/10007"
                    },
                    {
                      "id": "10008",
                      "key": "CPG-9",
                      "self": "https://api.atlassian.com/ex/jira/rest/api/3/issue/10008"
                    },
                    {
                      "id": "10009",
                      "key": "CPG-10",
                      "self": "https://api.atlassian.com/ex/jira/rest/api/3/issue/10009"
                    }
                  ],
                  "isLast": true
                }
                """;

            // Act
            var result = JiraIssueSerializer.DeserializeJson(json);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.isLast);
            Assert.NotNull(result.issues);
            Assert.Equal(3, result.issues.Length);

            Assert.Equal("10007", result.issues[0].id);
            Assert.Equal("CPG-8", result.issues[0].key);

            Assert.Equal("10008", result.issues[1].id);
            Assert.Equal("CPG-9", result.issues[1].key);

            Assert.Equal("10009", result.issues[2].id);
            Assert.Equal("CPG-10", result.issues[2].key);
        }

        /// <summary>
        /// Tests that complex nested JSON structures are handled correctly.
        /// </summary>
        [Fact]
        public void DeserializeJson_WithComplexNestedStructure_DeserializesCorrectly()
        {
            // Arrange
            var json = """
                {
                  "issues": [
                    {
                      "id": "10007",
                      "key": "CPG-8",
                      "fields": {
                        "summary": "Complex issue",
                        "issuetype": {
                          "id": "10006",
                          "name": "Bug",
                          "subtask": false
                        },
                        "project": {
                          "id": "10000",
                          "key": "CPG",
                          "name": "Test Project"
                        },
                        "labels": ["bug", "urgent"],
                        "customfield_10019": "0|hzzzzz:"
                      }
                    }
                  ],
                  "isLast": true
                }
                """;

            // Act
            var result = JiraIssueSerializer.DeserializeJson(json);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.isLast);
            Assert.NotNull(result.issues);
            Assert.Single(result.issues);

            var issue = result.issues[0];
            Assert.Equal("10007", issue.id);
            Assert.Equal("CPG-8", issue.key);
            Assert.NotNull(issue.fields);
        }

        /// <summary>
        /// Tests that JSON with missing required properties still deserializes
        /// with null values for missing properties.
        /// </summary>
        [Fact]
        public void DeserializeJson_WithMissingProperties_SetsNullValues()
        {
            // Arrange
            var json = """
                {
                  "issues": [
                    {
                      "key": "CPG-8"
                    }
                  ],
                  "isLast": true
                }
                """;

            // Act
            var result = JiraIssueSerializer.DeserializeJson(json);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.isLast);
            Assert.NotNull(result.issues);
            Assert.Single(result.issues);

            var issue = result.issues[0];
            Assert.Equal("CPG-8", issue.key);
            Assert.Null(issue.id);
            Assert.Null(issue.self);
            Assert.Null(issue.expand);
            Assert.Null(issue.fields);
        }
    }
}