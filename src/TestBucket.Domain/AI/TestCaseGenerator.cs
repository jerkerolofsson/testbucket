using System.ComponentModel;

using Microsoft.Extensions.AI;

using TestBucket.Domain.AI.Models;
using TestBucket.Domain.Progress;
using TestBucket.Domain.Testing.Heuristics.Models;
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

    public IReadOnlyList<Heuristic> Heuristics => [];

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
        if(_options?.Heuristic?.Description is not null)
        {
            return _options.Heuristic.Description;
        }
        return "";
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

}
