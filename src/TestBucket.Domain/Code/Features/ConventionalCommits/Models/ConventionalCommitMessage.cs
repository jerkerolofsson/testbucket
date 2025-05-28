using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Code.Features.ConventionalCommits.Models;
public class ConventionalCommitMessage 
{
    public List<ConventionalCommitType> Types { get; set; } = [];

    public List<ConventionalCommitType> Footer { get; set; } = [];

    public bool IsBreakingChange { get; set; }
    public string? LongerDescription { get; set; }
    public string? Description { get; set; }

    public IEnumerable<ConventionalCommitType> GetTypes()
    {
        foreach (var type in Types) yield return type;
        foreach (var type in Footer) yield return type;
    }
}
