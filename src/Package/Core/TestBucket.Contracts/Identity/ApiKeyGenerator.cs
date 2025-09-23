using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace TestBucket.Contracts.Identity
{
    public class ApiKeyGenerator
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;

        public ApiKeyGenerator(string key, string issuer, string audience)
        {
            _key = key;
            _issuer = issuer;
            _audience = audience;
        }

        public string GenerateAccessToken(string scope, ClaimsPrincipal principal, DateTime expires)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var symmetricKey = _key;
            var claims = principal.Claims;
            List<Claim> allClaims = [

                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("scope", scope),
                ..claims.Where(x => x.Type is "project" or "tenant" or ClaimTypes.Name or ClaimTypes.Email or "permissions")
            ];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(symmetricKey));
            securityKey.KeyId = _issuer + "-0";

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(allClaims),
                Expires = expires,
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public string GenerateAccessToken(ClaimsPrincipal principal, DateTime expires)
        {
            return GenerateAccessToken(principal, null, expires);
        }
        public string GenerateAccessToken(ClaimsPrincipal principal, long? projectId, DateTime expires)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var symmetricKey = _key;
            var claims = principal.Claims;
            List<Claim> allClaims = [

                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                ..claims.Where(x =>
                                    x.Type != JwtRegisteredClaimNames.Iss &&
                                    x.Type != JwtRegisteredClaimNames.Aud &&
                                    x.Type != JwtRegisteredClaimNames.Jti)
            ];
            if(projectId is not null)
            {
                allClaims.Add(new Claim("project", projectId.Value.ToString()));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(symmetricKey));
            securityKey.KeyId = _issuer + "-0";

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(allClaims),
                Expires = expires,
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
