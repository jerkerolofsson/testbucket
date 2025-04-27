using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Markdown
{
    internal class TemplateDetector : IMarkdownDetector
    {
       public Task ProcessAsync(ClaimsPrincipal principal, TestCase testCase)
        {
            if (testCase.Description is not null && testCase.IsTemplate == false)
            {
                testCase.IsTemplate = testCase.Description.Contains("@Body", StringComparison.InvariantCultureIgnoreCase);
            }
            return Task.CompletedTask;
        }
    }
}
