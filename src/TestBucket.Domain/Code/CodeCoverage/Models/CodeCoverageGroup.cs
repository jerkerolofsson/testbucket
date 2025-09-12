using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Code.CodeCoverage.Models;

/// <summary>
/// Contains information about code coverage for a specific grouping
/// </summary>
[Index(nameof(TenantId), nameof(Group), nameof(Name))]
public class CodeCoverageGroup : ProjectEntity
{
    public long Id { get; set; }

    /// <summary>
    /// Defines the what data is included in this group
    /// </summary>
    public required CodeCoverageGroupType Group { get; set; }

    /// <summary>
    /// Group name
    /// 
    /// The value depends on the Group:
    /// - CodeCoverageGroupType.Commit: SHA1
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// List of code coverage report files
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<long>? ResourceIds { get; set; }

    public long ClassCount { get; set; }
    public long CoveredClassCount { get; set; }

    public long MethodCount { get; set; }
    public long CoveredMethodCount { get; set; }

    public long CoveredLineCount { get; set; }
    public long LineCount { get; set; }
}
