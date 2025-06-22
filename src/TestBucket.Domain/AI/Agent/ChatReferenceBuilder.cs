using TestBucket.Domain.AI.Agent.Models;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.AI.Agent;
public class ChatReferenceBuilder
{
    public static ChatReference Create(TestProject project, bool isActiveDocument = false)
    {
        return new ChatReference { Name = project.Name, Text = project.Description ?? "", Id = project.Id, EntityTypeName = "Project", IsActiveDocument = isActiveDocument };
    }
    public static ChatReference Create(TestCase testCase, bool isActiveDocument = false)
    {
        return new ChatReference { Name = testCase.Name, Text = testCase.Description ?? "", Id = testCase.Id, EntityTypeName = "TestCase", IsActiveDocument = isActiveDocument };
    }
    public static ChatReference Create(TestSuite testSuite, bool isActiveDocument = false)
    {
        return new ChatReference { Name = testSuite.Name, Text = testSuite.Description ?? "", Id = testSuite.Id, EntityTypeName = "TestSuite", IsActiveDocument = isActiveDocument };
    }
    public static ChatReference Create(Requirement item, bool isActiveDocument = false)
    {
        return new ChatReference { Name = item.Name, Text = item.Description ?? "", Id = item.Id, EntityTypeName = "Requirement", IsActiveDocument = isActiveDocument };
    }
    public static ChatReference Create(Feature item, bool isActiveDocument = false)
    {
        return new ChatReference { Name = item.Name, Text = item.Description ?? "", Id = item.Id, EntityTypeName = "Feature", IsActiveDocument = isActiveDocument };
    }
    public static ChatReference Create(Component item, bool isActiveDocument = false)
    {
        return new ChatReference { Name = item.Name, Text = item.Description ?? "", Id = item.Id, EntityTypeName = "Component", IsActiveDocument = isActiveDocument };
    }
}
