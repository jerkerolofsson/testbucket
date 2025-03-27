using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.ApiTests;
using TestBucket.Contracts.Integrations;

namespace Microsoft.Extensions.DependencyInjection;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDotHttpApiTestExtension(this IServiceCollection services)
    {
        services.AddTransient<IMarkdownTestRunner, DotHttpMarkdownTestRunner>();
        return services;
    }
}
