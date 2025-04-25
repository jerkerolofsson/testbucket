using System.Security.Claims;
using TestBucket.Domain.Projects.Mapping;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Tenants;
using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Requirements;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Requirements.Specifications;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TestBucket.Domain.Identity;

namespace TestBucket.Domain.Requirements.RequirementExtensions;
internal class BackgroundExternalRequirementSynchronizer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundExternalRequirementSynchronizer> _logger;
    public BackgroundExternalRequirementSynchronizer(IServiceProvider serviceProvider, ILogger<BackgroundExternalRequirementSynchronizer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    private ClaimsPrincipal Impersonate(string? tenantId)
    {
        return Impersonation.Impersonate(tenantId);
    }
    private ClaimsPrincipal Impersonate(TestProject project) => Impersonate(project.TenantId);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(120));

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var tenantManager = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
            var requirementExtensionManager = scope.ServiceProvider.GetRequiredService<IRequirementExtensionManager>();
            var requirementManager = scope.ServiceProvider.GetRequiredService<IRequirementManager>();
            var projectManager = scope.ServiceProvider.GetRequiredService<IProjectManager>();

            try
            {
                var tenants = await tenantManager.SearchAsync(new SearchQuery() { Offset = 0, Count = 100 });
                foreach (var tenant in tenants.Items)
                {
                    await ProcessTenantAsync(requirementExtensionManager, requirementManager, projectManager, tenant, stoppingToken);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error processing tenant");
            }
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ProcessTenantAsync(IRequirementExtensionManager requirementExtensionManager, IRequirementManager requirementManager, IProjectManager projectManager, Tenant tenant, CancellationToken stoppingToken)
    {
        ClaimsPrincipal principal = Impersonate(tenant.Id);

        var extensions = requirementExtensionManager.ExternalProviders;

        // Loop over all projects
        var response = await projectManager.BrowseTestProjectsAsync(principal, 0, 100);
        foreach (var project in response.Items)
        {
            await ProcessProjectAsync(requirementManager, projectManager, principal, extensions, project, stoppingToken);
        }
    }

    private async Task ProcessProjectAsync(IRequirementManager requirementManager, IProjectManager projectManager, ClaimsPrincipal principal, IReadOnlyList<IExternalRequirementProvider> extensions, TestProject project, CancellationToken stoppingToken)
    {
        var configs = await projectManager.GetProjectIntegrationsAsync(principal, project.Id);
        foreach (var config in configs.Where(x => (x.SupportedCapabilities & ExternalSystemCapability.GetRequirements) == ExternalSystemCapability.GetRequirements))
        {
            var configDto = config.ToDto();
            var extension = extensions.Where(x => x.SystemName == config.Provider).FirstOrDefault();
            if (extension is not null)
            {
                try
                {
                    await ProcessExtensionAsync(requirementManager, principal, project, configDto, extension, stoppingToken);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error processing exension");
                }
            }
        }
    }

    private async Task ProcessExtensionAsync(IRequirementManager requirementManager, ClaimsPrincipal principal, TestProject project, ExternalSystemDto configDto, IExternalRequirementProvider extension, CancellationToken stoppingToken)
    {
        await foreach (var specification in extension.EnumSpecificationsAsync(configDto, stoppingToken))
        {
            RequirementSpecification dbo = await ImportAsync(principal, project, requirementManager, specification);

            await foreach (var requirement in extension.EnumRequirementsAsync(configDto, specification, stoppingToken))
            {
                await ImportAsync(principal, project, requirementManager, dbo, requirement);
            }
        }
    }

    private async Task ImportAsync(ClaimsPrincipal principal, TestProject project, IRequirementManager requirementManager, RequirementSpecification specification, RequirementEntityDto requirementOrFolder)
        {
        if (requirementOrFolder is RequirementDto requirement)
        {
            Debug.Assert(project.TenantId is not null, "Project tenant id is null");
            if (project.TenantId is null)
            {
                return;
            }

            if (requirement.ExternalId is null) return;
            if (requirement.Provider is null) return;

            FilterSpecification<Requirement>[] specifications = [
                new FilterByTenant<Requirement>(project.TenantId),
                    new FilterRequirementByExternalId(requirement.Provider, requirement.ExternalId)
                ];

            var searchResult = await requirementManager.SearchRequirementsAsync(principal, specifications, 0, 1);
            if (searchResult.Items.Length == 0)
            {
                // Add new specification
                await requirementManager.AddRequirementAsync(principal, new Requirement
                {
                    ExternalId = requirement.ExternalId,
                    ExternalProvider = requirement.Provider,

                    Name = requirement.Name,
                    Description = requirement.Description,
                    State = requirement.State,
                    ReadOnly = requirement.ReadOnly,
                    Path = requirement.Path ?? "",
                    RequirementSpecificationId = specification.Id,

                    TestProjectId = project.Id,
                    TenantId = project.TenantId,
                    TeamId = project.TeamId,
                });
            }
            else
            {
                // Update
                var dbo = searchResult.Items[0];
                dbo.Description = requirement.Description;
                dbo.State = requirement.State;
                dbo.PathIds = null;
                dbo.ReadOnly = requirement.ReadOnly;
                dbo.Name = requirement.Name;
                dbo.Path = requirement.Path ?? "";
                await requirementManager.UpdateRequirementAsync(principal, dbo);
            }
        }
    }

    private static async Task<RequirementSpecification> ImportAsync(ClaimsPrincipal principal, TestProject project, IRequirementManager requirementManager, RequirementSpecificationDto spec)
    {
        ArgumentNullException.ThrowIfNull(spec.ExternalId);
        ArgumentNullException.ThrowIfNull(spec.Provider);
        ArgumentNullException.ThrowIfNull(project.TenantId);

        FilterSpecification<RequirementSpecification>[] specifications = [
            new FilterByTenant<RequirementSpecification>(project.TenantId),
                new FilterRequirementSpecificationByExternalId(spec.Provider, spec.ExternalId)
            ];

        var searchResult = await requirementManager.SearchRequirementSpecificationsAsync(principal, specifications, 0, 1);
        if (searchResult.Items.Length == 0)
        {
            // Add new specification
            var dbo = new RequirementSpecification
            {
                Name = spec.Name,
                ExternalId = spec.ExternalId,
                ExternalProvider = spec.Provider,
                Description = spec.Description,
                TestProjectId = project.Id,
                TenantId = project.TenantId,
                TeamId = project.TeamId,
                ReadOnly = spec.ReadOnly,
            };
            await requirementManager.AddRequirementSpecificationAsync(principal, dbo);
            return dbo;
        }
        else
        {
            // Update
            var dbo = searchResult.Items[0];
            dbo.Description = spec.Description;
            dbo.Icon = spec.Icon;
            dbo.Name = spec.Name;
            await requirementManager.UpdateRequirementSpecificationAsync(principal, dbo);

            return dbo;
        }
    }
}
