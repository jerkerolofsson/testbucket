using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.TestAccounts.Models;
public class AccountTypes
{
    /// <summary>
    /// A wifi account
    /// </summary>
    public const string Wifi = "wifi";

    /// <summary>
    /// An email account
    /// </summary>
    public const string Email = "email";

    public readonly static string[] All = [Wifi, Email];
}
