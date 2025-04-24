using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;

using static System.Net.Mime.MediaTypeNames;

namespace TestBucket.Domain.Search.Models
{
    public class SearchResult
    {
        private readonly TestCase? _test;
        private readonly Requirement? _requirement;

        public TestCase? TestCase => _test;
        public Requirement? Requirement => _requirement;

        public SearchResult(TestCase test)
        {
            _test = test;
            Text = test.Name;
        }

        public SearchResult(Requirement requirement)
        {
            _requirement = requirement;
            Text = requirement.Name;
        }

        public string Text { get; }

        public override string ToString() => Text;
    }
}
