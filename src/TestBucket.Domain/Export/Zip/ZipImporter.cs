using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Export.Zip;
public class ZipImporter : IDataImporterSource
{
    private readonly ZipArchive _zip;

    internal class ZipEntry : ExportEntity
    {
        private readonly ZipArchiveEntry _entry;
        public ZipEntry(ZipArchiveEntry entry)
        {
            _entry = entry;
        }

        public override Stream Open()
        {
            return _entry.Open();
        }
    }

    public ZipImporter(Stream stream)
    {
        _zip = new ZipArchive(stream, ZipArchiveMode.Read);
    }

    public void Dispose()
    {
        _zip.Dispose();
    }

    public IEnumerable<ExportEntity> ReadAll()
    {
        foreach (var entry in _zip.Entries)
        {
            var pathItems = entry.FullName.Split('/');
            if(pathItems.Length == 3)
            {
                yield return new ZipEntry(entry) { Source  = pathItems[0], Type = pathItems[1], Id = pathItems[2] };
            }
        }
    }

    public async Task WriteEntityAsync(string source, string entityType, string entityId, Stream sourceStream)
    {
        string path = $"{source}/{entityType}/{entityId}";
        var entry = _zip.CreateEntry(path);
        using var destinationStream = entry.Open();
        await sourceStream.CopyToAsync(destinationStream);
    }
}
