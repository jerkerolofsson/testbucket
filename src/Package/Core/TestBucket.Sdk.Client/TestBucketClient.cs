using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Sdk.Client;
public class TestBucketClient(HttpClient Client)
{
    /// <summary>
    /// Client to work with product architecture (systems,features,components,layers)
    /// </summary>
    public ArchitectureClient Architecture => new ArchitectureClient(Client);


    /// <summary>
    /// Client to work with teams
    /// </summary>
    public TeamClient Teams => new TeamClient(Client);

    /// <summary>
    /// Client to work with projects
    /// </summary>
    public ProjectClient Projects => new ProjectClient(Client);

    /// <summary>
    /// Client to work with custom fields
    /// </summary>
    public FieldsClient Fields => new FieldsClient(Client);

    /// <summary>
    /// Client to work with test repository (test suites, test cases)
    /// </summary>
    public TestRepositoryClient TestRepository => new TestRepositoryClient(Client);


    /// <summary>
    /// Client to work with test repository (test suites, test cases)
    /// </summary>
    public TestRunClient TestRuns => new TestRunClient(Client);
}
