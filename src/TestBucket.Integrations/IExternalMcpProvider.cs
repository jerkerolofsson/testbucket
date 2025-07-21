using TestBucket.Contracts.Integrations;

namespace TestBucket.Integrations;
public interface IExternalMcpProvider
{
    string SystemName { get; }
    Task<string?> GetMcpJsonConfigurationAsync(ExternalSystemDto system);
}
