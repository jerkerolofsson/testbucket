using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Testing.Models
{
    public class CompilerError
    {
        public required int Column { get; set; }
        public required int Line { get; set; }
        public required int Code { get; set; }
        public required string Message { get; set; }
    }
}
