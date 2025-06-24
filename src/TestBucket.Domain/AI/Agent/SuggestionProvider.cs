using TestBucket.Domain.AI.Agent.Models;

namespace TestBucket.Domain.AI.Agent;
public class SuggestionProvider
{
    public static IReadOnlyList<Suggestion> GetSuggestions(AgentChatContext context)
    {
        string suggestTests = $"get-heuristics. Consider different heuristics and the project and use a relevant heuristic to come up with some test ideas. Format the test case as markdown";
        string addTests = $"get-heuristics. Consider different heuristics and the project and use a relevant heuristic to come up with some test ideas. For each test idea add-test-case with the step parameter as a string.";

        List<Suggestion> suggestions = [];
        foreach(var reference in context.References)
        {
            if (reference.EntityTypeName == "Requirement" ||
                reference.EntityTypeName == "Component" ||
                reference.EntityTypeName == "Feature")
            {
                suggestions.Add(new Suggestion($"Give me some test ideas for '{reference.Name}'", suggestTests));
                suggestions.Add(new Suggestion($"Add some tests for '{reference.Name}'", addTests));
            }

            if (reference.EntityTypeName == "TestCase")
            {
                suggestions.Add(new Suggestion($"Suggest some tests similar to '{reference.Name}'", $"Suggest some tests similar to '{reference.Name}'. Use get-heuristics to get ideas."));
                suggestions.Add(new Suggestion($"Review this test case: {reference.Name}", $"Review this test case: {reference.Name}. Please provide feedback on the test coverage and if there are any problems with this test case."));
            }


            if (reference.EntityTypeName == "Feature")
            {
                suggestions.Add(new Suggestion("Summarize this feature", $"Please summarize the feature: {reference.Name}."));
            }
        }

        if(suggestions.Count == 0)
        {
            suggestions.Add(new Suggestion("What is the next milestone", $"list-milestones. What is the next milestone?"));
        }

        return suggestions;
    }
}
