using TestBucket.Domain.AI.Agent.Models;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.AI.Agent;
public class ChatReferenceBuilder
{
    public static ChatReference Create(TestCase testCase)
    {
        return new ChatReference { Name = testCase.Name, Text = testCase.Description ?? "", Id = testCase.Id, EntityTypeName = "TestCase" };
    }
    public static ChatReference Create(TestSuite testSuite)
    {
        return new ChatReference { Name = testSuite.Name, Text = testSuite.Description ?? "", Id = testSuite.Id, EntityTypeName = "TestSuite" };
    }
    public static ChatReference Create(Requirement item)
    {
        return new ChatReference { Name = item.Name, Text = item.Description ?? "", Id = item.Id, EntityTypeName = "Requirement" };
    }
    public static ChatReference Create(Feature item)
    {
        return new ChatReference { Name = item.Name, Text = item.Description ?? "", Id = item.Id, EntityTypeName = "Feature" };
    }
    public static ChatReference Create(Component item)
    {
        return new ChatReference { Name = item.Name, Text = item.Description ?? "", Id = item.Id, EntityTypeName = "Component" };
    }
}
