using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Contracts.Runners.Models;

namespace TestBucket.Domain.Automation.Runners.Registration
{
    public record class RunnerConnectedEvent(ClaimsPrincipal Principal, ConnectRequest Request) : INotification;
}
