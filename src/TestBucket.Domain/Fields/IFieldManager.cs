using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields;

/// <summary>
/// Field values for various entities: Requirements, Test Cases etc..
/// </summary>
public interface IFieldManager
{
    #region Requirements
    /// <summary>
    /// Gets all requirement fields for a specific requirement identified by the ID
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="id"></param>
    /// <param name="fieldDefinitions"></param>
    /// <returns></returns>
    Task<IReadOnlyList<RequirementField>> GetRequirementFieldsAsync(ClaimsPrincipal principal, long id, IEnumerable<FieldDefinition> fieldDefinitions);

    /// <summary>
    /// Saves requirement fields
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    Task SaveRequirementFieldsAsync(ClaimsPrincipal principal, IEnumerable<RequirementField> fields);

    #endregion Requirements

    #region Test Case
    /// <summary>
    /// Returns test case fields, with default values for current field definitions
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="id">Test case ID</param>
    /// <param name="fieldDefinitions"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TestCaseField>> GetTestCaseFieldsAsync(ClaimsPrincipal principal, long id, IEnumerable<FieldDefinition> fieldDefinitions);
    Task SaveTestCaseFieldsAsync(ClaimsPrincipal principal, IEnumerable<TestCaseField> fields);
    #endregion Test Case

    #region Test Case Run
    Task<IReadOnlyList<TestCaseRunField>> GetTestCaseRunFieldsAsync(ClaimsPrincipal principal, long testRunId, long testCaseRunId, IEnumerable<FieldDefinition> fieldDefinitions);
    Task SaveTestCaseRunFieldsAsync(ClaimsPrincipal principal, IEnumerable<TestCaseRunField> fields);
    #endregion Test Case Run

    Task UpsertTestCaseRunFieldAsync(ClaimsPrincipal principal, TestCaseRunField field);
    Task UpsertTestCaseFieldAsync(ClaimsPrincipal principal, TestCaseField field);
    Task UpsertTestRunFieldAsync(ClaimsPrincipal principal, TestRunField field);
    Task UpsertRequirementFieldAsync(ClaimsPrincipal principal, RequirementField field);


    #region Test Run
    Task<IReadOnlyList<TestRunField>> GetTestRunFieldsAsync(ClaimsPrincipal principal, long testRunId, IEnumerable<FieldDefinition> fieldDefinitions);
    Task SaveTestRunFieldsAsync(ClaimsPrincipal principal, IEnumerable<TestRunField> fields);
    #endregion Test Run
}