using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Export.Models;

namespace TestBucket.Domain.Export.Zip;
public class ZipExporter : IDataExporterSink
{
    private readonly ZipArchive _zip;

    /// <summary>
    /// Creates a new sink that will write the items as entities in a zip file.
    /// The stream must be writable and will be not be closed.
    /// </summary>
    /// <param name="destination"></param>
    public ZipExporter(Stream destination)
    {
        _zip = new ZipArchive(destination, ZipArchiveMode.Create, leaveOpen: true);
    }

    public void Dispose()
    {
        _zip.Dispose();
    }

    public async Task WriteEntityAsync(string source, string entityType, string entityId, Stream sourceStream, CancellationToken cancellationToken)
    {
        string path = $"{source}/{entityType}/{entityId}";
        var entry = _zip.CreateEntry(path);
        using var destinationStream = entry.Open();
        await sourceStream.CopyToAsync(destinationStream, cancellationToken);
        await sourceStream.FlushAsync(cancellationToken);
    }
}
