using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Testing.TestCases.Search;

namespace TestBucket.Domain.UnitTests.Shared
{
    /// <summary>
    /// Contains unit tests for <see cref="SearchTestQueryExtensions.ToSearchText"/> to verify correct conversion of <see cref="SearchTestQuery"/> to search text.
    /// </summary>
    [UnitTest]
    [FunctionalTest]
    [Feature("Search")]
    public class SearchTestQueryExtensionsTests
    {
        /// <summary>
        /// Verifies that SearchTestQuery.ToSearchText returns "state:[State] whenthe State property is set
        /// </summary>
        [Fact]
        public void ToSearchText_WithState()
        {
            var query = new SearchTestQuery();
            query.State = "Completed";

            // Act
            var result = query.ToSearchText();

            // Assert
            Assert.Equal("state:Completed", result);
        }
        /// <summary>
        /// Verifies that SearchTestQuery.ToSearchText returns "review-assigned-to:[user] whenthe ReviewAssignedTo property is set
        /// </summary>
        [Fact]
        public void ToSearchText_WithReviewAssignedTo()
        {
            var query = new SearchTestQuery();
            query.ReviewAssignedTo = "admin@admin.com";

            // Act
            var result = query.ToSearchText();

            // Assert
            Assert.Equal("review-assigned-to:admin@admin.com", result);
        }

        /// <summary>
        /// Verifies that SearchTestQuery.ToSearchText returns the plain text when only the <c>Text</c> property is set.
        /// </summary>
        [Fact]
        public void ToSearchText_WithText()
        {
            var query = new SearchTestQuery();
            query.Text = "Hello World";

            // Act
            var result = query.ToSearchText();

            // Assert
            Assert.Equal("Hello World", result);
        }

        /// <summary>
        /// Verifies that SearchTestQuery.ToSearchText returns "is:automated" when <c>TestExecutionType</c> is set to Automated.
        /// </summary>
        [Fact]
        public void ToSearchText_WithIsAutomated()
        {
            var query = new SearchTestQuery();
            query.TestExecutionType = TestExecutionType.Automated;

            // Act
            var result = query.ToSearchText();

            // Assert
            Assert.Equal("is:automated", result);
        }

        /// <summary>
        /// Verifies that SearchTestQuery.ToSearchText returns "is:manual" when <c>TestExecutionType</c> is set to Manual.
        /// </summary>
        [Fact]
        public void ToSearchText_WithIsManual()
        {
            var query = new SearchTestQuery();
            query.TestExecutionType = TestExecutionType.Manual;

            // Act
            var result = query.ToSearchText();

            // Assert
            Assert.Equal("is:manual", result);
        }

        /// <summary>
        /// Verifies that SearchTestQuery.ToSearchText returns "is:hybrid" when <c>TestExecutionType</c> is set to Hybrid.
        /// </summary>
        [Fact]
        public void ToSearchText_WithIsHybrid()
        {
            var query = new SearchTestQuery();
            query.TestExecutionType = TestExecutionType.Hybrid;

            // Act
            var result = query.ToSearchText();

            // Assert
            Assert.Equal("is:hybrid", result);
        }

        /// <summary>
        /// Verifies that SearchTestQuery.ToSearchText returns "is:hybrid-auto" when <c>TestExecutionType</c> is set to HybridAutomated.
        /// </summary>
        [Fact]
        public void ToSearchText_WithIsHybridAutomated()
        {
            var query = new SearchTestQuery();
            query.TestExecutionType = TestExecutionType.HybridAutomated;

            // Act
            var result = query.ToSearchText();

            // Assert
            Assert.Equal("is:hybrid-auto", result);
        }

        /// <summary>
        /// Verifies that SearchTestQuery.ToSearchText returns the correct field filter when a field is present.
        /// </summary>
        [Fact]
        public void ToSearchText_WithField()
        {
            var query = new SearchTestQuery();
            query.Fields = [new FieldFilter { FilterDefinitionId = 1, Name = "color", StringValue = "red" }];

            // Act
            var result = query.ToSearchText();

            // Assert
            Assert.Equal("color:red", result);
        }

        /// <summary>
        /// Verifies that SearchTestQuery.ToSearchText quotes the field value if it contains a space.
        /// </summary>
        [Fact]
        public void ToSearchText_WithFieldValueWithSpace()
        {
            var query = new SearchTestQuery();
            query.Fields = [new FieldFilter { FilterDefinitionId = 1, Name = "color", StringValue = "dark red" }];

            // Act
            var result = query.ToSearchText();

            // Assert
            Assert.Equal("color:\"dark red\"", result);
        }

        /// <summary>
        /// Verifies that SearchTestQuery.ToSearchText quotes the field name if it contains a space.
        /// </summary>
        [Fact]
        public void ToSearchText_WithFieldNameWithSpace()
        {
            var query = new SearchTestQuery();
            query.Fields = [new FieldFilter { FilterDefinitionId = 1, Name = "vehicle propulsion", StringValue = "electric" }];

            // Act
            var result = query.ToSearchText();

            // Assert
            Assert.Equal("\"vehicle propulsion\":electric", result);
        }
    }
}