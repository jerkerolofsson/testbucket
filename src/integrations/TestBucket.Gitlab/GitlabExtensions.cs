using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;
using TestBucket.Gitlab;

namespace Microsoft.Extensions.DependencyInjection;
public static class GitlabExtensions
{
    public static IServiceCollection AddGitLabIntegration(this IServiceCollection services)
    {
        services.AddSingleton<IProjectDataSource, GitlabProjectDataSource>();
        services.AddSingleton<IExternalPipelineRunner, GitlabPipelineRunner>();

        return services;
    }
}
