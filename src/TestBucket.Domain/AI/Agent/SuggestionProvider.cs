using TestBucket.Domain.AI.Agent.Models;

namespace TestBucket.Domain.AI.Agent;
public class SuggestionProvider
{
    public static string GetAiRunTestPrompt(string name) => $"""
                    Run the steps described in the reference description for '{name}' one by one. 
                    Use relevant tools as needed. 
                    After running the steps, if a step failed, output "FAILED: <DescriptiveErrorMessage>" where <DescriptiveErrorMessage> describes what went wrong.
                    If all steps passed, output "PASSED"
                    """;

    public static IReadOnlyList<Suggestion> GetSuggestions(AgentChatContext context)
    {
        List<Suggestion> suggestions = [];
        foreach(var reference in context.References)
        {
            if (reference.EntityTypeName == "Requirement" ||
                reference.EntityTypeName == "Component" ||
                reference.EntityTypeName == "Feature")
            {
                if(reference.EntityTypeName == "Feature")
                {
                    suggestions.Add(new Suggestion($"Give me some test ideas for the feature: '{reference.Name}'", $"Give me some test ideas for the feature: '{reference.Name}'")
                    {
                        AgentId = "test-designer"
                    });
                }
                else
                {
                    suggestions.Add(new Suggestion($"Give me some test ideas for '{reference.Name}'", $"Give me some test ideas for '{reference.Name}'")
                    {
                        AgentId = "test-designer"
                    });
                }

                if (reference.EntityTypeName == "Feature")
                {
                    suggestions.Add(new Suggestion($"Add tests for the feature: '{reference.Name}'", $"Add tests for the feature: '{reference.Name}'")
                    {
                        AgentId = "test-creator"
                    });

                    //suggestions.Add(new Suggestion($"Add requirements for the feature: '{reference.Name}'", $"""
                    //    # Scope
                    //    - Feature: '{reference.Name}'

                    //    # Steps
                    //    1. Collect information about current test cases for the feature
                    //    2. Collect existing requirements for the feature
                    //    3. Design new requirements for the feature
                    //    4. Review the requirements
                    //    5. Add the requirements
                    //    """)
                    //{
                    //    AgentId = "requirement-creator"
                    //});
                    suggestions.Add(new Suggestion($"Add requirements for the feature: '{reference.Name}'", $"""
                        Add requirements for the feature: '{reference.Name}'
                        """)
                    {
                        AgentId = "requirement-creator"
                    });
                }
                else
                {
                    suggestions.Add(new Suggestion($"Add tests for '{reference.Name}'", $"Add tests for '{reference.Name}'")
                    {
                        AgentId = "test-creator"
                    });
                }
            }

            if (reference.EntityTypeName == "TestCase")
            {
                suggestions.Add(new Suggestion($"Run '{reference.Name}' with AI", GetAiRunTestPrompt(reference.Name))
                {
                    Icon = TbIcons.BoldDuoTone.AI
                });

                suggestions.Add(new Suggestion($"Suggest some tests similar to '{reference.Name}'", $"Suggest some tests similar to '{reference.Name}'. Use get-heuristics to get ideas."));
                suggestions.Add(new Suggestion($"Review this test case: {reference.Name}", $"Review this test case: {reference.Name}. Please provide feedback on the test coverage and if there are any problems with this test case."));
            }

            if (reference.EntityTypeName == "Issue")
            {
                suggestions.Add(new Suggestion("Summarize this issue", $"""
                    1. Summarize the issue: {reference.Name}.
                    """));
                    
                suggestions.Add(new Suggestion($"Find issues similar to '{reference.Name}'", $"Find issues similar to '{reference.Name}'. Summarize the description and search for similar issues"));
            }

            if (reference.EntityTypeName == "Feature")
            {
                suggestions.Add(new Suggestion("Summarize this feature", $"""
                    1. Search for the feature {reference.Name} using the search_features tool.
                    2. Summarize the feature: {reference.Name} considering both the feature description and the related requirements.
                    """));

                suggestions.Add(new Suggestion("Show me test coverage for this feature", $"""
                    Show me test coverage for the "{reference.Name}" feature
                    """));
            }
        }

        if(suggestions.Count == 0)
        {
            suggestions.Add(new Suggestion("What is the next milestone", $"""
                1. Use the 'list-milestones' tool to collect information about milestones.
                2. Which is the next milestone?
                """)
            {
                Icon = TbIcons.BoldDuoTone.AI
            });

            suggestions.Add(new Suggestion("Summarize all open issues", $"""
                1. Use the 'list-open-issues' tool to list all open issues
                2. Show the issues in a table, with a summary in one column and the issue ID in the second column.
                3. Group issues by similarity, showing another table with the summary and all related issue IDs in the second column.
                """)
            {
                Icon = TbIcons.BoldDuoTone.Bug
            });
        }

        return suggestions;
    }
}
