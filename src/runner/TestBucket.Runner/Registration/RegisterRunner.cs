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
        private readonly ILogger<RegisterRunner> _logger;

        public RegisterRunner(TestBucketApiClient client, SettingsManager settingsManager, ILogger<RegisterRunner> logger)
        {
            _client = client;
            _settingsManager = settingsManager;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var settings = await _settingsManager.LoadSettingsAsync();
                    var request = new ConnectRequest
                    {
                        Id = settings.Id!,
                        Name = settings.Name!,
                        Tags = settings.Tags,
                        PublicBaseUrl = settings.PublicBaseUrl,
                        Languages = ["pwsh", "powershell", "bash", "cmd", "http", "python"]
                    };

                    if (settings.AccessToken is not null)
                    {
                        await _client.RegisterAsync(request, settings.AccessToken);
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error registering runner!");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
