using System;
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
        private GlobalSettings _globalSettings = new()
        {
            JwtIssuer = "testbucket",
            JwtAudience = "testbucket",
            SymmetricJwtKey = "01234567890123456789012345678901234567890123456789"
        };

        public Task<T?> GetDomainSettingsAsync<T>(string tenantId, long? projectId) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<GlobalSettings> LoadGlobalSettingsAsync()
        {
            return Task.FromResult(_globalSettings);
        }

        public Task SaveDomainSettingsAsync<T>(string tenantId, long? projectId, T setting) where T : class
        {
            throw new NotImplementedException();
        }

        public Task SaveGlobalSettingsAsync(GlobalSettings settings)
        {
            _globalSettings = settings;
            return Task.CompletedTask;
        }
    }
}
