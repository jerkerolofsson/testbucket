using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Runners.Models;
using TestBucket.Runner.Settings;

namespace TestBucket.Runner.Registration
{
    /// <summary>
    /// Registers the runner on the remote host
    /// </summary>
    public class RegisterRunner : BackgroundService
    {
        private readonly TestBucketApiClient _client;
        private readonly SettingsManager _settingsManager;

        public RegisterRunner(TestBucketApiClient client, SettingsManager settingsManager)
        {
            _client = client;
            _settingsManager = settingsManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                var settings = await _settingsManager.LoadSettingsAsync();
                var request = new ConnectRequest
                {
                    Id = settings.Id!,
                    Name = settings.Name!,
                    Tags = settings.Tags,
                    PublicBaseUrl = settings.PublicBaseUrl,
                    Languages = ["pwsh", "powershell", "bash", "cmd", "http"]
                };

                if (settings.AccessToken is not null)
                {
                    await _client.RegisterAsync(request, settings.AccessToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
