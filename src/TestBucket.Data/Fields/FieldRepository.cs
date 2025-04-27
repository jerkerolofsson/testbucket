using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Fields.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Data.Fields;
internal class FieldRepository : IFieldRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public FieldRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    #region Field Definitions

    public async Task AddAsync(FieldDefinition fieldDefinition)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.AddAsync(fieldDefinition);
        await dbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<FieldDefinition>> SearchAsync(IReadOnlyList<FilterSpecification<FieldDefinition>> specifications)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var fields = dbContext.FieldDefinitions.AsNoTracking().Where(x =>x.IsDeleted == false);

        foreach(var spec in specifications)
        {
            fields = fields.Where(spec.Expression);
        }

        return await fields.OrderBy(x => x.Name).ToListAsync();
    }



    public async Task UpdateAsync(FieldDefinition fieldDefinition)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Update(fieldDefinition);
        await dbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(FieldDefinition fieldDefinition)
    {
        fieldDefinition.IsDeleted = true;
        await UpdateAsync(fieldDefinition);
    }

    #endregion Field Definitions

    #region Test Case

    public async Task UpsertTestCaseFieldAsync(TestCaseField field)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var existingField = await dbContext.TestCaseFields
            .Where(x => x.FieldDefinitionId == field.FieldDefinitionId && x.TestCaseId == field.TestCaseId && x.TenantId == field.TenantId).FirstOrDefaultAsync();
        if (existingField is not null)
        {
            field.CopyTo(existingField);
            dbContext.TestCaseFields.Update(existingField);
        }
        else
        {
            var tmp = field.FieldDefinition;
            field.FieldDefinition = null;
            await dbContext.TestCaseFields.AddAsync(field);
            field.FieldDefinition = tmp;
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task SaveTestCaseFieldsAsync(IEnumerable<TestCaseField> fields)
    {
        if (fields.Count() == 0)
        {
            return;
        }

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        await dbContext.TestCaseFields.AsNoTracking().Where(x => x.TestCaseId == fields.First().TestCaseId).ExecuteDeleteAsync();

        foreach(var field in fields)
        {
            var tmp = field.FieldDefinition;
            field.FieldDefinition = null;
            await dbContext.TestCaseFields.AddAsync(field);
            field.FieldDefinition = tmp;
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<TestCaseField>> GetTestCaseFieldsAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var fields = dbContext.TestCaseFields.AsNoTracking()
            .Include(x=>x.FieldDefinition)
            .Where(x => x.TenantId == tenantId && x.TestCaseId == id && x.FieldDefinition!.IsDeleted == false);
        return await fields.ToListAsync();
    }
    #endregion Test Case

    #region Test Run

    public async Task UpsertTestRunFieldAsync(TestRunField field)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var existingField = await dbContext.TestRunFields
            .Where(x => x.FieldDefinitionId == field.FieldDefinitionId && x.TestRunId == field.TestRunId && x.TenantId == field.TenantId).FirstOrDefaultAsync();
        if (existingField is not null)
        {
            field.CopyTo(existingField);
            dbContext.TestRunFields.Update(existingField);
        }
        else
        {
            var tmp = field.FieldDefinition;
            field.FieldDefinition = null;
            await dbContext.TestRunFields.AddAsync(field);
            field.FieldDefinition = tmp;
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task SaveTestRunFieldsAsync(IEnumerable<TestRunField> fields)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        foreach (var field in fields)
        {
            if (field.Id > 0)
            {
                dbContext.TestRunFields.Update(field);
            }
            else
            {
                var tmp = field.FieldDefinition;
                field.FieldDefinition = null;
                await dbContext.TestRunFields.AddAsync(field);
                field.FieldDefinition = tmp;
            }
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<TestRunField>> GetTestRunFieldsAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var fields = dbContext.TestRunFields.AsNoTracking()
            .Include(x => x.FieldDefinition)
            .Where(x => x.TenantId == tenantId && x.TestRunId == id && x.FieldDefinition!.IsDeleted == false);
        return await fields.ToListAsync();
    }

    #endregion Test Case Run

    #region Test Case Run

    public async Task UpsertTestCaseRunFieldsAsync(TestCaseRunField field)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var existingField = await dbContext.TestCaseRunFields
            .Where(x => x.FieldDefinitionId == field.FieldDefinitionId && x.TestCaseRunId == field.TestCaseRunId && x.TenantId == field.TenantId).FirstOrDefaultAsync();
        if (existingField is not null)
        {
            field.CopyTo(existingField);
            dbContext.TestCaseRunFields.Update(existingField);
        }
        else
        {
            var tmp = field.FieldDefinition;
            field.FieldDefinition = null;
            await dbContext.TestCaseRunFields.AddAsync(field);
            field.FieldDefinition = tmp;
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task SaveTestCaseRunFieldsAsync(IEnumerable<TestCaseRunField> fields)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        foreach (var field in fields)
        {
            if (field.Id > 0)
            {
                dbContext.TestCaseRunFields.Update(field);
            }
            else
            {
                var tmp = field.FieldDefinition;
                field.FieldDefinition = null;
                await dbContext.TestCaseRunFields.AddAsync(field);
                field.FieldDefinition = tmp;
            }
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<TestCaseRunField>> GetTestCaseRunFieldsAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var fields = dbContext.TestCaseRunFields.AsNoTracking()
            .Include(x => x.FieldDefinition)
            .Where(x => x.TenantId == tenantId && x.TestCaseRunId == id && x.FieldDefinition!.IsDeleted == false);
        return await fields.ToListAsync();
    }

    #endregion Test Case Run

    #region Requirement

    public async Task UpsertRequirementFieldsAsync(RequirementField field)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var existingField = await dbContext.RequirementFields
            .Where(x => x.FieldDefinitionId == field.FieldDefinitionId && x.RequirementId == field.RequirementId && x.TenantId == field.TenantId).FirstOrDefaultAsync();
        if (existingField is not null)
        {
            field.CopyTo(existingField);
            dbContext.RequirementFields.Update(existingField);
        }
        else
        {
            var tmp = field.FieldDefinition;
            field.FieldDefinition = null;
            await dbContext.RequirementFields.AddAsync(field);
            field.FieldDefinition = tmp;
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task SaveRequirementFieldsAsync(IEnumerable<RequirementField> fields)
    {
        if (fields.Count() == 0)
        {
            return;
        }

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        await dbContext.RequirementFields.AsNoTracking().Where(x => x.RequirementId == fields.First().RequirementId).ExecuteDeleteAsync();

        foreach (var field in fields)
        {
            var tmp = field.FieldDefinition;
            field.FieldDefinition = null;
            await dbContext.RequirementFields.AddAsync(field);
            field.FieldDefinition = tmp;
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<RequirementField>> GetRequirementFieldsAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var fields = dbContext.RequirementFields.AsNoTracking()
            .Include(x => x.FieldDefinition)
            .Where(x => x.TenantId == tenantId && x.RequirementId == id && x.FieldDefinition!.IsDeleted == false);
        return await fields.ToListAsync();
    }
    #endregion Requirement


}
