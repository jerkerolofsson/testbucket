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
    /// <summary>
    /// Marks a test case as hybrid
    /// </summary>
    public class HybridDetector : IMarkdownDetector
    {
        private readonly IEnumerable<IMarkdownTestRunner> _hybridRunners;

        public HybridDetector(IEnumerable<IMarkdownTestRunner> hybridRunners) 
        {
            _hybridRunners = hybridRunners;
        }

        private async Task<HashSet<string>> GetRunnerLanguagesAsync(ClaimsPrincipal principal)
        {
            var langs = new HashSet<string>();
            foreach (var runner in _hybridRunners)
            {
                foreach(var lang in await runner.GetSupportedLanguagesAsync(principal))
                {
                    if(!langs.Contains(lang))
                    {
                        langs.Add(lang);
                    }
                }
            }
            return langs;
        }

        /// <summary>
        /// Returns either TestExecutionType.Manual or TestExecutionType.Hybrid based on the markdown description of a test
        /// </summary>
        /// <returns></returns>
        private TestExecutionType DetectHybridTestExecutionType(string markdown, IEnumerable<string> runnerLanguages)
        {
            foreach (var runnerLanguage in runnerLanguages)
            {
                var codeBlockMarker = $"```{runnerLanguage}";
                if(markdown.Contains(codeBlockMarker))
                {
                    return TestExecutionType.Hybrid;
                }
            }

            return TestExecutionType.Manual;
        }

        public async Task ProcessAsync(ClaimsPrincipal principal, TestCase testCase)
        {
            if (testCase.Description is not null)
            {
                if (testCase.ExecutionType == TestExecutionType.Hybrid || testCase.ExecutionType == TestExecutionType.Manual)
                {
                    var runnerLanguages = await GetRunnerLanguagesAsync( principal);

                    testCase.ExecutionType = DetectHybridTestExecutionType(testCase.Description, runnerLanguages);
                }
            }
        }
    }
}
