using TestBucket.Domain.AI.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.AI.Services.Classifier;

public interface IClassifier
{
    /// <summary>
    /// Name of the model used
    /// </summary>
    Task<string?> GetModelNameAsync(ModelType modelType);

    /// <summary>
    /// Identifies a category from a test case
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="categories"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task<string[]> ClassifyAsync(ClaimsPrincipal principal, string fieldName, string[] categories, TestCase testCase);
}