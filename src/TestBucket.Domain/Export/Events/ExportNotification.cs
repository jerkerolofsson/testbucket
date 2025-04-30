using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Export.Models;
using TestBucket.Domain.Progress;

namespace TestBucket.Domain.Export.Events;
public record class ExportNotification(string TenantId, ExportOptions Options, IDataExporterSink Sink, ProgressTask progressTask) : INotification;
