using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace TestBucket.Formats.Ctrf.Models
{

    public class XunitTestsExtra : TestExtra
    {
        public string? Id { get; set; }
        public string? Collection { get; set; }
        public string? Type { get; set; }
        public string? Method { get; set; }
        public CtrfTraits? Traits { get; set; }
        public XunitAttachments? Attachments { get; set; }
    }

    /// <summary>
    /// Attachments is an object where the value can be a string, or an XunitAttachment
    /// </summary>
    public class XunitAttachments : Dictionary<string, JsonNode> { }

    public class XunitAttachment
    {
        public string? Mediatype { get; set; }
        public string? Value { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class XunitEnvironmentExtra : EnvironmentExtra
    {
        /// <summary>
        /// Computer/hostname running the tests
        /// Used by reporters: xUnit v3
        /// </summary>
        public string? Computer { get; set; }

        /// <summary>
        /// User running the tests
        /// Used by reporters: xUnit v3
        /// </summary>
        public string? User { get; set; }

        /// <summary>
        /// Test suite information
        /// Used by reporters: xUnit v3
        /// </summary>
        public XunitCtrfSuite[]? Suites { get; set; }
    }

    public class XunitCtrfSuite
    {
        public string? Id { get; set; }
        public string? FilePath { get; set; }
        public string? Environment { get; set; }
        public string? TestFramework { get; set; }
        public string? TargetFramework { get; set; }
        public int Start { get; set; }
        public int Stop { get; set; }
        public int Duration { get; set; }
        public XunitCtrfCollection[]? Collections { get; set; }
    }

    public class XunitCtrfCollection
    {
        public string? Id { get; set; }

        /// <summary>
        /// Collection name, this corresponds to the testsuite name in xUnit/JUnit
        /// </summary>
        public string? Name { get; set; }
    }
}
