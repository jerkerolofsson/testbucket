using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;
using TestBucket.Azure;

namespace Microsoft.Extensions.DependencyInjection;
public static class AzureExtensions
{
    public static IServiceCollection AddAzureExtension(this IServiceCollection services)
    {
        services.AddSingleton<IExtension, AzureExtension>();
        return services;
    }
}
