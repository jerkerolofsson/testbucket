
using Mediator;

using Microsoft.Extensions.Localization;

using TestBucket.Components.Code.Dialogs;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Code.Services.CommitFeatureMapping;
using TestBucket.Localization;

namespace TestBucket.Components.Code.Controllers;

/// <summary>
/// Blazor controller for architecture
/// </summary>
internal class ArchitectureController : TenantBaseService
{
    private readonly IArchitectureManager _manager;
    private readonly ICommitManager _commitManager;
    private readonly IMediator _mediator;
    private readonly IDialogService _dialogService;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly AppNavigationManager _appNavigationManager;

    public ArchitectureController(AuthenticationStateProvider authenticationStateProvider, IArchitectureManager manager, ICommitManager commitManager, IMediator mediator, IDialogService dialogService, IStringLocalizer<SharedStrings> loc, AppNavigationManager appNavigationManager) : base(authenticationStateProvider)
    {
        _manager = manager;
        _commitManager = commitManager;
        _mediator = mediator;
        _dialogService = dialogService;
        _loc = loc;
        _appNavigationManager = appNavigationManager;
    }

    #region Features

    public async Task AddFeatureAsync()
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Architecture, PermissionLevel.Write);

        var parameters = new DialogParameters<EditFeatureDialog>
        {
            { x => x.Feature, new Feature() { GlobPatterns = [], Name = "New feature" } }
        };
        var dialog = await _dialogService.ShowAsync<EditFeatureDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is Feature feature)
        {
            feature.TestProjectId ??= _appNavigationManager.State.SelectedProject?.Id;
            await _manager.AddFeatureAsync(principal,feature);
        }
    }
    public async Task SaveFeatureAsync(Feature feature)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Architecture, PermissionLevel.Write);

        feature.TestProjectId ??= _appNavigationManager.State.SelectedProject?.Id;
        await _manager.UpdateFeatureAsync(principal, feature);
    }
    
    public async Task AddCommitsToFeatureAsync(long projectId, string featureName, IEnumerable<Commit> commits)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Architecture, PermissionLevel.Write);

        var feature = await _manager.GetFeatureByNameAsync(principal,projectId,featureName);
        if (feature is not null)
        {
            foreach (var commit in commits)
            {
                await _mediator.Send(new MapCommitFilesToFeatureRequest(principal, commit, feature));
            }
        }
    }

    public async Task EditFeatureAsync(Feature feature)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Architecture, PermissionLevel.Write);


        var parameters = new DialogParameters<EditFeatureDialog>
        {
            { x => x.Feature, feature }
        };
        var dialog = await _dialogService.ShowAsync<EditFeatureDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is Feature editedFeature)
        {
            feature.TestProjectId ??= _appNavigationManager.State.SelectedProject?.Id;
            await _manager.UpdateFeatureAsync(principal, editedFeature);
        }
    }

    public async Task DeleteFeatureAsync(Feature feature)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Architecture, PermissionLevel.Delete);

        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc["yes"],
            NoText = _loc["no"],
            Title = _loc["confirm-delete-title"],
            MarkupMessage = new MarkupString(_loc["confirm-delete-message"])
        });
        if (result == true)
        {
            await _manager.DeleteFeatureAsync(principal, feature);
        }
    }

    public async Task<IReadOnlyList<Feature>> GetFeaturesAsync(long projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetFeaturesAsync(principal, projectId);
    }

    #endregion Features

    #region Components
    public async Task<IReadOnlyList<Component>> GetComponentsAsync(long projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetComponentsAsync(principal, projectId);
    }

    public async Task AddComponentAsync()
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Architecture, PermissionLevel.Write);

        var parameters = new DialogParameters<EditFeatureDialog>
        {
            { x => x.Feature, new Component() { GlobPatterns = [], Name = "New component" } }
        };
        var dialog = await _dialogService.ShowAsync<EditFeatureDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is Component component)
        {
            component.TestProjectId = _appNavigationManager.State.SelectedProject?.Id;
            await _manager.AddComponentAsync(principal, component);
        }
    }
    public async Task EditComponentAsync(Component component)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Architecture, PermissionLevel.Write);

        var parameters = new DialogParameters<EditFeatureDialog>
        {
            { x => x.Feature, component }
        };
        var dialog = await _dialogService.ShowAsync<EditFeatureDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is Component editedComponent)
        {
            component.TestProjectId ??= _appNavigationManager.State.SelectedProject?.Id;
            await _manager.UpdateComponentAsync(principal, editedComponent);
        }
    }

    public async Task DeleteComponentAsync(Component component)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Architecture, PermissionLevel.Delete);

        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc["yes"],
            NoText = _loc["no"],
            Title = _loc["confirm-delete-title"],
            MarkupMessage = new MarkupString(_loc["confirm-delete-message"])
        });
        if (result == true)
        {
            await _manager.DeleteComponentAsync(principal, component);
        }
    }

    public async Task<Component?> GetComponentByNameAsync(TestProject project, string name)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetComponentByNameAsync(principal, project.Id, name);
    }
    #endregion Components

    #region Systems
    public async Task<IReadOnlyList<ProductSystem>> GetSystemsAsync(long projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetSystemsAsync(principal, projectId);
    }

    public async Task AddSystemAsync()
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Architecture, PermissionLevel.Write);

        var parameters = new DialogParameters<EditFeatureDialog>
        {
            { x => x.Feature, new ProductSystem() { GlobPatterns = [], Name = "New system" } }
        };
        var dialog = await _dialogService.ShowAsync<EditFeatureDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is ProductSystem system)
        {
            system.TestProjectId = _appNavigationManager.State.SelectedProject?.Id;
            await _manager.AddSystemAsync(principal, system);
        }
    }
    public async Task EditSystemAsync(ProductSystem system)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Architecture, PermissionLevel.Write);

        var parameters = new DialogParameters<EditFeatureDialog>
        {
            { x => x.Feature, system }
        };
        var dialog = await _dialogService.ShowAsync<EditFeatureDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is ProductSystem editedSystem)
        {
            editedSystem.TestProjectId ??= _appNavigationManager.State.SelectedProject?.Id;
            await _manager.UpdateSystemAsync(principal, editedSystem);
        }
    }

    public async Task DeleteSystemAsync(ProductSystem system)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Architecture, PermissionLevel.Delete);

        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc["yes"],
            NoText = _loc["no"],
            Title = _loc["confirm-delete-title"],
            MarkupMessage = new MarkupString(_loc["confirm-delete-message"])
        });
        if (result == true)
        {
            await _manager.DeleteSystemAsync(principal, system);
        }
    }
    #endregion Systems

    #region Layers
    public async Task<IReadOnlyList<ArchitecturalLayer>> GetLayersAsync(long projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetLayersAsync(principal, projectId);
    }

    public async Task AddLayerAsync()
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Architecture, PermissionLevel.Write);

        var parameters = new DialogParameters<EditFeatureDialog>
        {
            { x => x.Feature, new ArchitecturalLayer() { GlobPatterns = [], Name = "New layer" } }
        };
        var dialog = await _dialogService.ShowAsync<EditFeatureDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is ArchitecturalLayer layer)
        {
            layer.TestProjectId ??= _appNavigationManager.State.SelectedProject?.Id;
            await _manager.AddLayerAsync(principal, layer);
        }
    }
    public async Task EditLayerAsync(ArchitecturalLayer component)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Architecture, PermissionLevel.Write);

        var parameters = new DialogParameters<EditFeatureDialog>
        {
            { x => x.Feature, component }
        };
        var dialog = await _dialogService.ShowAsync<EditFeatureDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is ArchitecturalLayer editedLayer)
        {
            editedLayer.TestProjectId ??= _appNavigationManager.State.SelectedProject?.Id;
            await _manager.UpdateLayerAsync(principal, editedLayer);
        }
    }

    public async Task DeleteLayerAsync(ArchitecturalLayer component)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Architecture, PermissionLevel.Delete);

        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc["yes"],
            NoText = _loc["no"],
            Title = _loc["confirm-delete-title"],
            MarkupMessage = new MarkupString(_loc["confirm-delete-message"])
        });
        if (result == true)
        {
            await _manager.DeleteLayerAsync(principal, component);
        }
    }

    #endregion Layers

    #region Model

    public async Task<ProjectArchitectureModel> GetProductArchitectureAsync(TestProject project)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetProductArchitectureAsync(principal, project);
    }
    public async Task ImportProductArchitectureAsync(TestProject project, ProjectArchitectureModel model)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Architecture, PermissionLevel.Write);

        await _manager.ImportProductArchitectureAsync(principal, project, model);
    }
    #endregion Model
}
