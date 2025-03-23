using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Fields.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields;
public interface IFieldRepository
{
    #region Field Definitions
    /// <summary>
    /// Adds a new field definition
    /// </summary>
    /// <param name="fieldDefinition"></param>
    /// <returns></returns>
    Task AddAsync(FieldDefinition fieldDefinition);

    /// <summary>
    /// Deletes a field
    /// </summary>
    /// <param name="fieldDefinition"></param>
    /// <returns></returns>
    Task DeleteAsync(FieldDefinition fieldDefinition);

    /// <summary>
    /// Saves a field definition
    /// </summary>
    /// <param name="fieldDefinition"></param>
    /// <returns></returns>
    Task UpdateAsync(FieldDefinition fieldDefinition);

    /// <summary>
    /// Searches for field definitions
    /// </summary>
    /// <param name="specifications"></param>
    /// <returns></returns>
    Task<IReadOnlyList<FieldDefinition>> SearchAsync(IReadOnlyList<FilterSpecification<FieldDefinition>> specifications);

    #endregion Field Definitions

    #region Test Case

    Task<IReadOnlyList<TestCaseField>> GetTestCaseFieldsAsync(string tenantId, long testCaseId);

    /// <summary>
    /// Saves all test case fields
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    Task SaveTestCaseFieldsAsync(IEnumerable<TestCaseField> fields);

    /// <summary>
    /// Adds a new field for the test case if it doesn't exist or replaces the value
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    Task UpsertTestCaseFieldsAsync(TestCaseField field);

    #endregion

    #region Test Run

    /// <summary>
    /// Gets all test run fields
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="testCaseRunId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TestRunField>> GetTestRunFieldsAsync(string tenantId, long testCaseRunId);

    /// <summary>
    /// Saves all test run fields
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    Task SaveTestRunFieldsAsync(IEnumerable<TestRunField> fields);

    /// <summary>
    /// Adds a new field for the test run if it doesn't exist or replaces the value
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    Task UpsertTestRunFieldsAsync(TestRunField field);

    #endregion Test Case Runs
    #region Test Case Run

    Task<IReadOnlyList<TestCaseRunField>> GetTestCaseRunFieldsAsync(string tenantId, long testCaseRunId);

    /// <summary>
    /// Saves all test case fields
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    Task SaveTestCaseRunFieldsAsync(IEnumerable<TestCaseRunField> fields);

    /// <summary>
    /// Adds a new field for the test case if it doesn't exist or replaces the value
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    Task UpsertTestCaseRunFieldsAsync(TestCaseRunField field);

    #endregion Test Case Runs

}
