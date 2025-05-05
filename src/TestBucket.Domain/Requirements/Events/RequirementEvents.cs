using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Requirements.Events;
public record RequirementCreatedEvent(ClaimsPrincipal Principal, Requirement Requirement) : INotification;
public record RequirementUpdatedEvent(ClaimsPrincipal Principal, Requirement Requirement) : INotification;
public record RequirementDeletedEvent(ClaimsPrincipal Principal, Requirement Requirement) : INotification;
