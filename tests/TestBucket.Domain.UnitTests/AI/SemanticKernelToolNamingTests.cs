using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.AI.Agent;

namespace TestBucket.Domain.UnitTests.AI
{
    /// <summary>
    /// Tests that verify that invalid characters in tool names are replaced with underscore for semantic kernel plugin
    /// </summary>
    [Feature("Chat")]
    [Component("AI")]
    [UnitTest]
    [FunctionalTest]
    [EnrichedTest]
    public class SemanticKernelToolNamingTests
    {
        /// <summary>
        /// Verifies that a dash (-) is replaced by an underscore (_)
        /// </summary>
        [Fact]
        public void GetSemanticKernelPluginName_WithDash_ReplacedWithUnderscore()
        {
            string name = "playwright-mcp";
            var pluginName = SemanticKernelToolNaming.GetSemanticKernelPluginName(name);
            Assert.Equal("playwright_mcp", pluginName);
        }
    }
}
