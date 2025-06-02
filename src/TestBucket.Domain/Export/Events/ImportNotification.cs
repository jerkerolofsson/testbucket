using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

namespace TestBucket.Domain.Export.Events;
public record class ImportNotification(string TenantId) : INotification
{
}
