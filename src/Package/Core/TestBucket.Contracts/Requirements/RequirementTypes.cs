using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Requirements;

/// <summary>
/// Default requirement types
/// </summary>
public class RequirementTypes
{
    public static string[] AllTypes => [General, Story, Initiative, Epic, Standard, Regulatory];

    /// <summary>
    /// General requirement
    /// </summary>
    public const string General = "General";

    /// <summary>
    /// A regulatory requirement
    /// </summary>
    public const string Regulatory = "Regulatory";

    /// <summary>
    /// A requirement linked to a standard specification
    /// </summary>
    public const string Standard = "Standard";

    // Agile

    public const string Epic = "Epic";
    public const string Story = "Story";
    public const string Initiative = "Initiative";
}
