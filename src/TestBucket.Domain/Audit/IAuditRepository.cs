using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Audit.Models;

namespace TestBucket.Domain.Audit;
public interface IAuditRepository
{
    Task AddEntryAsync(AuditEntry entry);
    Task DeleteEntryAsync(AuditEntry entry);
    Task<IReadOnlyList<AuditEntry>> GetEntriesAsync(string entityType, long entityId);
}
