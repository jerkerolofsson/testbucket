﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Mediator;

namespace TestBucket.Domain.Export.Handlers
{
    public static class SinkJsonWriterExtension
    {
        public static async Task WriteJsonEntityAsync<T>(this IDataExporterSink sink, string source, string entityType, string entityId, T dto, CancellationToken cancellationToken)
        {
            using var stream = new MemoryStream();
            var options = new JsonSerializerOptions { WriteIndented = true };
            await JsonSerializer.SerializeAsync(stream, dto, options, cancellationToken);
            stream.Seek(0, SeekOrigin.Begin);
            await sink.WriteEntityAsync(source, entityType, entityId, stream, cancellationToken);
        }
    }
}
