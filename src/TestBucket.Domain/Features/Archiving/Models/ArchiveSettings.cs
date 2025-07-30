using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Features.Archiving.Models;
public class ArchiveSettings
{
    /// <summary>
    /// If true, test runs will be archived automatically
    /// </summary>
    public bool ArchiveTestRunsAutomatically { get; set; }

    /// <summary>
    /// Age of test runs before archiving the runs
    /// This only applies to test runs that are open.
    /// </summary>
    public TimeSpan AgeBeforeArchivingTestRuns { get; set; }
}
