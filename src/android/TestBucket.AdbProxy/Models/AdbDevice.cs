using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Models;
public record class AdbDevice
{
    /// <summary>
    /// Serial / Device ID
    /// </summary>
    public required string DeviceId { get; set; }

    /// <summary>
    /// Appium port
    /// </summary>
    public int AppiumPort { get; set; }

    /// <summary>
    /// Status (device, unauthorized) from "adb devices"
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// ADB connect URL
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Appium URL
    /// </summary>
    public string? AppiumUrl { get; set; }

    /// <summary>
    /// Information read from getprop
    /// </summary>
    public ModelInformation ModelInfo { get; set; } = new();

    /// <summary>
    /// Proxied port
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// Version information
    /// </summary>
    public AndroidVersion Version { get; set; } = new();

    /// <summary>
    /// Android API level
    /// </summary>
    public int ApiLevel { get; set; }

    /// <summary>
    /// Public IP or hostname
    /// </summary>
    public string? Hostname { get; set; }

    /// <summary>
    /// ro.product.build.id
    /// </summary>
    public string? SoftwareVersion { get; set; }
    public string? SocModel { get; internal set; }

    /// <summary>
    /// CPU architecture
    /// </summary>
    public string? CpuAbi { get; internal set; }

    /// <summary>
    /// CPU name/id
    /// </summary>
    public string? CpuName { get; internal set; }

    /// <summary>
    /// Build variant
    /// </summary>
    public string? SoftwareVariant { get; internal set; }
}
