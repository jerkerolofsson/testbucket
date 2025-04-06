using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.TestAccounts.Models;
public class WifiAccountSubTypes
{
    public const string Open = "open";

    public const string Wep = "wep";
    public const string WpaPsk = "wpa-psk";
    public const string Wpa2Psk = "wpa2-psk";
    public const string Wpa3Sae = "wpa3-sae";

    public const string Wpa3Owe = "wpa3-owe";

    public const string WpaEnterprise = "wpa";
    public const string Wpa2Enterprise = "wpa2";
    public const string Wpa3Enterprise = "wpa3";

    public static readonly string[] All = [Open, Wep, WpaPsk, Wpa2Psk, Wpa3Sae];

}
