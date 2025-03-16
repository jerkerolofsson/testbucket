using System.Text;
using System.Text.Json;

using Microsoft.Extensions.AI;

using TestBucket.Domain.AI.Models;

namespace TestBucket.Domain.AI;
internal class TestCaseGenerator : ITestCaseGenerator
{
    private readonly IReadOnlyList<IChatClient> _chatClients;

    private readonly string _systemPrompt = """
        You are a technical test designer whos role is to create multiple test cases that verify that the product works correctly.

        A test has the following properties:
        - Name
        - Premise
        - TestSteps

        A TestStep has one or more actions which should be performed by the tester. A TestStep may also have an ExpectedResult.

        The user will provide a description of a feature, a function or a use-case that defines the scope of the test cases. 

        Start by defining the Premise of the test from the "description" provided by the user. 
        The Premise defines the intent of a test.

        After defining the premise, define the steps (TestSteps) a user needs to perform to evaluate the premise. 

        Finally, summarize the Premise and TestSteps into a short and suitable name for the test case.

        Please create a test case based on the Premise and the TestSteps and output them in JSON format. 
        """;

    public bool Enabled => _chatClients.Count > 0;

    public IReadOnlyList<Heuristic> Heuristics => 
        [
            new Heuristic ()
            { 
                Name = "Goldilocks", 
                Prompt = """
                Consider data entry fields and create test cases with data entries that are too big, too small or with entries that are not suitable for the input such as
                entering a string in a numeric field.

                For numbers, consider:
                - Consider testing numbers that may cross common integer boundaries, such as 256, 65536, 65537, 2147483648, 2147483649, 4294967296, 4294967297
                - Negative numbers
                - Zero
                - Different internationalization options where the thousands or ten-thousands separated comma (,), a space ( ) or a period (.).
                
                For floating point numbers, consider:
                - Different internationalization where the decimal character could be a comma (,) or a period (.).

                For strings, consider:
                - Special characters such as /*,.<>|\\()[]{};:`!@#$%^&* for text input fields (including username, email and password fields). These characters could be used in different combinations, and in combination with normal alpha numerical latin characters.
                - Different length of strings.
                - Accented characters (àáâãäåçèéêëìíîðñòôõöö, etc.) and emojis.
                - Non-latin scripts such as simplified chinese, traditional chinese, thai, arabic, cyrillic scripts.
                - SQL injection patterns.
                - HTML input
                """
            },

            new Heuristic ()
            {
                Name = "Time and Date",
                Prompt = """
                Consider various input to date and time fields or properties.
                Consider changing the globalization/internationalization options of the system used to test the product.

                - Date/time entry fields or presentation of date/time and different date formats, time formats (24h or am/pm).
                - Internationalization 
                - Daylight saving
                - Leap year (febuary 29), and invalid date inputs.
                
                For multiple dates, consider using different time zones if that can be selected.
                """
            },

            new Heuristic ()
            {
                Name = "Performance",
                Prompt = """
                - That the action completes in a suitable time frame
                - For actions that take a long time to execute, a loading indicator should be displayed
                """
            },


            new Heuristic ()
            {
                Name = "Resource Utilization",
                Prompt = """
                - That the action doesn't use an unreasonable amount of CPU
                - That the action doesn't use an unreasonable amount of GPU
                - That the action doesn't use an unreasonable amount of RAM
                - That the action doesn't use an unreasonable amount of storage space
                """
            },
        ];

    public TestCaseGenerator(IEnumerable<IChatClient> chatClients)
    {
        _chatClients = chatClients.ToList();
    }

    public async IAsyncEnumerable<LlmGeneratedTestCase?> GetStreamingResponseAsync(GenerateTestOptions options)
    {
        string userPrompt = "Function Description: \n" + options.UserPrompt;

        // Determine the response format
        string systemPrompt = _systemPrompt;
        ChatResponseFormat responseFormat = ChatResponseFormat.Text;
        if (options.ResponseMode == GenerateTestResponseMode.JsonResponse)
        {
            JsonElement rootElement = JsonDocument.Parse(ChatResponseFormatting.ExampleJson).RootElement;
            responseFormat = ChatResponseFormatJson.ForJsonSchema(rootElement, "test_case", "Test cases");
        }
        else
        {
            systemPrompt = _systemPrompt + $"\n\nEXAMPLE JSON OUTPUT:\n{ChatResponseFormatting.ExampleJson}\n\n";
        }


        if (options.Heuristic?.Prompt is not null)
        {
            var heuristicText = $"\n\nUse one or more of the following constraints to define a premise for testing:\n{options.Heuristic.Prompt}";
            userPrompt += heuristicText;
        }

        if (_chatClients.Count > 0)
        {
            var llm = _chatClients.First();

            var systemPromptMessage = new ChatMessage(ChatRole.System, systemPrompt);
            var chatMessage = new ChatMessage(ChatRole.User, userPrompt);
            List<ChatMessage> chatHistory = [systemPromptMessage, chatMessage];

            LlmGeneratedTestCase? generatedTest = await GenereateTestAsync(responseFormat, llm, chatHistory);
            if (generatedTest?.Name is not null)
            {
                yield return generatedTest;
            }

            for (int i = 1; i < options.NumTests; i++)
            {
                chatHistory.Add(new ChatMessage(ChatRole.User, "Generate another test case with a different Premise"));
                LlmGeneratedTestCase? generatedTest2 = await GenereateTestAsync(responseFormat, llm, chatHistory);
                if (generatedTest2?.Name is not null)
                {
                    yield return generatedTest2;
                }

            }

        }
    }

    private static async Task<LlmGeneratedTestCase?> GenereateTestAsync(ChatResponseFormat responseFormat, IChatClient llm, List<ChatMessage> chatHistory, CancellationToken cancellationToken = default)
    {
        //var response = await llm.GetResponseAsync(chatHistory, new ChatOptions { ResponseFormat = responseFormat });
        var sb = new StringBuilder();
        await foreach (var stream in llm.GetStreamingResponseAsync(chatHistory, new ChatOptions { ResponseFormat = responseFormat }, cancellationToken))
        {
            sb.Append(stream.Text);
        }
        LlmGeneratedTestCase? generatedTest = ParseTestFromResponse(sb.ToString());
        return generatedTest;
    }

    private static LlmGeneratedTestCase? ParseTestFromResponse(string text)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                var jsonMarkdownStartMarker = "```json";

                // If the output is text, and not json, extract the json from within
                bool isTextWithJson = text.Contains(jsonMarkdownStartMarker);
                string json = text;
                if(isTextWithJson)
                {
                    var p = text.IndexOf(jsonMarkdownStartMarker);
                    p = text.IndexOf('{', p);
                    if (p > 0)
                    {
                        var p2 = text.IndexOf("```", p);
                        var len = p2 - p;
                        json = text.Substring(p, len);
                    }

                }

                var generatedTest = JsonSerializer.Deserialize<LlmGeneratedTestCase>(json);
                return generatedTest;
            }
        }
        catch (Exception ex)
        { 
        // todo, handle exception and show message
        }

        return null;
    }
}
