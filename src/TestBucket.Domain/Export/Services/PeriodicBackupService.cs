using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Mediator;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using TestBucket.Domain.Export.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Tenants;

namespace TestBucket.Domain.Export.Services
{
    public class PeriodicBackupService : BackgroundService
    {
        public readonly IServiceProvider _serviceProvider;

        public PeriodicBackupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var tenantRepository = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
                var backupManager = scope.ServiceProvider.GetRequiredService<IBackupManager>();

                int offset = 0;
                int count = 20;
                var response = await tenantRepository.SearchAsync(new SearchQuery { Offset = offset, Count = count });
                while (response.Items.Length > 0)
                {
                    foreach (var tenant in response.Items)
                    {
                        var principal = Impersonation.Impersonate(tenant.Id);
                        await backupManager.CreateBackupAsync(principal, new ExportOptions
                        {
                            Destination = null, // auto
                            DestinationType = ExportDestinationType.Disk,
                            ExportFormat = ExportFormat.Zip
                        });
                    }

                    offset += count;
                    response = await tenantRepository.SearchAsync(new SearchQuery { Offset = offset, Count = count });
                }


                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
