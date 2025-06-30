using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.AI.Agent.Models;

/// <summary>
/// This is an action performed by the pipeline such as fetching a reference. 
/// It updates the context that is passed to the LLM
/// </summary>
public class PipelineAction
{
    public required string Prompt { get; set; }
}
