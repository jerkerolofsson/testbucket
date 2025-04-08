using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.IdentityModel.Tokens;

namespace TestBucket.Domain.ApiKeys.Validation
{
    public class AccessTokenValidator()
    {
        public static ClaimsPrincipal ValidateToken(GlobalSettings settings, string token)
        {
            var symmetricKey = settings.SymmetricJwtKey ?? throw new UnauthorizedAccessException("Configuration error: Global settings is missing JWT symmetric key!");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(symmetricKey));

            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = securityKey,
                ValidAudience = settings.JwtAudience,
                ValidIssuer = settings.JwtAudience,
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken? validatedToken = null;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            return principal;
        }
    }
}
