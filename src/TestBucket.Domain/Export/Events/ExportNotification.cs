using Mediator;

using TestBucket.Domain.Export.Models;
using TestBucket.Domain.Progress;

namespace TestBucket.Domain.Export.Events;
public record class ExportNotification(ClaimsPrincipal Principal, string TenantId, ExportOptions Options, IDataExporterSink Sink, ProgressTask progressTask) : INotification
{
   
}
