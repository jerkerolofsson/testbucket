using System;
using TestBucket.AdbProxy.Protocol;
using TestBucket.Traits.Xunit;
using Xunit;

namespace TestBucket.AdbProxy.UnitTests.Protocol
{
    /// <summary>
    /// Unit tests for the <see cref="AdbCrc"/> class.
    /// </summary>
    [EnrichedTest]
    [UnitTest]
    [Component("ADB Protocol")]
    [Feature("Test Resources")]
    public class AdbCrcTests
    {
        /// <summary>
        /// Tests the <see cref="AdbCrc.Crc(byte[])"/> method with valid data.
        /// Verifies that the CRC is calculated correctly.
        /// </summary>
        [Fact]
        public void Crc_WithValidData_ShouldReturnCorrectCrc()
        {
            // Arrange
            byte[] data = { 1, 2, 3, 4, 5 };

            // Act
            uint result = AdbCrc.Crc(data);

            // Assert
            Assert.Equal(15u, result); // Expected CRC value
        }

        /// <summary>
        /// Tests the <see cref="AdbCrc.Crc(byte[], int, int)"/> method with a subset of data.
        /// Verifies that the CRC is calculated correctly for the specified range.
        /// </summary>
        [Fact]
        public void Crc_WithSubsetOfData_ShouldReturnCorrectCrc()
        {
            // Arrange
            byte[] data = { 1, 2, 3, 4, 5 };

            // Act
            uint result = AdbCrc.Crc(data, 1, 3);

            // Assert
            Assert.Equal(9u, result); // Expected CRC value for subset {2, 3, 4}
        }

        /// <summary>
        /// Tests the <see cref="AdbCrc.Crc(byte[])"/> method with an empty array.
        /// Verifies that the CRC is 0.
        /// </summary>
        [Fact]
        public void Crc_WithEmptyArray_ShouldReturnZero()
        {
            // Arrange
            byte[] data = Array.Empty<byte>();

            // Act
            uint result = AdbCrc.Crc(data);

            // Assert
            Assert.Equal(0u, result);
        }
    }
}
