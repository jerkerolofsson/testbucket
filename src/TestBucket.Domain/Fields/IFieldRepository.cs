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
    Task SaveTestCaseFieldsAsync(IEnumerable<TestCaseField> fields);
}
