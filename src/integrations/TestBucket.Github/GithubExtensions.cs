using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;
using TestBucket.Github;

namespace Microsoft.Extensions.DependencyInjection;
public static class GithubExtensions
{
    public static IServiceCollection AddGitHubIntegration(this IServiceCollection services)
    {
        services.AddSingleton<IProjectDataSource, GithubProjectDataSource>();
        return services;
    }
}
