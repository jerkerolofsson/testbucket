using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.AI;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Settings;
using TestBucket.Domain.States;
using TestBucket.Domain.Tenants;
using TestBucket.Domain.Testing;

namespace Microsoft.Extensions.DependencyInjection;
public static class DomainServiceExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IStateService, StateService>();
        services.AddScoped<ITextTestResultsImporter, TextImporter>();
        services.AddScoped<ITestCaseGenerator, TestCaseGenerator>();

        return services;
    }
}
