using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Code.Models;

namespace TestBucket.Domain.Code.Events;
public record class CommitAddedEvent(ClaimsPrincipal Principal, Commit Commit) : INotification; 
