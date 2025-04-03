using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.ApiKeys
{
    public class ApiKeyAuthenticator : IApiKeyAuthenticator
    {
        private readonly IServiceProvider _serviceProvider;
        private GlobalSettings? _settings;

        public ApiKeyAuthenticator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private async Task EnsureInitializedAsync()
        {
            if(_settings is null)
            {
                await ReloadSettingsAsync();
            }
        }

        private async Task ReloadSettingsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var settingsProvider = scope.ServiceProvider.GetRequiredService<ISettingsProvider>();
            _settings = await settingsProvider.LoadGlobalSettingsAsync();
        }

        public async Task<ClaimsPrincipal?> AuthenticateAsync(string token)
        {
            await EnsureInitializedAsync();
            if (_settings?.SymmetricJwtKey is null || _settings?.JwtIssuer is null || _settings.JwtAudience is null)
            {
                await ReloadSettingsAsync();
            }
            if (_settings?.SymmetricJwtKey is null || _settings?.JwtIssuer is null || _settings.JwtAudience is null)
            {
                return null;
            }

            var symmetricKey = _settings.SymmetricJwtKey;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(symmetricKey));

            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = securityKey,
                ValidAudience = _settings.JwtAudience,
                ValidIssuer = _settings.JwtAudience,
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken? validatedToken = null;
            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return principal;
            }
            catch (SecurityTokenException)
            {
                return null;
            }
            catch
            {
                throw;
            }
        }
    }
}
