using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.AdbProxy.DeviceHandling;
using TestBucket.Traits.Xunit;
using Xunit;

namespace TestBucket.AdbProxy.UnitTests.DeviceHandling
{
    /// <summary>
    /// Unit tests for the GetPropParser class.
    /// </summary>
    [EnrichedTest]
    [UnitTest]
    [Component("ADB Device Handling")]
    [Feature("Test Resources")]
    public class GetPropParserTests
    {
        /// <summary>
        /// Verifies that the parser correctly parses a valid getprop output.
        /// </summary>
        [Fact]
        public void Parse_ValidGetPropOutput_ReturnsCorrectDictionary()
        {
            // Arrange
            var getPropOutput = @"[DEVICE_PROVISIONED]: [1]\n[aaudio.hw_burst_min_usec]: [2000]\n[aaudio.mmap_exclusive_policy]: [2]\n[aaudio.mmap_policy]: [2]\n[af.fast_track_multiplier]: [1]\n[arm64.memtag.process.system_server]: [off]\n[audio.deep_buffer.media]: [true]\n[audio.offload.min.duration.secs]: [30]";

            var expected = new Dictionary<string, string>
            {
                { "DEVICE_PROVISIONED", "1" },
                { "aaudio.hw_burst_min_usec", "2000" },
                { "aaudio.mmap_exclusive_policy", "2" },
                { "aaudio.mmap_policy", "2" },
                { "af.fast_track_multiplier", "1" },
                { "arm64.memtag.process.system_server", "off" },
                { "audio.deep_buffer.media", "true" },
                { "audio.offload.min.duration.secs", "30" }
            };

            // Act
            var result = GetPropParser.Parse(getPropOutput);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Verifies that the parser returns an empty dictionary for empty input.
        /// </summary>
        [Fact]
        public void Parse_EmptyInput_ReturnsEmptyDictionary()
        {
            // Arrange
            var getPropOutput = string.Empty;

            // Act
            var result = GetPropParser.Parse(getPropOutput);

            // Assert
            Assert.Empty(result);
        }

        /// <summary>
        /// Verifies that the parser skips invalid lines in the getprop output.
        /// </summary>
        [Fact]
        public void Parse_InvalidLines_SkipsInvalidLines()
        {
            // Arrange
            var getPropOutput = @"[DEVICE_PROVISIONED]: [1]\nInvalidLineWithoutBrackets\n[aaudio.hw_burst_min_usec]: [2000]";

            var expected = new Dictionary<string, string>
            {
                { "DEVICE_PROVISIONED", "1" },
                { "aaudio.hw_burst_min_usec", "2000" }
            };

            // Act
            var result = GetPropParser.Parse(getPropOutput);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
