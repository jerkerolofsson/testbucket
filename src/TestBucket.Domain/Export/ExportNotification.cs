using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Progress;

namespace TestBucket.Domain.Export;
public record class ExportNotification(string TenantId, IDataExporterSink Sink, IProgressObserver Progress) : INotification;
