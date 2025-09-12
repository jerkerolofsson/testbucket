using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Settings.Fakes
{
    /// <summary>
    /// Test double
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class FakeSettingsProvider : ISettingsProvider
    {
        private readonly ConcurrentDictionary<string, object> _domainSettings = new();

        private GlobalSettings _globalSettings = new()
        {
            JwtIssuer = "testbucket",
            JwtAudience = "testbucket",
            SymmetricJwtKey = "01234567890123456789012345678901234567890123456789"
        };


        public Task SaveDomainSettingsAsync<T>(string tenantId, long? projectId, T setting) where T : class
        {
            string key = CreateKey(tenantId, projectId, typeof(T).FullName);
            _domainSettings[key] = setting ?? throw new ArgumentNullException(nameof(setting));
            return Task.CompletedTask;
        }


        public Task<T?> GetDomainSettingsAsync<T>(string tenantId, long? projectId) where T : class
        {
            string key = CreateKey(tenantId, projectId, typeof(T).FullName);
            T? settings = null;

            if(_domainSettings.TryGetValue(key, out var storedSettings))
            {
                settings = storedSettings as T;
            }

            settings ??= default(T);
            return Task.FromResult(settings);
        }

        private string CreateKey(string tenantId, long? projectId, string? fullName)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(tenantId);
            if(projectId.HasValue)
            {
                stringBuilder.Append($":{projectId.Value}");
            }
            if(fullName != null)
            {
                stringBuilder.Append($":{fullName}");
            }
            return stringBuilder.ToString();
        }

        public Task<GlobalSettings> LoadGlobalSettingsAsync()
        {
            return Task.FromResult(_globalSettings);
        }

        public Task SaveGlobalSettingsAsync(GlobalSettings settings)
        {
            _globalSettings = settings;
            return Task.CompletedTask;
        }
    }
}
