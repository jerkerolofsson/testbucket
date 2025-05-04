using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Milestones;
public interface IMilestoneRepository
{
    Task<Milestone?> GetMilestoneByIdAsync(long id);
    Task DeleteMilestoneByIdAsync(long id);
    Task AddMilestoneAsync(Milestone milestone);
    Task UpdateMilestoneAsync(Milestone milestone);
    Task<IReadOnlyList<Milestone>> GetMilestonesAsync(IEnumerable<FilterSpecification<Milestone>> filters);
}
