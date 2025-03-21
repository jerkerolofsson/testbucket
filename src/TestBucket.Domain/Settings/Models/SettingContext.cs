using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Settings.Models;
public class SettingContext
{
    /// <summary>
    /// Current user
    /// </summary>
    public required ClaimsPrincipal Principal { get; set; }

    /// <summary>
    /// Current project
    /// </summary>
    public long? ProjectId { get; set; }
}
