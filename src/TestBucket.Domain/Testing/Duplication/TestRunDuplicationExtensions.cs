using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Duplication;
internal static class TestRunDuplicationExtensions
{
    public static TestRun Duplicate(this TestRun run)
    {
        TestRun copy = new TestRun()
        {
            Name = run.Name + " copy",
            CiCdRef = run.CiCdRef,
            CiCdSystem = run.CiCdSystem,
            ExternalId = run.ExternalId,
            ExternalSystemId = run.ExternalSystemId,
            Description = run.Description,
            TenantId = run.TenantId,
            TestProjectId = run.TestProjectId,
            TeamId = run.TeamId,
            Open = run.Open,
        };
        return copy;
    }
}