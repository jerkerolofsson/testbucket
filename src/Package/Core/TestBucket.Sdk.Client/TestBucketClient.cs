using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Sdk.Client;
public class TestBucketClient(HttpClient Client)
{
    /// <summary>
    /// Client to work with teams
    /// </summary>
    public TeamClient Teams => new TeamClient(Client);


    /// <summary>
    /// Client to work with test repository (test suites, test cases)
    /// </summary>
    public TestRepositoryClient TestRepository => new TestRepositoryClient(Client);
}
