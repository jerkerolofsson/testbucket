using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Jira.Models;
public class CloudResource
{
    public required string id { get; set; }
    public required string url { get; set; }
    public required string name { get; set; }
    public required string[] scopes { get; set; }
    public string? avatarUrl { get; set; }
}
