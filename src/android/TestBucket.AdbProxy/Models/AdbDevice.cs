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
    public int ApiLevel { get; internal set; }
    public string Hostname { get; internal set; }
}
