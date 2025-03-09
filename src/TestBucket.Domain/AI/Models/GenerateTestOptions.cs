using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.AI.Models;
public class GenerateTestOptions
{
    public GenerateTestResponseMode ResponseMode { get; set; } = GenerateTestResponseMode.JsonAsSystemPrompt;
    public required string UserPrompt { get; set; }

    /// <summary>
    /// Heuristic used to enrich the system prompt and generate tests matching the heuristic
    /// </summary>
    public Heuristic? Heuristic { get; set; }

    public int NumTests { get; set; } = 15;
}
