using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.ApiKeys.Validation;
using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Domain.Automation.Runners.Registration
{
    public class UpdateRunnerWhenConnected : INotificationHandler<RunnerConnectedEvent>
    {
        private readonly IRunnerRepository _runnerRepository;

        public UpdateRunnerWhenConnected(IRunnerRepository runnerRepository)
        {
            _runnerRepository = runnerRepository;
        }

        public async ValueTask Handle(RunnerConnectedEvent notification, CancellationToken cancellationToken)
        {
            var principal = notification.Principal;
            var tenantId = principal.GetTenantIdOrThrow();
            var validator = new PrincipalValidator(principal);
            var projectId = validator.GetProjectId();

            Runner? runner = await _runnerRepository.GetByIdAsync(notification.Request.Id);

            if (runner is null)
            {
                runner = new Runner()
                {
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow,
                    CreatedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated"),
                    ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated"),
                    Id = notification.Request.Id,
                    Name = notification.Request.Name,
                    PublicBaseUrl = notification.Request.PublicBaseUrl,
                    Tags = notification.Request.Tags,
                    TestProjectId = projectId,
                    TenantId = tenantId,
                    Languages = notification.Request.Languages
                };

                await _runnerRepository.AddAsync(runner);
            }
            else
            {

                bool changed = false;
                if (runner.TenantId != tenantId)
                {
                    runner.TenantId = tenantId;
                    changed = true;
                }
                if (runner.TestProjectId != projectId)
                {
                    runner.TestProjectId = projectId;
                    changed = true;
                }
                if (runner.Name != notification.Request.Name)
                {
                    runner.Name = notification.Request.Name;
                    changed = true;
                }
                if (runner.Tags.Length != notification.Request.Tags.Length)
                {
                    runner.Tags = notification.Request.Tags;
                    changed = true;
                }
                else
                {
                    for (int i = 0; i < runner.Tags.Length; i++)
                    {
                        if (runner.Tags[i] != notification.Request.Tags[i])
                        {
                            runner.Tags = notification.Request.Tags;
                            changed = true;
                        }
                    }
                }
                if(runner.Languages is null)
                {
                    runner.Languages = [];
                    changed = true;
                }
                if (runner.Languages.Length != notification.Request.Languages.Length)
                {
                    runner.Languages = notification.Request.Languages;
                    changed = true;
                }
                else
                {
                    for (int i = 0; i < runner.Languages.Length; i++)
                    {
                        if (runner.Languages[i] != notification.Request.Languages[i])
                        {
                            runner.Languages = notification.Request.Languages;
                            changed = true;
                        }
                    }
                }

                if (runner.PublicBaseUrl != notification.Request.PublicBaseUrl)
                {
                    runner.PublicBaseUrl = notification.Request.PublicBaseUrl;
                    changed = true;
                }
                if (changed)
                {
                    runner.Modified = DateTime.UtcNow;
                    runner.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");
                    await _runnerRepository.UpdateAsync(runner);
                }
            }
        }
    }
}
