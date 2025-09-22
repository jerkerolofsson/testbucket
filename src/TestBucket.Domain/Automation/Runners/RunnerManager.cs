using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http.HttpResults;

using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Domain.Automation.Runners;
internal class RunnerManager : IRunnerManager
{
    private readonly IRunnerRepository _repository;
    private readonly TimeProvider _timeProvider;

    public RunnerManager(IRunnerRepository repository, TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task AddAsync(ClaimsPrincipal principal, Runner runner)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Runner, PermissionLevel.Write);
        runner.TenantId = principal.GetTenantIdOrThrow();
        runner.Created = runner.Modified = _timeProvider.GetUtcNow();
        var name = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");
        runner.CreatedBy = runner.ModifiedBy = name;

        await _repository.AddAsync(runner);
    }

    public async Task<IReadOnlyList<Runner>> GetAllAsync(ClaimsPrincipal principal)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Runner, PermissionLevel.Read);
        return await _repository.GetAllAsync(principal.GetTenantIdOrThrow());
    }

    public Task<Runner?> GetByIdAsync(ClaimsPrincipal principal, string id)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Runner, PermissionLevel.Read);

        return _repository.GetByIdAsync(principal.GetTenantIdOrThrow(), id);
    }

    public async Task RemoveAsync(ClaimsPrincipal principal, Runner runner)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Runner, PermissionLevel.Delete);
        await _repository.DeleteAsync(runner);
    }

    public async Task UpdateAsync(ClaimsPrincipal principal, Runner runner)
    {
        principal.ThrowIfEntityTenantIsDifferent(runner);
        principal.ThrowIfNoPermission(PermissionEntityType.Runner, PermissionLevel.Write);
        runner.Modified = _timeProvider.GetUtcNow();
        runner.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");

        await _repository.UpdateAsync(runner);
    }
}
