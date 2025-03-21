using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using TestBucket.Domain.AI.Models;

namespace TestBucket.Domain.AI;
internal class ChatResponseFormatting
{
    public static GeneratedTest Example = new GeneratedTest
        {
            TestCaseName = "Name of Test Case",
            TestSteps =
                [
                    new GeneratedTestStep { Action = "Description of first action", ExpectedResult = "Expected result of first action" },
                    new GeneratedTestStep { Action = "Description of second action", ExpectedResult = "Expected result of second action" },
                    new GeneratedTestStep { Action = "Description of third action", ExpectedResult = "Expected result of third action" }
                ]
    };

    public static string ExampleJson => JsonSerializer.Serialize(Example, new JsonSerializerOptions() { WriteIndented = true });
}
