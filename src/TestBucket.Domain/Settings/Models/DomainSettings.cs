using TestBucket.Domain.Teams.Models;

namespace TestBucket.Domain.Settings.Models;

[Index(nameof(Type), nameof(TestProjectId), nameof(TenantId))]
public class DomainSettings
{
    public required string TenantId { get; set; }
    public long? TestProjectId { get; set; }

    public long Id { get; set; }
    public required string Type { get; set; }
    public required string Json { get; set; }
}
