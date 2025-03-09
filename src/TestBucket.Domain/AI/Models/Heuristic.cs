using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.AI.Models;

public class Heuristic
{
    /// <summary>
    /// Prompt embedded in the system prompt
    /// </summary>
    public required string Prompt { get; set; }

    /// <summary>
    /// Friendly name of the heuristic
    /// </summary>
    public required string Name { get; set; }
}
