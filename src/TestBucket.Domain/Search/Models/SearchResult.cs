using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Search.Models
{
    public class SearchResult
    {
        private readonly TestCase? _test;

        public TestCase? TestCase => _test;

        public SearchResult(TestCase test)
        {
            _test = test;
            Text = test.Name;
        }

        public string Text { get; }

        public override string ToString() => Text;
    }
}
