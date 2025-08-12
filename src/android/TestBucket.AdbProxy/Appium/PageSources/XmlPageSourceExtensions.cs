using TestBucket.Embeddings;

namespace TestBucket.AdbProxy.Appium.PageSources;
public static class XmlPageSourceExtensions
{
    private static readonly LocalEmbedder _localEmbedder = new();
    private const double COSINE_SIMILARITY_MIN_DISTANCE = 0.6;

    public static List<MatchedHierarchyNode> Flatten(this XmlPageSource xmlPageSource)
    {
        List<MatchedHierarchyNode> matches = [];
        Flatten(matches, xmlPageSource.Nodes);
        return matches;
    }

    internal static async Task<List<MatchedHierarchyNode>> FindElementsAsync(this XmlPageSource xmlPageSource, string query, Predicate<HierarchyNode> predicate)
    {
        List<MatchedHierarchyNode> matches = [];
        await FindElementStrategyAsync(xmlPageSource, query, matches, predicate);

        if (matches.Count == 0)
        {
            // Remove some keywords that may be added by the AI
            query = CleanQuery(query);
            if (!string.IsNullOrEmpty(query))
            {
                await FindElementStrategyAsync(xmlPageSource, query, matches, predicate);
            }
        }

        // Sort to return the best match first
        matches.Sort((a, b) => b.MatchScore - a.MatchScore);

        return matches;
    }

    private static async Task FindElementStrategyAsync(this XmlPageSource xmlPageSource, string query, List<MatchedHierarchyNode> matches, Predicate<HierarchyNode> predicate)
    {
        await FindElementRecursiveAsync(query, matches, xmlPageSource.Nodes, predicate, ElementMatching.MatchText);
        if (matches.Count == 0)
        {
            await FindElementRecursiveAsync(query, matches, xmlPageSource.Nodes, predicate, ElementMatching.MatchContentDescription);
        }
        if (matches.Count == 0)
        {
            await FindElementRecursiveAsync(query, matches, xmlPageSource.Nodes, predicate, ElementMatching.MatchId);
        }
        if (matches.Count == 0)
        {
            await FindElementRecursiveAsync(query, matches, xmlPageSource.Nodes, predicate, ElementMatching.MatchText | ElementMatching.UseEmbeddings);
        }
        if (matches.Count == 0)
        {
            await FindElementRecursiveAsync(query, matches, xmlPageSource.Nodes, predicate, ElementMatching.MatchContentDescription | ElementMatching.UseEmbeddings);
        }
        if (matches.Count == 0)
        {
            await FindElementRecursiveAsync(query, matches, xmlPageSource.Nodes, predicate, ElementMatching.MatchId | ElementMatching.UseEmbeddings);
        }
    }

    internal static string CleanQuery(string query)
    {
        query = query.ToLower();
        if (query.Contains(' '))
        {
            string[] keywordsToRemove = ["button", "field", "box", "text", "input", "element", "control", "widget", "toggle", "switch"];
            foreach (var keyword in keywordsToRemove)
            {
                query = query.Replace(keyword, "");
            }
        }
        query = query.Trim();
        return query;
    }

    private static void Flatten(List<MatchedHierarchyNode> matches, List<HierarchyNode> nodes)
    {
        foreach (var node in nodes)
        {
            matches.Add(new MatchedHierarchyNode(NodeMatchType.ExactText, node));
            Flatten(matches, node.Nodes);
        }
    }

    private static async Task FindElementRecursiveAsync(string query, List<MatchedHierarchyNode> matches, List<HierarchyNode> nodes, Predicate<HierarchyNode> predicate, ElementMatching matching)
    {
        ReadOnlyMemory<float>? queryEmbedding = null;
        ReadOnlyMemory<float>? embedding = null;

        foreach (var node in nodes)
        {
            if (predicate(node))
            {
                if (string.IsNullOrEmpty(query))
                {
                    matches.Add(new MatchedHierarchyNode(NodeMatchType.ExactText, node));
                }
                else
                {
                    bool match = false;
                    if (!string.IsNullOrEmpty(node.ResourceId) && !match && (matching & ElementMatching.MatchId) == ElementMatching.MatchId)
                    {
                        if (node.ResourceId == query)
                        {
                            match = true;
                            matches.Add(new MatchedHierarchyNode(NodeMatchType.ExactId, node));
                        }
                        else if (node.ResourceId.Contains(query, StringComparison.InvariantCultureIgnoreCase))
                        {
                            match = true;
                            matches.Add(new MatchedHierarchyNode(NodeMatchType.PartialId, node));
                        }
                        else if ((matching & ElementMatching.UseEmbeddings) == ElementMatching.UseEmbeddings)
                        {
                            if(node.ResourceId.Contains("album_"))
                            {

                            }

                            embedding = (await _localEmbedder.GenerateEmbeddingsAsync([node.ResourceId])).FirstOrDefault();
                            if(queryEmbedding is null)
                            {
                                queryEmbedding = (await _localEmbedder.GenerateEmbeddingsAsync([query])).FirstOrDefault();
                            }
                            if(embedding != null && queryEmbedding != null)
                            {
                                var distance = CosineSimilarity.Calculate(embedding.Value, queryEmbedding.Value);
                                if (distance > COSINE_SIMILARITY_MIN_DISTANCE)
                                {
                                    matches.Add(new MatchedHierarchyNode(NodeMatchType.EmbeddingId, node) { EmbeddingDistance = distance });
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(node.ContentDescription) && !match && (matching & ElementMatching.MatchContentDescription) == ElementMatching.MatchContentDescription)
                    {
                        if (node.ContentDescription == query)
                        {
                            match = true;
                            matches.Add(new MatchedHierarchyNode(NodeMatchType.ExactContentDescription, node));
                        }
                        else if (node.ContentDescription.Contains(query, StringComparison.InvariantCultureIgnoreCase))
                        {
                            match = true;
                            matches.Add(new MatchedHierarchyNode(NodeMatchType.PartialContentDescription, node));
                        }
                        else if ((matching & ElementMatching.UseEmbeddings) == ElementMatching.UseEmbeddings)
                        {
                            embedding = (await _localEmbedder.GenerateEmbeddingsAsync([node.ContentDescription])).FirstOrDefault();
                            if (queryEmbedding is null)
                            {
                                queryEmbedding = (await _localEmbedder.GenerateEmbeddingsAsync([query])).FirstOrDefault();
                            }
                            if (embedding != null && queryEmbedding != null)
                            {
                                var distance = CosineSimilarity.Calculate(embedding.Value, queryEmbedding.Value);
                                if (distance > COSINE_SIMILARITY_MIN_DISTANCE)
                                {
                                    matches.Add(new MatchedHierarchyNode(NodeMatchType.EmbeddingContentDescription, node) { EmbeddingDistance = distance });
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(node.Text) && !match && (matching & ElementMatching.MatchText) == ElementMatching.MatchText)
                    {
                        if (node.Text == query)
                        {
                            match = true;
                            matches.Add(new MatchedHierarchyNode(NodeMatchType.ExactText, node));
                        }
                        else if (node.Text.Contains(query, StringComparison.InvariantCultureIgnoreCase))
                        {
                            match = true;
                            matches.Add(new MatchedHierarchyNode(NodeMatchType.PartialText, node));
                        }
                        else if((matching&ElementMatching.UseEmbeddings) == ElementMatching.UseEmbeddings)
                        {
                            embedding = (await _localEmbedder.GenerateEmbeddingsAsync([node.Text])).FirstOrDefault();
                            if (queryEmbedding is null)
                            {
                                queryEmbedding = (await _localEmbedder.GenerateEmbeddingsAsync([query])).FirstOrDefault();
                            }
                            if (embedding != null && queryEmbedding != null)
                            {
                                var distance = CosineSimilarity.Calculate(embedding.Value, queryEmbedding.Value);
                                if (distance > COSINE_SIMILARITY_MIN_DISTANCE)
                                {
                                    matches.Add(new MatchedHierarchyNode(NodeMatchType.EmbeddingText, node) { EmbeddingDistance = distance });
                                }
                            }
                        }
                    }
                }
            }

            await FindElementRecursiveAsync(query, matches, node.Nodes, predicate, matching);
        }
    }
}
