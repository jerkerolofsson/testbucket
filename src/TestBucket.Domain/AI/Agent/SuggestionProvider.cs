using TestBucket.Domain.AI.Agent.Models;

namespace TestBucket.Domain.AI.Agent;
public class SuggestionProvider
{
    public static IReadOnlyList<Suggestion> GetSuggestions(AgentChatContext context)
    {
        string suggestTests = $"get-heuristics. Consider different test heuristics and the project and use a relevant heuristic to come up with some test ideas. Format the test case as markdown";
        string addTests = $"get-heuristics. Consider different test heuristics and the project and use a relevant heuristic to come up with some test ideas. For each test idea add-test-case with the step parameter as a string.";

        List<Suggestion> suggestions = [];
        foreach(var reference in context.References)
        {
            if (reference.EntityTypeName == "Requirement" ||
                reference.EntityTypeName == "Component" ||
                reference.EntityTypeName == "Feature")
            {
                if(reference.EntityTypeName == "Feature")
                {
                    suggestions.Add(new Suggestion($"Give me some test ideas for the feature: '{reference.Name}'", 
                        $"""
                        1. Call the search-features tool with \"{reference.Name}\" as argument to collect related requirements.
                        2. Call the get-heuristics tool to collect test heuristics for general test ideas. Consider them in combination with the feature description and requirements.
                        3. Based on the feature description and for each requirement and relevant hueristic, think about how to test the feature and describe them as test cases in markdown format.
                        """));
                }
                else
                {
                    suggestions.Add(new Suggestion($"Give me some test ideas for '{reference.Name}'", suggestTests));
                }

                if (reference.EntityTypeName == "Feature")
                {
                    suggestions.Add(new Suggestion($"Add tests for the feature: '{reference.Name}'",
                        $"""
                        1. Call the search-features tool with \"{reference.Name}\" as argument to collect related requirements.
                        2. Call the get-heuristics tool to collect test heuristics for general test ideas. Consider them in combination with the feature description and requirements.
                        3. Based on the feature description and for each requirement and relevant hueristic, think about how to test the feature and describe them as test cases in markdown format. Call the add-test-case tool for each test.
                        """));
                }
                else
                {
                    suggestions.Add(new Suggestion($"Add some tests for '{reference.Name}'", addTests));
                }
            }

            if (reference.EntityTypeName == "TestCase")
            {
                suggestions.Add(new Suggestion($"Run '{reference.Name}' with AI", $"""
                    Run the steps described in the reference description for '{reference.Name}' one by one. 
                    Use relevant tools as needed. 
                    If a step fails, output FAILED: <DescriptiveErrorMessage>
                    """));
                    

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
                suggestions.Add(new Suggestion("Summarize this issue", $"""
                    1. Search for the feature {reference.Name} using the search-features tool.
                    2. Summarize the feature: {reference.Name} considering both the feature description and the related requirements.
                    """));
            }
        }

        if(suggestions.Count == 0)
        {
            suggestions.Add(new Suggestion("What is the next milestone", $"What is the next milestone?"));

            suggestions.Add(new Suggestion("Summarize all open issues", $"Summarize all open issues"));
        }

        return suggestions;
    }
}
