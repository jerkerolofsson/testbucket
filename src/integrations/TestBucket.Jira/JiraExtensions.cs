using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;
using TestBucket.Jira;
using TestBucket.Jira.Issues;

namespace Microsoft.Extensions.DependencyInjection;
public static class JiraExtensions
{
    public static IServiceCollection AddJiraExtension (this IServiceCollection services)
    {
        services.AddSingleton<IExtension, JiraExtension>();
        services.AddTransient<IExternalIssueProvider, JiraIssues>();
        return services;
    }
}
