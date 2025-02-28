using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Identity;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Tenants;
using TestBucket.Domain.Testing;

namespace Microsoft.Extensions.DependencyInjection;
public static class DomainServiceExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<ITextTestResultsImporter, TextImporter>();

        return services;
    }
}
