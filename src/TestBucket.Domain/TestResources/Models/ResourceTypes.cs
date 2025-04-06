using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.TestResources.Models;
public class ResourceTypes
{
    // Sensors
    public const string Sensor = "sensor";
    public const string Accelerometer = "accelerometer";
    public const string Gyro = "gyro";

    // Computing

    public const string Computer = "computer";

    // HID

    public const string GameController = "game-controller";
    public const string Keyboard = "keyboard";
    public const string Mouse = "mouse";

    // Networking
    public const string NetworkAdapter = "network-adapter";
    public const string NetworkSwitch = "network-switch";
    public const string Router = "router";

    // Devices

    public const string Phone = "phone";
    public const string Tablet = "tablet";

    // Gaming
    public const string GamingConsole = "gaming-console";

    // Camera

    public const string Camera = "camera";
    public const string VideoCamera = "video-camera";

    // Automotive
    public const string VehicleEcu = "vehicle-ecu";
    public const string VehicleInfotainmentSystem = "vehicle-ivi";
    public const string CanDevice = "can-device";

    // GNSS

    public const string GnssReceiver = "gnss-receiver";
    public const string GnssSimulator = "gnss-simulator";

    // Wi-Fi

    public const string WifiAccessPoint = "wifi-ap";
    public const string WifiStation = "wifi-sta";

}
