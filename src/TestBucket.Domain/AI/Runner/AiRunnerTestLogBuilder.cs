using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;

namespace TestBucket.Domain.AI.Runner;
internal class AiRunnerTestLogBuilder
{
    internal static void Append(StringBuilder responseBuilder, ChatResponseUpdate update)
    {

        foreach (var content in update.Contents)
        {
            if (content is FunctionCallContent functionCallContent)
            {
                responseBuilder.AppendLine();
                responseBuilder.AppendLine($"#### Function Call: {functionCallContent.Name}");
                if (functionCallContent.Arguments is not null)
                {
                    foreach (var arg in functionCallContent.Arguments)
                    {
                        responseBuilder.AppendLine($"- {arg.Key}: {arg.Value}");
                    }
                }
            }
            else if (content is FunctionResultContent functionResult)
            {
                if (functionResult.RawRepresentation is not null)
                {
                    responseBuilder.AppendLine($"""
                            ```json
                            {JsonSerializer.Serialize(functionResult.RawRepresentation)}
                            ```
                            """);
                }
            }
            else if (content is TextContent textContent)
            {
                responseBuilder.Append(textContent.Text);
            }
            else
            {
                responseBuilder.Append(content.ToString());
            }
        }
    }
}
