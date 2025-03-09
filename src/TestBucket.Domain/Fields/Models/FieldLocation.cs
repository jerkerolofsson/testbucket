using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Fields.Models;
public class FieldLocation
{
    /// <summary>
    /// Location on the screen
    /// </summary>
    public FieldAnchor Anchor { get; set; } = FieldAnchor.Below;

    /// <summary>
    /// Order number within the anchor position
    /// </summary>
    public int OrderNumber { get; set; }

    // Navigation

    public long? ScreenId { get; set; }

}
