using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts;
public class SearchFolder
{
    public required string Name { get; set; }
    public required string Query { get; set; }
}
