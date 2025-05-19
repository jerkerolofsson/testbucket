using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Insights.Extensions;

namespace TestBucket.Domain.UnitTests.Insights
{
    [Component("Insights")]
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    public class InsightsDataPointTests
    {
        [Fact]
        [TestDescription("""
            Verifies that a long will be converted to a double correctly
            """)]
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

        [Fact]
        [TestDescription("""
            Verifies that a int will be converted to a double correctly
            """)]
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

        [Fact]
        [TestDescription("""
            Verifies that a float will be converted to a double correctly
            """)]
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

        [Fact]
        [TestDescription("""
            Verifies that a decimal will be converted to a double correctly
            """)]
        public void ToDouble_FromDecimal_CorrectValueReturned()
        {
            // Arrange
            var expected = 10.0;
            var dataPoint = new InsightsDataPoint<string,decimal>("A", 10.0m);

            // Act
            var result =  dataPoint.ToDouble();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        [TestDescription("""
            Verifies that a double point will be converted to a double correctly
            """)]
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
