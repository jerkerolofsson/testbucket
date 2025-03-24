
namespace TestBucket.Domain.AI;

public interface IClassifier
{
    Task<string[]> ClassifyAsync(string[] categories, string userPrompt);
}