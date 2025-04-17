using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Export;
public interface IDataExporterSink : IDisposable
{
    Task WriteEntityAsync(string source, string entityType, string entityId, Stream sourceStream, CancellationToken cancellationToken);

}
