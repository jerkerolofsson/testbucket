using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Xunit;

namespace TestBucket.Traits.Core.UnitTests.Xunit.Fakes;

[ExcludeFromCodeCoverage]
internal class OperatingSystemCreator
{
    public static OperatingSystemInformation CreateIOS()
    {
        return new OperatingSystemInformation
        {
            IsMacOS = false,
            IsWindows = false,
            IsLinux = false,
            IsBrowser = false,
            IsMacCatalyst = false,
            IsIOS = true,
            IsAndroid = false,
        };
    }

    public static OperatingSystemInformation CreateMacOS()
    {
        return new OperatingSystemInformation
        {
            IsMacOS = true,
            IsWindows = false,
            IsLinux = false,
            IsBrowser = false,
            IsMacCatalyst = false,
            IsIOS = false,
            IsAndroid = false,
        };
    }
    public static OperatingSystemInformation CreateAndroid()
    {
        return new OperatingSystemInformation
        {
            IsMacOS = false,
            IsWindows = false,
            IsLinux = false,
            IsBrowser = false,
            IsMacCatalyst = false,
            IsIOS = false,
            IsAndroid = true,
        };
    }
    public static OperatingSystemInformation CreateMacCatalyst()
    {
        return new OperatingSystemInformation
        {
            IsMacOS = false,
            IsWindows = false,
            IsLinux = false,
            IsBrowser = false,
            IsMacCatalyst = true,
            IsIOS = false,
            IsAndroid = false,
        };
    }
    public static OperatingSystemInformation CreateBrowser()
    {
        return new OperatingSystemInformation
        {
            IsMacOS = false,
            IsWindows = false,
            IsLinux = false,
            IsBrowser = true,
            IsMacCatalyst = false,
            IsIOS = false,
            IsAndroid = false,
        };
    }
    public static OperatingSystemInformation CreateLinux()
    {
        return new OperatingSystemInformation
        {
            IsMacOS = false,
            IsWindows = false,
            IsLinux = true,
            IsBrowser = false,
            IsMacCatalyst = false,
            IsIOS = false,
            IsAndroid = false,
        };
    }
    public static OperatingSystemInformation CreateWindows()
    {
        return new OperatingSystemInformation
        {
            IsMacOS = false,
            IsWindows = true,
            IsLinux = false,
            IsBrowser = false,
            IsMacCatalyst = false,
            IsIOS = false,
            IsAndroid = false,
        };
    }
}
