using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.AI.Models;

/// <summary>
/// Options that can be provided by the user to generate AI tests
/// </summary>
public class GenerateTestOptions
{
    public GenerateTestResponseMode ResponseMode { get; set; } = GenerateTestResponseMode.JsonAsSystemPrompt;
    public required string UserPrompt { get; set; }

    public required long TestSuiteId { get; set; }

    /// <summary>
    /// Folder where tests will be added
    /// </summary>
    public TestSuiteFolder? Folder { get; set; }

    /// <summary>
    /// Heuristic used to enrich the system prompt and generate tests matching the heuristic
    /// </summary>
    public Heuristic? Heuristic { get; set; }

    public int NumTests { get; set; } = 15;
}
