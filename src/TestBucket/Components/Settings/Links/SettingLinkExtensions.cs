using Microsoft.Extensions.DependencyInjection.Extensions;

using TestBucket.Domain;
using TestBucket.Domain.Settings.Models;

namespace TestBucket.Components.Settings.Links;

public static class SettingLinkExtensions
{
    public static IServiceCollection AddSettingLinks(this IServiceCollection services)
    {
        services.AddSingleton(new SettingsLink
        {
            Title = "integrations",
            Description = "integrations-description",
            RelativeUrl = "/{tenantId}/Settings/Projects/{projectId}/Integrations",
            Keywords = "github gitlab trello git",
            Icon = MudBlazor.Icons.Material.Filled.SettingsSystemDaydream
        });

        services.AddSingleton(new SettingsLink
        {
            Title = "test-environments",
            Description = "test-environments-description",
            RelativeUrl = "/{tenantId}/Settings/ManageEnvironments",
            Keywords = "test-environment",
            Icon = TbIcons.Filled.Leaf
        });

        services.AddSingleton(new SettingsLink
        {
            Title = "api-keys",
            Description = "api-keys-description",
            RelativeUrl = "/{tenantId}/Settings/ManageApiKeys",
            Keywords = "tokens access api key",
            Icon = MudBlazor.Icons.Material.Filled.Security
        });

        return services;
    }
}
