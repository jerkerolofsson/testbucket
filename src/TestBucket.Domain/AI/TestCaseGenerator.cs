using System.ComponentModel;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.AI;

using TestBucket.Domain.AI.Models;
using TestBucket.Domain.Progress;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestCases;

namespace TestBucket.Domain.AI;
internal class TestCaseGenerator : ITestCaseGenerator
{
    private readonly IChatClientFactory _chatClientFactory;
    private readonly IProgressManager _progressManager;
    private readonly ITestCaseManager _testCaseManager;

    /// <summary>
    /// Base prompt
    /// </summary>
    private readonly string _systemPrompt = """
        You are a technical test designer whose role is to design test cases that verify that the product works correctly.

        The user will provide a description of a feature, a function or a use-case that defines the scope of the test cases. 

        Find a heuristic by using a tool and use this to define the type of test to create.
        """;
    private GenerateTestOptions? _options;

    public bool Enabled => true;

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

    public TestCaseGenerator(IChatClientFactory chatClientFactory, IProgressManager progressManager, ITestCaseManager testCaseManager)
    {
        _chatClientFactory = chatClientFactory;
        _progressManager = progressManager;
        _testCaseManager = testCaseManager;
    }

    public Task GenerateTestsAsync(ClaimsPrincipal principal, GenerateTestOptions options)
    {
        return Task.Run(async () =>
        {
            var folder = options.Folder;

            await foreach (var llmTestCase in GetStreamingResponseAsync(options))
            {
                if (llmTestCase?.TestCaseName is not null)
                {
                    var description = llmTestCase.AsTestMarkup();

                    var testCase = new TestCase() { Name = llmTestCase.TestCaseName, Description = description.ToString(), TenantId = folder?.TenantId ?? "" };
                    if (folder is not null)
                    {
                        testCase.TestSuiteFolderId = folder.Id;
                        testCase.TestSuiteId = folder.TestSuiteId;
                    }
                    testCase.TestSuiteId = options.TestSuiteId;

                    await _testCaseManager.AddTestCaseAsync(principal, testCase);
                }
            }
        });
    }

    public async IAsyncEnumerable<GeneratedTest?> GetStreamingResponseAsync(GenerateTestOptions options)
    {
        string userPrompt = "Function Description: \n" + options.UserPrompt;
        _options = options;

        await using var progress = _progressManager.CreateProgressTask("Generating tests..");

        // Determine the response format
        string systemPrompt = _systemPrompt;

        var llm = await _chatClientFactory.CreateChatClientAsync(ModelType.TestGenerator);

        if (llm is not null)
        {
            var systemPromptMessage = new ChatMessage(ChatRole.System, systemPrompt);
            var chatMessage = new ChatMessage(ChatRole.User, userPrompt);
            List<ChatMessage> chatHistory = [systemPromptMessage, chatMessage];

            var generatedTest = await GenereateTestAsync(llm, chatHistory, progress);
            if (generatedTest?.TestCaseName is not null)
            {
                await progress.ReportStatusAsync("Generating test #1..", 0);

                yield return generatedTest;
            }

            for (int i = 1; i < options.NumTests; i++)
            {
                await progress.ReportStatusAsync($"Generating test #{i+1}..", (i*100.0/options.NumTests));
                chatHistory.Add(new ChatMessage(ChatRole.User, "Generate another test case with different parameters"));
                var generatedTest2 = await GenereateTestAsync(llm, chatHistory, progress);
                if (generatedTest2?.TestCaseName is not null)
                {
                    yield return generatedTest2;
                }
            }
        }
    }

    [Description("Gets the heuristic")]
    string GetHeuristic() 
    {
        if(_options?.Heuristic?.Prompt is not null)
        {
            return _options.Heuristic.Prompt;
        }
        return Heuristics.First().Prompt;
    }

    private async Task<GeneratedTest?> GenereateTestAsync(IChatClient llm, List<ChatMessage> chatHistory, ProgressTask progress, CancellationToken cancellationToken = default)
    {
        var chatOptions = new ChatOptions()
        {
            Tools = [AIFunctionFactory.Create(GetHeuristic)]
        };
        //llm.CompleteAsync<GeneratedTest>("");
        var response = await llm.GetResponseAsync<GeneratedTest>(chatHistory, chatOptions);

        if(response.TryGetResult(out var testCase))
        {
            return testCase;
        }
        return null;
    }

    //private static GeneratedTest? ParseTestFromResponse(string text)
    //{
    //    try
    //    {
    //        if (!string.IsNullOrWhiteSpace(text))
    //        {
    //            var jsonMarkdownStartMarker = "```json";

    //            // If the output is text, and not json, extract the json from within
    //            bool isTextWithJson = text.Contains(jsonMarkdownStartMarker);
    //            string json = text;
    //            if(isTextWithJson)
    //            {
    //                var p = text.IndexOf(jsonMarkdownStartMarker);
    //                p = text.IndexOf('{', p);
    //                if (p > 0)
    //                {
    //                    var p2 = text.IndexOf("```", p);
    //                    var len = p2 - p;
    //                    json = text.Substring(p, len);
    //                }

    //            }

    //            var generatedTest = JsonSerializer.Deserialize<GeneratedTest>(json);
    //            return generatedTest;
    //        }
    //    }
    //    catch (Exception ex)
    //    { 
    //    // todo, handle exception and show message
    //    }

    //    return null;
    //}
}
