using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;
using TestBucket.Github;
using TestBucket.Github.Issues;
using TestBucket.Github.Mcp;
using TestBucket.Integrations;

namespace Microsoft.Extensions.DependencyInjection;
public static class GithubExtensions
{
    public static IServiceCollection AddGitHubExtension(this IServiceCollection services)
    {
        services.AddSingleton<IExtension, GithubExtension>();
        services.AddSingleton<IExternalProjectDataSource, GithubProjectDataSource>();
        services.AddSingleton<IExternalPipelineRunner, GithubWorkflowRunner>();
        services.AddSingleton<IExternalCodeRepository, GithubRepository>();
        services.AddSingleton<IExternalIssueProvider, GithubIssues>();
        services.AddSingleton<IExternalMcpProvider, GithubMcpProvider>();
        return services;
    }
}
