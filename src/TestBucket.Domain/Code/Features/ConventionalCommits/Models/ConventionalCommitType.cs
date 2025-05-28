using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Code.Features.ConventionalCommits.Models;
public class ConventionalCommitType
{
    /// <summary>
    /// fix, feat, test, chore etc..
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Argument to type, e.g. fix(scope)
    /// </summary>
    public string? Scope { get; set; }

    /// <summary>
    /// Description text that follows the :
    /// </summary>
    public string? Description { get; set; }
}
