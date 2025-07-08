using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Jira.Models;
internal class CreateIssueResponse
{
    public required string id { get; set; }
    public required string key { get; set; }
}
