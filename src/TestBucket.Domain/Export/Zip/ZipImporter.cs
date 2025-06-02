using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;

using TestBucket.Contracts.Projects;
using TestBucket.Domain.Export.Models;

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

    public async Task<T?> DeserializeEntityAsync<T>(Predicate<ExportEntity> filter, CancellationToken cancellationToken) where T : class
    {
        var entity = Find(filter);
        return await DeserializeEntityAsync<T>(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task<T?> DeserializeEntityAsync<T>(ExportEntity? entity, CancellationToken cancellationToken) where T : class
    {
        if (entity == null)
        {
            return null;
        }
        using var entityStream = entity.Open();
        return await JsonSerializer.DeserializeAsync<T>(entityStream, new JsonSerializerOptions());
    }

    /// <summary>
    /// Finds an exported entity
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    public ExportEntity? Find(Predicate<ExportEntity> filter)
    {
        foreach(var entity in ReadAll())
        {
            if(filter(entity))
            {
                return entity;
            }
        }
        return null;
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

    //public async Task WriteEntityAsync(string source, string entityType, string entityId, Stream sourceStream)
    //{
    //    string path = $"{source}/{entityType}/{entityId}";
    //    var entry = _zip.CreateEntry(path);
    //    using var destinationStream = entry.Open();
    //    await sourceStream.CopyToAsync(destinationStream);
    //}
}
