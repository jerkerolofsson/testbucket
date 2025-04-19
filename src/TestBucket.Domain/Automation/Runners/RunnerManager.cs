using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Domain.Automation.Runners;
internal class RunnerManager : IRunnerManager
{
    private readonly IRunnerRepository _repository;

    public RunnerManager(IRunnerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<Runner>> GetAllAsync(ClaimsPrincipal principal)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Runner, PermissionLevel.Read);
        return await _repository.GetAllAsync(principal.GetTenantIdOrThrow());
    }

    public async Task RemoveAsync(ClaimsPrincipal principal, Runner runner)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Runner, PermissionLevel.Delete);
        await _repository.DeleteAsync(runner);
    }
}
