using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Code.CodeCoverage.Models;

namespace TestBucket.Domain.Code.CodeCoverage;

public interface ICodeCoverageRepository
{
    Task AddGroupAsync(CodeCoverageGroup group);
    Task<CodeCoverageGroup?> GetGroupAsync(string tenantId, long projectId, CodeCoverageGroupType groupType, string groupName);
    Task UpdateGroupAsync(CodeCoverageGroup group);
}
