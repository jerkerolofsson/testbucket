using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Models;
public class InformSettings
{
    /// <summary>
    /// POST URL
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Auth header
    /// </summary>
    public string? AuthHeader { get; set; }
}
