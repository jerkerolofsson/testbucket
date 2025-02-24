using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Data.Testing.Models;
public class SearchTestQuery : SearchQuery
{
    public long? TestSuiteId { get; set; }
    public long? FolderId { get; set; }
}
