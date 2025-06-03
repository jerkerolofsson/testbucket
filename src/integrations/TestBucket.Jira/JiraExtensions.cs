using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;
using TestBucket.Jira;

namespace Microsoft.Extensions.DependencyInjection;
public static class JiraExtensions
{
    public static IServiceCollection AddJiraExtension (this IServiceCollection services)
    {
        services.AddSingleton<IExtension, JiraExtension>();

        return services;
    }
}
