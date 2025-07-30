using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Insights.Extensions;

namespace TestBucket.Domain.UnitTests.Insights
{
    /// <summary>
    /// Contains unit tests for the <see cref="InsightsDataPoint{TKey, TValue}"/> class,
    /// specifically testing the <c>ToDouble</c> extension method for various numeric types.
    /// </summary>
    [Component("Insights")]
    [Feature("Insights")]
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    public class InsightsDataPointTests
    {
        /// <summary>
        /// Verifies that a long value in <see cref="InsightsDataPoint{TKey, TValue}"/> 
        /// is correctly converted to double using <c>ToDouble</c>.
        /// </summary>
        [Fact]
        public void ToDouble_FromLong_CorrectValueReturned()
        {
            // Arrange
            var expected = 10.0;
            var dataPoint = new InsightsDataPoint<string, long>("A", 10L);

            // Act
            var result = dataPoint.ToDouble();

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Verifies that an int value in <see cref="InsightsDataPoint{TKey, TValue}"/>
        /// is correctly converted to double using <c>ToDouble</c>.
        /// </summary>
        [Fact]
        public void ToDouble_FromInt_CorrectValueReturned()
        {
            // Arrange
            var expected = 10.0;
            var dataPoint = new InsightsDataPoint<string, int>("A", 10);

            // Act
            var result = dataPoint.ToDouble();

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Verifies that a float value in <see cref="InsightsDataPoint{TKey, TValue}"/>
        /// is correctly converted to double using <c>ToDouble</c>.
        /// </summary>
        [Fact]
        public void ToDouble_FromFloat_CorrectValueReturned()
        {
            // Arrange
            var expected = 10.0;
            var dataPoint = new InsightsDataPoint<string, float>("A", 10.0f);

            // Act
            var result = dataPoint.ToDouble();

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Verifies that a <see cref="decimal"/> value in <see cref="InsightsDataPoint{TKey, TValue}"/>
        /// is correctly converted to <see cref="double"/> using <c>ToDouble</c>.
        /// </summary>
        [Fact]
        public void ToDouble_FromDecimal_CorrectValueReturned()
        {
            // Arrange
            var expected = 10.0;
            var dataPoint = new InsightsDataPoint<string, decimal>("A", 10.0m);

            // Act
            var result = dataPoint.ToDouble();

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Verifies that a <see cref="double"/> value in <see cref="InsightsDataPoint{TKey, TValue}"/>
        /// is correctly returned by <c>ToDouble</c> without conversion loss.
        /// </summary>
        [Fact]
        public void ToDouble_FromDouble_CorrectValueReturned()
        {
            // Arrange
            var expected = 10.0;
            var dataPoint = new InsightsDataPoint<string, double>("A", expected);

            // Act
            var result = dataPoint.ToDouble();

            // Assert
            Assert.Equal(expected, result);
        }
    }
}