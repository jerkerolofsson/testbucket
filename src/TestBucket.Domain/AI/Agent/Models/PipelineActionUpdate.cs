using Microsoft.Extensions.AI;

namespace TestBucket.Domain.AI.Agent.Models;
public class PipelineActionUpdate : ChatResponseUpdate
{
    public required string ActionName { get; set; }
}
