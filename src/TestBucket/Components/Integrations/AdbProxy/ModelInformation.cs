using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Components.Integrations.AdbProxy;
public class ModelInformation
{
    /// <summary>
    /// ro.product.model
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// ro.product.manufacturer
    /// </summary>
    public string? Manufacturer { get; set; }
}
