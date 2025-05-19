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
    [UnitTest]
    [FunctionalTest]
    [Feature("Search")]
    public class SearchTestQueryExtensionsTests
    {
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

        [Fact]
        public void ToSearchText_WithField()
        {
            var query = new SearchTestQuery();
            query.Fields = [new FieldFilter { FilterDefinitionId=1, Name = "color", StringValue = "red" }];

            // Act
            var result = query.ToSearchText();

            // Assert
            Assert.Equal("color:red", result);
        }

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
