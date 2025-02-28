using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Settings.Models;
public class GlobalSettings
{
    public long Id { get; set; }

    /// <summary>
    /// The default tenant when the user logs in
    /// </summary>
    public string DefaultTenant { get; set; } = "default";

    /// <summary>
    /// Keep track of the changes
    /// </summary>
    public int Revision { get; set; }
}
