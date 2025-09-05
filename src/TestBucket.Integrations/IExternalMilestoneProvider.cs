using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Issues.Models;

namespace TestBucket.Integrations;
public interface IExternalMilestoneProvider
{
    string SystemName { get; }

    /// <summary>
    /// Returns all milestones modified/updated between the specified dates
    /// </summary>
    /// <param name="system"></param>
    /// <param name="from"></param>
    /// <param name="until"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<MilestoneDto>> GetMilestonesAsync(ExternalSystemDto system, DateTimeOffset? from, DateTimeOffset until, CancellationToken cancellationToken);

}
