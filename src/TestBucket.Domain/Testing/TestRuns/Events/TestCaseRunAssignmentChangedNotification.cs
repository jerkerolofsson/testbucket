using Mediator;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestRuns.Events;
public record class TestCaseRunAssignmentChangedNotification(ClaimsPrincipal Principal, TestCaseRun TestCaseRun, string? OldAssignedUserName) : INotification;
