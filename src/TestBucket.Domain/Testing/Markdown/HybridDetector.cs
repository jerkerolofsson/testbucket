using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Markdown
{
    public class HybridDetector : IMarkdownDetector
    {
        private readonly HashSet<string> _runnerLanguages = new HashSet<string>();
        public HybridDetector(IEnumerable<IMarkdownTestRunner> hybridRunners) 
        {
            foreach (var runnerLanguage in hybridRunners.Select(x => x.Language).Distinct())
            {
                _runnerLanguages.Add(runnerLanguage);
            }
        }

        /// <summary>
        /// Returns either TestExecutionType.Manual or TestExecutionType.Hybrid based on the markdown description of a test
        /// </summary>
        /// <returns></returns>
        public TestExecutionType DetectHybridTestExecutionType(string markdown)
        {
            foreach (var runnerLanguage in _runnerLanguages)
            {
                var codeBlockMarker = $"```{runnerLanguage}";
                if(markdown.Contains(codeBlockMarker))
                {
                    return TestExecutionType.Hybrid;
                }
            }

            return TestExecutionType.Manual;
        }

        public Task ProcessAsync(ClaimsPrincipal principal, TestCase testCase)
        {
            if (testCase.Description is not null)
            {
                if (testCase.ExecutionType == TestExecutionType.Hybrid || testCase.ExecutionType == TestExecutionType.Manual)
                {
                    testCase.ExecutionType = DetectHybridTestExecutionType(testCase.Description);
                }
            }
            return Task.CompletedTask;
        }
    }
}
