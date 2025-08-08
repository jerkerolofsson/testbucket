using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Shared;
public record class AssignedReviewer
{
    /// <summary>
    /// User name
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    /// Assigned as role
    /// </summary>
    public required string Role { get; set; }

    /// <summary>
    /// How the user voted
    /// </summary>
    public int Vote { get; set; }
}
