using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.AI.Models;

[Flags]
public enum ModelCapability 
{
    None = 0,

    /// <summary>
    /// Supports classification
    /// </summary>
    Classification = 1,

    /// <summary>
    /// Supports calling tools
    /// </summary>
    Tools = 2,

    /// <summary>
    /// Supports thinking
    /// </summary>
    Thinking = 4,

    /// <summary>
    /// Embedding model
    /// </summary>
    Embedding = 8,
}
