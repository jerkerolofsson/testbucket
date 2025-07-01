using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Tests.EndToEndTests.Fixtures
{
    public record class BrowserTestContext
    {
        public required PlaywrightFixture Fixture { get; set; }
        public required IBrowser Browser { get; set; }
        public required IBrowserContext Context { get; set; }
    }
}
