using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.AI.Models;
public record class GeneratedTest
{
    public string? TestCaseName { get; set; }
    public string? TestDescription { get; set; }

    /// <summary>
    /// Generated test steps
    /// </summary>
    public List<GeneratedTestStep> TestSteps { get; set; } = [];

    public string AsTestMarkup()
    {
        var description = new StringBuilder();
        description.AppendLine("# Description");
        description.AppendLine(TestDescription);
        description.AppendLine();
        description.AppendLine("# Steps");
        description.AppendLine("| Action                           | Expected Result                  |");
        description.AppendLine("| -------------------------------- | -------------------------------- |");
        foreach (var testStep in TestSteps.Where(x => x.Action is not null))
        {
            description.Append("| " + testStep.Action!.PadRight(32, ' ') + " | ");
            if (!string.IsNullOrEmpty(testStep.ExpectedResult))
            {
                description.Append(testStep.ExpectedResult.PadRight(32, ' ') + " |");
            }
            else
            {
                description.Append("".PadRight(32, ' ') + " |");
            }
            description.AppendLine();
        }
        description.AppendLine();
        return description.ToString();
    }
}
