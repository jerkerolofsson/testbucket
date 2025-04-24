using System.Security.Claims;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields;
public interface IFieldManager
{
    /// <summary>
    /// Returns test case fields, with default values for current field definitions
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="id">Test case ID</param>
    /// <param name="fieldDefinitions"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TestCaseField>> GetTestCaseFieldsAsync(ClaimsPrincipal principal, long id, IEnumerable<FieldDefinition> fieldDefinitions);
    Task<IReadOnlyList<RequirementField>> GetRequirementFieldsAsync(ClaimsPrincipal principal, long id, IEnumerable<FieldDefinition> fieldDefinitions);
    Task<IReadOnlyList<TestCaseRunField>> GetTestCaseRunFieldsAsync(ClaimsPrincipal principal, long testRunId, long testCaseRunId, IEnumerable<FieldDefinition> fieldDefinitions);
    Task<IReadOnlyList<TestRunField>> GetTestRunFieldsAsync(ClaimsPrincipal principal, long testRunId, IEnumerable<FieldDefinition> fieldDefinitions);
    Task SaveTestCaseFieldsAsync(ClaimsPrincipal principal, IEnumerable<TestCaseField> fields);
    Task SaveTestCaseRunFieldsAsync(ClaimsPrincipal principal, IEnumerable<TestCaseRunField> fields);
    Task SaveTestRunFieldsAsync(ClaimsPrincipal principal, IEnumerable<TestRunField> fields);
}