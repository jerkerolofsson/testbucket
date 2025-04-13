using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Integrations;

/// <summary>
/// Optional fields that are required
/// </summary>
[Flags]
public enum ExtensionFields
{
    AccessToken = 0x01,
    ApiKey      = 0x02,
    BaseUrl     = 0x04,
    ProjectId   = 0x08,

}
