using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Formats.MicrosoftTrx
{
    public record class TrxTestEntry(string TestId, string ExecutionId, string TestListId);
}
