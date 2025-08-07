using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using TestBucket.Domain.Audit;
using TestBucket.Domain.Audit.Models;

namespace TestBucket.Data.Audit;
internal class AuditRepository : IAuditRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public AuditRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }


    public async Task AddEntryAsync(AuditEntry entry)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        db.AuditEntries.Add(entry);
        await db.SaveChangesAsync();
    }

    public async Task DeleteEntryAsync(AuditEntry entry)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        db.AuditEntries.Remove(entry);
        await db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<AuditEntry>> GetEntriesAsync(string entityType, long entityId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        var entries = await db.AuditEntries
            .Where(e => e.EntityType == entityType && e.EntityId == entityId)
            .OrderBy(e => e.Created)
            .ToListAsync();
        return entries;
    }
}
