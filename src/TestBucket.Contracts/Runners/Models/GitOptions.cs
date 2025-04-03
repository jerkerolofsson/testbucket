using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Runners.Models
{
    public class GitOptions
    {
        /// <summary>
        /// URL where the repository will be pulled from, e.g.
        /// https://username:accesstoken@github.com/jerkerolofsson/testbucket
        /// </summary>
        public string? RepositoryUrl { get; set; }
    }
}
