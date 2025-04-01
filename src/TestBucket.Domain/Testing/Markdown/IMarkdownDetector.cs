using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Markdown
{
    public interface IMarkdownDetector
    {
        Task ProcessAsync(ClaimsPrincipal principal, TestCase testCase);
    }
}
