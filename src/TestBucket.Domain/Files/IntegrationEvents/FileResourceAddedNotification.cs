using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Files.Models;

namespace TestBucket.Domain.Files.IntegrationEvents
{
    public record class FileResourceAddedNotification(ClaimsPrincipal Principal, FileResource Resource) : INotification;
}
