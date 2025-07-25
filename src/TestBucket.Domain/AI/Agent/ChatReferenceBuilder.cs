using System.Text;

using TestBucket.Domain.AI.Agent.Models;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.AI.Agent;
public class ChatReferenceBuilder
{
    public static ChatReference Create(TestProject project, bool isActiveDocument = false)
    {

        var description = new StringBuilder();
        if(project.Description is not null)
        {
            description.AppendLine("# Project description");
            description.AppendLine(project.Description);
        }
        if (project.ExternalSystems is not null )
        {
            foreach(var externalSystem in project.ExternalSystems)
            {
                if (externalSystem.ExternalProjectId is not null && externalSystem.Provider?.Equals("github", StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    if(externalSystem.ExternalProjectId.Contains('/'))
                    {
                        var items = externalSystem.ExternalProjectId.Split('/');
                        description.AppendLine();
                        description.AppendLine("Use this information when invoking GitHub tools:");
                        description.AppendLine($"- Owner: {items[0]}");
                        description.AppendLine($"- Repository: {items[1]}");
                    }
                }
            }
        }

        return new ChatReference { Name = project.Name, Text = description.ToString(), Id = project.Id.ToString(), EntityTypeName = "Project", IsActiveDocument = isActiveDocument };
    }
    public static ChatReference Create(TestCase testCase, bool isActiveDocument = false)
    {
        return new ChatReference 
        { 
            Name = testCase.Name, Text = testCase.Description ?? "", Id = testCase.Id.ToString(), EntityTypeName = "TestCase", IsActiveDocument = isActiveDocument };
    }
    public static ChatReference Create(TestSuite testSuite, bool isActiveDocument = false)
    {
        return new ChatReference { Name = testSuite.Name, Text = testSuite.Description ?? "", Id = testSuite.Id.ToString(), EntityTypeName = "TestSuite", IsActiveDocument = isActiveDocument };
    }
    public static ChatReference Create(Requirement item, bool isActiveDocument = false)
    {
        return new ChatReference { Name = item.Name, Text = item.Description ?? "", Id = item.Id.ToString(), EntityTypeName = "Requirement", IsActiveDocument = isActiveDocument };
    }
    public static ChatReference Create(Feature item, bool isActiveDocument = false)
    {
        return new ChatReference { Name = item.Name, Text = item.Description ?? "", Id = item.Id.ToString(), EntityTypeName = "Feature", IsActiveDocument = isActiveDocument };
    }
    public static ChatReference Create(Component item, bool isActiveDocument = false)
    {
        return new ChatReference { Name = item.Name, Text = item.Description ?? "", Id = item.Id.ToString(), EntityTypeName = "Component", IsActiveDocument = isActiveDocument };
    }
    public static ChatReference Create(LocalIssue item, bool isActiveDocument = false)
    {
        return new ChatReference { Name = item.Title ?? "", Text = item.Description ?? "", Id = item.ExternalDisplayId ?? "", EntityTypeName = "Issue", IsActiveDocument = isActiveDocument };
    }
}
