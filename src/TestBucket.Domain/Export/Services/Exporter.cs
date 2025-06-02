using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Export.Events;
using TestBucket.Domain.Export.Models;
using TestBucket.Domain.Progress;

namespace TestBucket.Domain.Export.Services;
public class Exporter
{
    private readonly IMediator _mediator;

    public Exporter(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Performs a full export
    /// </summary>
    /// <param name="options"></param>
    /// <param name="tenantId"></param>
    /// <param name="destinationStream"></param>
    /// <param name="progressTask"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task ExportFullAsync(ClaimsPrincipal principal, ExportOptions options, string tenantId, Stream destinationStream, ProgressTask progressTask)
    {
        IDataExporterSink? sink = null;
        switch (options.ExportFormat)
        {
            case ExportFormat.Zip:
                sink = new Zip.ZipExporter(destinationStream);
                break;
            default:
                throw new NotImplementedException($"Format not implemented: {options.ExportFormat}");
        }

        await sink.WriteEntityAsync("exporter", "tenant", tenantId, new MemoryStream(), CancellationToken.None);

        await _mediator.Publish(new ExportNotification(principal, tenantId, options, sink, progressTask));

        sink.Dispose();
    }
}
