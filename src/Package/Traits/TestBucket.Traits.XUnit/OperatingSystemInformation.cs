using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Traits.Xunit;
internal class OperatingSystemInformation
{
    public bool IsMacOS { get; set; } = OperatingSystem.IsMacOS();
    public bool IsLinux { get; set; } = OperatingSystem.IsLinux();
    public bool IsAndroid { get; set; } = OperatingSystem.IsAndroid();
    public bool IsWindows { get; set; } = OperatingSystem.IsWindows();
    public bool IsBrowser { get; set; } = OperatingSystem.IsBrowser();
    public bool IsMacCatalyst { get; set; } = OperatingSystem.IsMacCatalyst();
    public bool IsIOS { get; set; } = OperatingSystem.IsIOS();

}
