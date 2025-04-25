using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Teams;
public class TeamDto
{
    /// <summary>
    /// Name of the project
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Project slug (when creating set to an empty string)
    /// </summary>
    public required string Slug { get; set; }

    public required string ShortName { get; set; }
}
