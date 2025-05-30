using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Traits.Core;

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

    Task UpsertRequirementFieldAsync(ClaimsPrincipal principal, RequirementField field);
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

    Task UpsertTestCaseFieldAsync(ClaimsPrincipal principal, TestCaseField field);


    #endregion Test Case

    #region Test Case Run
    Task<IReadOnlyList<TestCaseRunField>> GetTestCaseRunFieldsAsync(ClaimsPrincipal principal, long testRunId, long testCaseRunId, IEnumerable<FieldDefinition> fieldDefinitions);
    Task UpsertTestCaseRunFieldAsync(ClaimsPrincipal principal, TestCaseRunField field);
    #endregion Test Case Run



    #region Issues
    Task UpsertIssueFieldAsync(ClaimsPrincipal principal, IssueField field);
    Task<IReadOnlyList<IssueField>> GetIssueFieldsAsync(ClaimsPrincipal principal, long id, IEnumerable<FieldDefinition> fieldDefinitions);

    #endregion

    #region Test Run
    Task<IReadOnlyList<TestRunField>> GetTestRunFieldsAsync(ClaimsPrincipal principal, long testRunId, IEnumerable<FieldDefinition> fieldDefinitions);
    Task UpsertTestRunFieldAsync(ClaimsPrincipal principal, TestRunField field);
    Task<bool> SetIssueFieldAsync(ClaimsPrincipal principal, long projectId, long issueId, Predicate<IssueField> fieldPredicate, string value);
    Task<IssueField?> GetIssueFieldAsync(ClaimsPrincipal principal, long projectId, long issueId, Predicate<IssueField> fieldPredicate, string value);
    Task<bool> SetIssueFieldAsync(ClaimsPrincipal principal, long projectId, long issueId, TraitType traitType, string value);
    Task<bool> SetTestRunFieldAsync(ClaimsPrincipal principal, long projectId, long testRunId, TraitType traitType, string value);
    Task<TestRunField?> GetTestRunFieldAsync(ClaimsPrincipal principal, long projectId, long testRunId, Predicate<TestRunField> fieldPredicate, string value);
    Task<bool> SetTestRunFieldAsync(ClaimsPrincipal principal, long projectId, long testRunId, Predicate<TestRunField> fieldPredicate, string value);

    #endregion Test Run
}