using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Markdown
{
    /// <summary>
    /// Detects whether a <see cref="TestCase"/> is a template based on its description content.
    /// </summary>
    public interface IMarkdownDetector
    {
        /// <summary>
        /// Processes the specified <see cref="TestCase"/> and sets its <c>IsTemplate</c> property to <c>true</c>
        /// if the description contains the <c>@Body</c> marker (case-insensitive).
        /// </summary>
        /// <param name="principal">The user principal performing the operation.</param>
        /// <param name="testCase">The test case to process and update.</param>
        Task ProcessAsync(ClaimsPrincipal principal, TestCase testCase);
    }
}
