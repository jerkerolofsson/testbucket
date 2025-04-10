using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;
using TestBucket.Trello;


namespace Microsoft.Extensions.DependencyInjection;
public static class TrelloExtensions
{
    public static IServiceCollection AddTrelloExtension(this IServiceCollection services)
    {
        services.AddSingleton<IExtension, TrelloExtension>();
        return services;
    }
}
