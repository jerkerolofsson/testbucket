using TestBucket.Contracts.Integrations;
using TestBucket.Domain.AI.Models;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Features.Classification;

public interface IClassifier
{
    /// <summary>
    /// Name of the model used
    /// </summary>
    Task<string?> GetModelNameAsync(ClaimsPrincipal principal, ModelType modelType);

    /// <summary>
    /// Identifies a category from a test case
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="categories"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task<string[]> ClassifyAsync(ClaimsPrincipal principal, string fieldName, string[] categories, TestCase testCase);

    /// <summary>
    /// Identifies a category for a test case
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="fieldName"></param>
    /// <param name="categories"></param>
    /// <param name="issue"></param>
    /// <returns></returns>
    Task<string[]> ClassifyAsync(ClaimsPrincipal principal, string fieldName, IReadOnlyList<GenericVisualEntity> categories, TestCase testCase);
    /// <summary>
    /// Identifies a category for an issue
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="fieldName"></param>
    /// <param name="categories"></param>
    /// <param name="issue"></param>
    /// <returns></returns>
    Task<string[]> ClassifyAsync(ClaimsPrincipal principal, string fieldName, string[] categories, LocalIssue issue);

    /// <summary>
    /// Identifies a category for an issue
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="fieldName"></param>
    /// <param name="categories"></param>
    /// <param name="issue"></param>
    /// <returns></returns>
    Task<string[]> ClassifyAsync(ClaimsPrincipal principal, string fieldName, IReadOnlyList<GenericVisualEntity> categories, LocalIssue issue);
}