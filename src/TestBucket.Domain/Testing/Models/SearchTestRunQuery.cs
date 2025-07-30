using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Testing.Models;
public class SearchTestRunQuery : SearchQuery
{
    public long? TestRunId { get; set; }
    public long? FolderId { get; set; }
    public bool? Archived { get; set; }
}
