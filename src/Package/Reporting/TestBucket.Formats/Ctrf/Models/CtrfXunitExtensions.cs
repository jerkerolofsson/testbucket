using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using TestBucket.Formats.Ctrf.Xunit;

namespace TestBucket.Formats.Ctrf.Models
{

    public class XunitTestsExtra : TestExtra
    {
        [JsonPropertyName("id")] public string? Id { get; set; }

        /// <summary>
        /// corresponds to id in the Xunitcollections
        /// </summary>
        [JsonPropertyName("collection")] public string? Collection { get; set; }
        [JsonPropertyName("type")] public string? Type { get; set; }
        [JsonPropertyName("method")] public string? Method { get; set; }
        [JsonPropertyName("traits")] public CtrfTraits? Traits { get; set; }
        
        [JsonConverter(typeof(AttachmentsConverter))]
        [JsonPropertyName("attachments")] 
        public XunitAttachments? Attachments { get; set; }
    }

    /// <summary>
    /// Attachments is an object where the value can be a string, or an XunitAttachment
    /// </summary>
    public class XunitAttachments 
    {
        public List<TestTrait> Traits { get; set; } = [];
        public List<AttachmentDto> Attachments { get; set; } = [];
    }

    public class XunitAttachment
    {
        [JsonPropertyName("mediaType")] public string? Mediatype { get; set; }
        [JsonPropertyName("value")] public string? Value { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class XunitEnvironmentExtra : EnvironmentExtra
    {
    }

    public class XunitResultsExtra : ResultsExtra
    {
        /// <summary>
        /// Computer/hostname running the tests
        /// Used by reporters: xUnit v3
        /// </summary>
        [JsonPropertyName("computer")] public string? Computer { get; set; }

        /// <summary>
        /// User running the tests
        /// Used by reporters: xUnit v3
        /// </summary>
        [JsonPropertyName("user")] public string? User { get; set; }

        /// <summary>
        /// Test suite information
        /// Used by reporters: xUnit v3
        /// </summary>
        [JsonPropertyName("suites")] public XunitCtrfSuite[]? Suites { get; set; }
    }

    public class XunitCtrfSuite
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("filePath")] public string? FilePath { get; set; }

        /// <summary>
        /// 64-bit (x64) .NET 9.0.2 [collection-per-class, parallel (16 threads)]
        /// </summary>
        [JsonPropertyName("environment")] public string? Environment { get; set; }
        [JsonPropertyName("testFramework")] public string? TestFramework { get; set; }

        /// <summary>
        /// .NETCoreApp,Version=v9.0
        /// </summary>
        [JsonPropertyName("targetFramework")] public string? TargetFramework { get; set; }
        [JsonPropertyName("start")] public long Start { get; set; }
        [JsonPropertyName("stop")] public long Stop { get; set; }

        /// <summary>
        /// duration, in milliseconds
        /// </summary>
        [JsonPropertyName("duration")] public int Duration { get; set; }
        [JsonPropertyName("collections")] public XunitCtrfCollection[]? Collections { get; set; }
    }

    public class XunitCtrfCollection
    {
        [JsonPropertyName("id")] public string? Id { get; set; }

        /// <summary>
        /// Collection name, this corresponds to the testsuite name in xUnit/JUnit
        /// </summary>
        [JsonPropertyName("name")] public string? Name { get; set; }
    }
}
