using TestBucket.Domain.Fields;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.UnitTests.Fakes;
internal class FakeFieldRepository : IFieldRepository
{
    private readonly List<FieldDefinition> _fieldDefinitions = new();
    private readonly List<IssueField> _issueFields = new();
    private readonly List<RequirementField> _requirementFields = new();
    private readonly List<TestCaseField> _testCaseFields = new();
    private readonly List<TestCaseRunField> _testCaseRunFields = new();
    private readonly List<TestRunField> _testRunFields = new();

    private long _idCounter = 0;

    public void Clear()
    {
        _fieldDefinitions.Clear();
        _issueFields.Clear();
        _requirementFields.Clear();
        _testCaseFields.Clear();
        _testCaseRunFields.Clear();
        _testRunFields.Clear();
    }

    public Task AddAsync(FieldDefinition fieldDefinition)
    {
        if (fieldDefinition.Id == 0)
        {
            fieldDefinition.Id = _idCounter++;
        }
        _fieldDefinitions.Add(fieldDefinition);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(FieldDefinition fieldDefinition)
    {
        _fieldDefinitions.Remove(fieldDefinition);
        return Task.CompletedTask;
    }

    public Task<FieldDefinition?> GetDefinitionByIdAsync(long id)
    {
        var fieldDefinition = _fieldDefinitions.FirstOrDefault(fd => fd.Id == id);
        return Task.FromResult(fieldDefinition);
    }

    public Task<IReadOnlyList<IssueField>> GetIssueFieldsAsync(string tenantId, long issueId)
    {
        var fields = _issueFields.Where(f => f.TenantId == tenantId && f.LocalIssueId == issueId).ToList();
        return Task.FromResult<IReadOnlyList<IssueField>>(fields);
    }

    public Task<IReadOnlyList<RequirementField>> GetRequirementFieldsAsync(string tenantId, long requirementId)
    {
        var fields = _requirementFields.Where(f => f.TenantId == tenantId && f.RequirementId == requirementId).ToList();
        return Task.FromResult<IReadOnlyList<RequirementField>>(fields);
    }

    public Task<IReadOnlyList<TestCaseField>> GetTestCaseFieldsAsync(string tenantId, long testCaseId)
    {
        var fields = _testCaseFields.Where(f => f.TenantId == tenantId && f.TestCaseId == testCaseId).ToList();
        return Task.FromResult<IReadOnlyList<TestCaseField>>(fields);
    }

    public Task<IReadOnlyList<TestCaseRunField>> GetTestCaseRunFieldsAsync(string tenantId, long testCaseRunId)
    {
        var fields = _testCaseRunFields.Where(f => f.TenantId == tenantId && f.TestCaseRunId == testCaseRunId).ToList();
        return Task.FromResult<IReadOnlyList<TestCaseRunField>>(fields);
    }

    public Task<IReadOnlyList<TestRunField>> GetTestRunFieldsAsync(string tenantId, long testCaseRunId)
    {
        var fields = _testRunFields.Where(f => f.TenantId == tenantId && f.TestRunId == testCaseRunId).ToList();
        return Task.FromResult<IReadOnlyList<TestRunField>>(fields);
    }

    public Task<IReadOnlyList<FieldDefinition>> SearchAsync(IReadOnlyList<FilterSpecification<FieldDefinition>> specifications)
    {
        var query = _fieldDefinitions.AsQueryable();
        foreach (var spec in specifications)
        {
            query = query.Where(spec.Expression);
        }
        return Task.FromResult<IReadOnlyList<FieldDefinition>>(query.ToList());
    }

    public Task UpdateAsync(FieldDefinition fieldDefinition)
    {
        var existing = _fieldDefinitions.FirstOrDefault(fd => fd.Id == fieldDefinition.Id);
        if (existing != null)
        {
            _fieldDefinitions.Remove(existing);
            _fieldDefinitions.Add(fieldDefinition);
        }
        return Task.CompletedTask;
    }

    public Task UpsertIssueFieldAsync(IssueField field)
    {
        var existing = _issueFields.FirstOrDefault(f => f.Id == field.Id);
        if (existing != null)
        {
            _issueFields.Remove(existing);
        }
        _issueFields.Add(field);
        return Task.CompletedTask;
    }

    public Task UpsertRequirementFieldAsync(RequirementField field)
    {
        var existing = _requirementFields.FirstOrDefault(f => f.Id == field.Id);
        if (existing != null)
        {
            _requirementFields.Remove(existing);
        }
        _requirementFields.Add(field);
        return Task.CompletedTask;
    }

    public Task UpsertTestCaseFieldAsync(TestCaseField field)
    {
        var existing = _testCaseFields.FirstOrDefault(f => f.Id == field.Id);
        if (existing != null)
        {
            _testCaseFields.Remove(existing);
        }
        _testCaseFields.Add(field);
        return Task.CompletedTask;
    }

    public Task UpsertTestCaseRunFieldAsync(TestCaseRunField field)
    {
        var existing = _testCaseRunFields.FirstOrDefault(f => f.Id == field.Id);
        if (existing != null)
        {
            _testCaseRunFields.Remove(existing);
        }
        _testCaseRunFields.Add(field);
        return Task.CompletedTask;
    }

    public Task UpsertTestRunFieldAsync(TestRunField field)
    {
        var existing = _testRunFields.FirstOrDefault(f => f.Id == field.Id);
        if (existing != null)
        {
            _testRunFields.Remove(existing);
        }
        _testRunFields.Add(field);
        return Task.CompletedTask;
    }
}
