
using Mediator;

using TestBucket.Domain.TestResources.Models;

namespace TestBucket.Domain.TestResources.Events;
public record class TestResourceUpdated(ClaimsPrincipal Principal, TestResource Resource) : INotification;
