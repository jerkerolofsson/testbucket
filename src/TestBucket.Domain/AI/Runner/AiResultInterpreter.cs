using Mediator;

using TestBucket.Domain.AI.Embeddings;
using TestBucket.Embeddings;

namespace TestBucket.Domain.AI.Runner;

/// <summary>
/// Provides methods to interpret AI-generated markdown results and extract test outcomes or paragraphs.
/// </summary>
public class AiResultInterpreter
{
    private readonly IMediator _mediator;
    private static readonly Dictionary<TestResult, string[]> _resultKeywords = new()
    {
        { TestResult.Passed, new[] { "PASSED", "SUCCESS", "COMPLETED", "no problems" } },
        { TestResult.Failed, new[] { "FAILED", "ERROR", "UNSUCCESSFUL", "could not find" } },
    };
    private static readonly Dictionary<TestResult, List<ReadOnlyMemory<float>>> _resultVectors = [];

    public AiResultInterpreter(IMediator mediator)
    {
        _mediator = mediator;
    }

    private async Task InitializeResultVectorsAsync(ClaimsPrincipal principal, long projectId)
    {
        if(_resultVectors.Count == 0)
        {
            foreach(var pair in _resultKeywords)
            {
                _resultVectors[pair.Key] = [];
                foreach(var text in pair.Value)
                {
                    var response = await _mediator.Send(new GenerateEmbeddingRequest(principal, projectId, text));
                    if (response.EmbeddingVector.HasValue)
                    {
                        _resultVectors[pair.Key].Add(response.EmbeddingVector.Value);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Interprets a markdown log response and determines the test result.
    /// </summary>
    /// <param name="responseMarkdown">The markdown string containing the test result.</param>
    /// <returns>The interpreted <see cref="TestResult"/>.</returns>
    public async Task<TestResult> InterpretAiRunnerAsync(ClaimsPrincipal principal, long projectId, string responseMarkdown)
    {
        // Our prompt instructs the LLM to return a markdown string with PASSED or FAILED, but that
        // is not accurate enough.
        if (responseMarkdown.Contains("PASSED"))
        {
            return TestResult.Passed;
        }
        else if (responseMarkdown.Contains("FAILED"))
        {
            return TestResult.Failed;
        }
        else
        {
            // Create embeddings from keywords that indicate a passed or a failed result
            await InitializeResultVectorsAsync(principal, projectId);

            var paragraphs = GetMarkdownParagraphs(responseMarkdown);
            if (paragraphs.Count > 0)
            {
                string text;
                if (paragraphs.Count == 1)
                {
                    // If there is only one paragraph, use it directly
                    text = paragraphs[0];
                }
                else
                {
                    // Otherwise, use the last two paragraphs as the result
                    // We don't want to use the complete text as there may be indications of success eariler on..
                    text = $"""
                        {paragraphs[^2]}
                        {paragraphs[^1]}
                        """;
                }

                var llmEmbedding = await _mediator.Send(new GenerateEmbeddingRequest(principal, projectId, text)); 
                if(llmEmbedding.EmbeddingVector.HasValue)
                {
                    var queryVector = llmEmbedding.EmbeddingVector.Value.ToArray();
                    TestResult bestResult = TestResult.Inconclusive;
                    double bestSimilarity = double.MinValue;
                    foreach (var pair in _resultVectors)
                    {
                        foreach (var vector in pair.Value)
                        {
                            var similarity = CosineSimilarity.Calculate(queryVector, vector.ToArray());
                            if (similarity > bestSimilarity)
                            {
                                bestSimilarity = similarity;
                                bestResult = pair.Key;
                            }
                        }
                    }
                    return bestResult;
                }
            }
        }

        return TestResult.Inconclusive;
    }

    /// <summary>
    /// Splits a markdown string into logical paragraphs, handling code blocks, lists, and blockquotes.
    /// </summary>
    /// <param name="markdown">The markdown string to split.</param>
    /// <returns>A list of paragraphs as strings.</returns>
    internal static List<string> GetMarkdownParagraphs(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return new List<string>();

        var paragraphs = new List<string>();
        var lines = markdown.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        var current = new List<string>();
        bool inCodeBlock = false;
        bool inListBlock = false;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmed = line.Trim();
            bool isList = trimmed.StartsWith("- ") || trimmed.StartsWith("* ") || System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"^\d+\. ");
            bool isCodeBlockDelimiter = trimmed.StartsWith("```");
            bool isBlockQuote = trimmed.StartsWith("> ");
            bool isEmpty = string.IsNullOrWhiteSpace(line);

            // Code block start/end
            if (isCodeBlockDelimiter)
            {
                inCodeBlock = !inCodeBlock;
                current.Add(line);
                // If code block ends, flush paragraph
                if (!inCodeBlock)
                {
                    paragraphs.Add(string.Join("\n", current).Trim());
                    current.Clear();
                }
                continue;
            }

            if (inCodeBlock)
            {
                current.Add(line);
                continue;
            }

            // List block handling
            if (isList)
            {
                if (!inListBlock && current.Count > 0)
                {
                    paragraphs.Add(string.Join("\n", current).Trim());
                    current.Clear();
                }
                inListBlock = true;
                current.Add(line);
                // If next line is not a list, flush
                bool nextIsList = false;
                if (i + 1 < lines.Length)
                {
                    var nextTrimmed = lines[i + 1].Trim();
                    nextIsList = nextTrimmed.StartsWith("- ") || nextTrimmed.StartsWith("* ") || System.Text.RegularExpressions.Regex.IsMatch(nextTrimmed, @"^\d+\. ");
                }
                if (!nextIsList)
                {
                    paragraphs.Add(string.Join("\n", current).Trim());
                    current.Clear();
                    inListBlock = false;
                }
                continue;
            }
            else if (inListBlock)
            {
                // End of list block
                if (current.Count > 0)
                {
                    paragraphs.Add(string.Join("\n", current).Trim());
                    current.Clear();
                }
                inListBlock = false;
            }

            // Blockquote as its own paragraph
            if (isBlockQuote)
            {
                if (current.Count > 0)
                {
                    paragraphs.Add(string.Join("\n", current).Trim());
                    current.Clear();
                }
                paragraphs.Add(line.Trim());
                continue;
            }

            // Empty line = paragraph boundary
            if (isEmpty)
            {
                if (current.Count > 0)
                {
                    paragraphs.Add(string.Join("\n", current).Trim());
                    current.Clear();
                }
                continue;
            }

            current.Add(line);
        }
        if (current.Count > 0)
        {
            paragraphs.Add(string.Join("\n", current).Trim());
        }
        return paragraphs.Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
    }
}
