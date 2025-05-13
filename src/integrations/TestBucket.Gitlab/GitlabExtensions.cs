using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;
using TestBucket.Github;
using TestBucket.Gitlab;
using TestBucket.Gitlab.Issues;

namespace Microsoft.Extensions.DependencyInjection;
public static class GitlabExtensions
{
    public static IServiceCollection AddGitlabExtension (this IServiceCollection services)
    {
        services.AddSingleton<IExtension, GitlabExtension>();
        services.AddSingleton<IExternalProjectDataSource, GitlabProjectDataSource>();
        services.AddSingleton<IExternalPipelineRunner, GitlabPipelineRunner>();
        services.AddSingleton<IExternalIssueProvider, GitlabIssueProvider>();
        services.AddSingleton<IExternalCodeRepository, GitlabRepository>();

        return services;
    }
}
