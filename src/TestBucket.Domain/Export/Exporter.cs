using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Progress;

namespace TestBucket.Domain.Export;
public class Exporter
{
    private readonly IMediator _mediator;

    public Exporter(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task ExportFullZipAsync(string tenantId, string destinationPath, IProgressObserver progressObserver)
    {
        using var stream = File.Create(destinationPath);
        var sink = new Zip.ZipExporter(stream);

        await _mediator.Publish(new ExportNotification(tenantId, sink, progressObserver));
    }
}
