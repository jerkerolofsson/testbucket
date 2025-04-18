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

    public async Task ExportFullAsync(ExportFormat format, string tenantId, Stream destinationStream, ProgressTask progressTask)
    {
        IDataExporterSink? sink = null;
        switch (format)
        {
            case ExportFormat.Zip:
                sink = new Zip.ZipExporter(destinationStream);
                break;
            default:
                throw new NotImplementedException($"Format not implemented: {format}");
        }
        await _mediator.Publish(new ExportNotification(tenantId, sink, progressTask));

        sink.Dispose();
    }

    public async Task ExportFullZipAsync(string tenantId, Stream destinationStream, ProgressTask progressTask)
    {
        var sink = new Zip.ZipExporter(destinationStream);
        await _mediator.Publish(new ExportNotification(tenantId, sink, progressTask));
    }
}
