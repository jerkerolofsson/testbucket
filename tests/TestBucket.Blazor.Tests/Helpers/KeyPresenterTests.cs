using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using TestBucket.Blazor.Helpers;

namespace TestBucket.Blazor.Tests.Helpers
{
    [UnitTest]
    [EnrichedTest]
    [Component("Blazor Components")]
    public class KeyPresenterTests
    {
        [Fact]
        public void ToPrintable_ShouldRemoveKeyPrefix()
        {
            // Arrange
            var input = "KeyA";

            // Act
            var result = KeyPresenter.ToPrintable(input);

            // Assert
            Assert.Equal("A", result);
        }

        [Fact]
        public void ToPrintable_ShouldReturnSlashForSlashKey()
        {
            // Arrange
            var input = "Slash";

            // Act
            var result = KeyPresenter.ToPrintable(input);

            // Assert
            Assert.Equal("/", result);
        }

        [Fact]
        public void ToPrintable_ShouldReturnOriginalKeyIfNoMatch()
        {
            // Arrange
            var input = "Enter";

            // Act
            var result = KeyPresenter.ToPrintable(input);

            // Assert
            Assert.Equal("Enter", result);
        }
    }
}
