using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Identity.Permissions
{
    public enum PermissionEntityType
    {
        Tenant,

        User,

        Project,

        RequirementSpecification,

        Requirement,

        TestSuite,

        TestCase,

        TestRun,

        TestCaseRun
    }
}
