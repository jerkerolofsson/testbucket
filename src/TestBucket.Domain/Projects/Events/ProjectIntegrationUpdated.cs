using Mediator;

namespace TestBucket.Domain.Projects.Events;
public record class ProjectIntegrationUpdated(ClaimsPrincipal Principal, ExternalSystem ExternalSystem) : INotification;
