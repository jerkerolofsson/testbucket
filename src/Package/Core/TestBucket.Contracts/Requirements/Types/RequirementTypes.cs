using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Requirements.Types;

/// <summary>
/// Default requirement types
/// </summary>
public class RequirementTypes
{
    public static string[] AllTypes => [General, Story, Initiative, Epic, Task, Standard, Regulatory];

    /// <summary>
    /// General requirement
    /// </summary>
    public const string General = nameof(MappedRequirementType.General);

    /// <summary>
    /// A regulatory requirement
    /// </summary>
    public const string Regulatory = nameof(MappedRequirementType.Regulatory);

    /// <summary>
    /// A requirement linked to a standard specification
    /// </summary>
    public const string Standard = nameof(MappedRequirementType.Standard);

    // Agile

    public const string Epic = nameof(MappedRequirementType.Epic);
    public const string Story = nameof(MappedRequirementType.Story);
    public const string Initiative = nameof(MappedRequirementType.Initiative);

    // Tasks

    public const string Task = nameof(MappedRequirementType.Task);


    public const string Other = nameof(MappedRequirementType.Other);
}
