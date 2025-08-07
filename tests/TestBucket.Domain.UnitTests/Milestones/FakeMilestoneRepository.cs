using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Milestones;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.UnitTests.Milestones
{
    /// <summary>
    /// A simple in-memory fake implementation of IMilestoneRepository for unit testing.
    /// </summary>
    internal class FakeMilestoneRepository : IMilestoneRepository
    {
        private readonly List<Milestone> _milestones = new();
        private long _idCounter = 1;

        public Task AddMilestoneAsync(Milestone milestone)
        {
            milestone.Id = _idCounter;
            _idCounter++;

            _milestones.Add(milestone);
            return Task.CompletedTask;
        }

        public Task DeleteMilestoneByIdAsync(long id)
        {
            var ms = _milestones.FirstOrDefault(m => m.Id == id);
            if (ms != null)
                _milestones.Remove(ms);
            return Task.CompletedTask;
        }

        public Task<Milestone?> GetMilestoneByIdAsync(long id)
        {
            return Task.FromResult(_milestones.FirstOrDefault(m => m.Id == id));
        }

        public Task<IReadOnlyList<Milestone>> GetMilestonesAsync(IEnumerable<FilterSpecification<Milestone>> filters)
        {
            IEnumerable<Milestone> result = _milestones;
            foreach (var filter in filters)
            {
                result = result.Where(m => filter.IsMatch(m));
            }
            return Task.FromResult((IReadOnlyList<Milestone>)result.ToList());
        }

        public Task UpdateMilestoneAsync(Milestone milestone)
        {
            var idx = _milestones.FindIndex(m => m.Id == milestone.Id);
            if (idx >= 0)
                _milestones[idx] = milestone;
            return Task.CompletedTask;
        }
    }
}
