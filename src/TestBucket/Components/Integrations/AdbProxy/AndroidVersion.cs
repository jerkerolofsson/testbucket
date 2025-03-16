using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TestBucket.Components.Integrations.AdbProxy;

public class AndroidVersion
{
    [JsonPropertyName("code_name")]
    public string? CodeName { get; set; }

    [JsonPropertyName("version_name")]
    public string? VersionName { get; set; }

    [JsonPropertyName("api_level")]
    public long ApiLevel { get; set; }
}
