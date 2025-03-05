using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Fields.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields;
public interface IFieldRepository
{
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
    /// Searches for fields
    /// </summary>
    /// <returns></returns>
    Task<IReadOnlyList<FieldDefinition>> SearchAsync(string tenantId, SearchQuery query);
    Task<IReadOnlyList<TestCaseField>> GetTestCaseFieldsAsync(string tenantId, long id);

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
}
