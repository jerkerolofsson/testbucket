using Microsoft.Extensions.AI;

using TestBucket.Embeddings;

namespace TestBucket.Formats.Csv;
public class HeaderTranslator
{
    private static readonly TestBucket.Embeddings.LocalEmbedder s_localEmbedder = new();

    private static readonly Dictionary<string, List<ReadOnlyMemory<float>>> s_embeddings = new();
    private static readonly string[] s_headers = new[]
    {
        "name",

        "suite",
        "tb-suite-slug",
        "id",
        "preconditions",
        "description",
        "postconditions",
        "steps",
        "expected-results",

        "category",

        "quality-characteristic",

        "created-by",

        "component",
        "feature",
        "category",
        "priority"
    };

    /// <summary>
    /// List of direct matching aliases for headers.
    /// If a match for the key is found it will be returned.
    /// 
    /// If no match is found, cosine similarity will be calculated based on the key in this dictionary and mapped to the value
    /// so non exact matching is supported
    /// </summary>
    private static readonly Dictionary<string,string> s_aliases = new()
    {
        { "creator", "created-by" },
        { "author", "created-by" },

        { "layer", "category" }, // qase
        { "test-category", "category" }, // qase

        { "q-char", "quality-characteristic" },

        { "prio", "priority" },
        { "test-prio", "priority" },
        { "test-priority", "priority" },

        { "pre-conditions", "preconditions" },
        { "post-conditions", "postconditions" },

        { "test-steps", "steps" },
        { "steps-actions", "steps" },

        { "steps-expected-result", "expected-results" },
        { "steps-result", "expected-results" },
        { "steps-results", "expected-results" },
        { "result", "expected-results" },
        { "results", "expected-results" },

        { "test-suite", "suite" },
        { "testsuite", "suite" },

        { "case-id", "id" },
        { "test-id", "id" },
        { "testcase-id", "id" },
        { "test-case-id", "id" },

        { "title", "name" },
        { "case", "name" },

        { "folder", "path" },
        { "section hierarchy", "path"}, // qase
    };

    private static async Task CreateEmbeddignsAsync()
    {
        if(s_embeddings.Count == 0)
        {
            var embeddings = await s_localEmbedder.GenerateAsync(s_headers);
            for(int i = 0; i<Math.Min(s_headers.Length, embeddings.Count); i++)
            {
                var header = s_headers[i];
                s_embeddings[header] = [embeddings[i].Vector];
            }

            foreach(var alias in s_aliases)
            {
                var aliasEmbedding = await s_localEmbedder.GenerateAsync(alias.Key);

                if(s_embeddings.TryGetValue(alias.Value, out var embeddingsList))
                {
                    embeddingsList.Add(aliasEmbedding.Vector);  
                }
                else
                {
                    s_embeddings[alias.Value] = [aliasEmbedding.Vector];
                }
            }

        }
    }

    public static async Task<string> TranslateHeaderAsync(string text)
    {
        var header = text;
        
        await CreateEmbeddignsAsync();

        // Example translation logic: convert to lowercase and replace spaces with underscores
        if (string.IsNullOrEmpty(header))
        {
            return header;
        }

        var headerId = header.ToLower().Replace(' ', '-').Replace('_', '-');

        if(s_aliases.TryGetValue(headerId, out var alias))
        {
            // If we have an alias for this header, use it
            headerId = alias;
            return headerId;
        }

        if (s_embeddings.TryGetValue(headerId, out var embedding))
        {
            // If we already have an embedding for this header, return it
            return headerId;
        }

        // Find closest embedding
        var headerEmbedding = await s_localEmbedder.GenerateAsync(text);
        if(headerEmbedding is not null)
        {
            double bestDistance = double.MinValue;
            string? bestHeader = null;
            foreach (var embeddingPair in s_embeddings)
            {
                foreach (var aliasEmbedding in embeddingPair.Value)
                {
                    var distance = CosineSimilarity.Calculate(headerEmbedding.Vector, aliasEmbedding);
                    if (distance > bestDistance && distance > 0.6)
                    {
                        bestDistance = distance;
                        bestHeader = embeddingPair.Key;
                    }
                }
            }

            if(bestHeader is not null)
            {
                headerId = bestHeader;
            }
        }

        return headerId;
    }
}
