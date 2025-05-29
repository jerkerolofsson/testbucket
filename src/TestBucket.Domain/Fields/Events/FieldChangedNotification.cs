using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields.Events;
public record class TestRunFieldChangedNotification(ClaimsPrincipal Principal, TestRunField Field) : INotification;
public record class TestCaseRunFieldChangedNotification(ClaimsPrincipal Principal, TestCaseRunField Field) : INotification;
public record class RequirementFieldChangedNotification(ClaimsPrincipal Principal, RequirementField Field) : INotification;
public record class IssueFieldChangedNotification(ClaimsPrincipal Principal, IssueField Field, IssueField? OldField) : INotification;
public record class TestCaseFieldChangedNotification(ClaimsPrincipal Principal, TestCaseField Field, TestCaseField? OldField) : INotification;
