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
            Title = "mcp",
            Description = "mcp-description",
            RelativeUrl = "/{tenantId}/Settings/Categories/MCP",
            Keywords = "mcp model context protocol ai llm",
            Icon = TbIcons.Filled.ModelContextProtocol
        });
        services.AddSingleton(new SettingsLink
        {
            Title = "ai-usage",
            Description = "ai-usage-description",
            RelativeUrl = "/{tenantId}/Settings/AI/Usage",
            Keywords = "mcp ai llm usage billing money cost token",
            Icon = TbIcons.BoldDuoTone.AI
        });

        services.AddSingleton(new SettingsLink
        {
            Title = "integrations",
            Description = "integrations-description",
            RelativeUrl = "/{tenantId}/Settings/Projects/{projectId}/Integrations",
            Keywords = "github gitlab trello git extension integration",
            Icon = TbIcons.Filled.Extensions
        });

        services.AddSingleton(new SettingsLink
        {
            Title = "fields",
            Description = "fields-description",
            RelativeUrl = "/{tenantId}/Settings/Projects/{projectId}/Fields",
            Keywords = "custom fields field",
            Icon = TbIcons.BoldDuoTone.Field
        });


        services.AddSingleton(new SettingsLink
        {
            Title = "milestones",
            Description = "milestones-description",
            RelativeUrl = "/{tenantId}/Settings/Milestones",
            Keywords = "milestones",
            Icon = TbIcons.BoldDuoTone.Flag
        });

        services.AddSingleton(new SettingsLink
        {
            Title = "test-accounts",
            Description = "test-accounts-description",
            RelativeUrl = "/{tenantId}/Settings/Accounts",
            Keywords = "test accounts",
            Icon = TbIcons.BoldDuoTone.UserCircle
        });
        services.AddSingleton(new SettingsLink
        {
            Title = "test-environments",
            Description = "test-environments-description",
            RelativeUrl = "/{tenantId}/Settings/ManageEnvironments",
            Keywords = "test environment",
            Icon = TbIcons.BoldDuoTone.Leaf
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
