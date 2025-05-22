using TestBucket.Domain.AI.Models;

namespace TestBucket.Domain.AI.Services.Classifier;

public interface IClassifier
{
    /// <summary>
    /// Name of the model used
    /// </summary>
    Task<string?> GetModelNameAsync(ModelType modelType);

    /// <summary>
    /// Identifies a category from a user prompt
    /// </summary>
    /// <param name="categories"></param>
    /// <param name="userPrompt"></param>
    /// <returns></returns>
    Task<string[]> ClassifyAsync(string[] categories, string userPrompt);
}