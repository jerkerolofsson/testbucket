using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Octokit;
using TestBucket.Contracts.Integrations;

namespace TestBucket.Github;
public class GithubIntegrationBaseClient
{

    protected GitHubClient CreateClient(ExternalSystemDto system)
    {
        var tokenAuth = new Credentials(system.AccessToken);
        var client = new GitHubClient(new ProductHeaderValue("TestBucket"));
        client.Credentials = tokenAuth;
        return client;
    }

}
