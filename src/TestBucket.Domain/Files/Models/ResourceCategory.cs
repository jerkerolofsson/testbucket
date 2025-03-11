using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Files.Models
{
    /// <summary>
    /// Type of resource
    /// </summary>
    public enum ResourceCategory
    {
        Other = 0,

        Attachment = 1,

        RequirementSpecification = 2,

        TestResults = 3,
    }
}
