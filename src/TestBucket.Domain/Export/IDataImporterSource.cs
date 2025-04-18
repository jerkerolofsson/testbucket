using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Export.Models;

namespace TestBucket.Domain.Export;
public interface IDataImporterSource : IDisposable
{
    IEnumerable<ExportEntity> ReadAll();
}
