namespace TestBucket.Domain.ApiKeys.Validation
{
    public class PrincipalValidator(ClaimsPrincipal Principal)
    {
        public long? ThrowIfNoProjectId()
        {
            var id = GetProjectId();
            if(id is null)
            {
                throw new UnauthorizedAccessException("Claim is missing project id");
            }
            return id;
        }
        public long? GetTestSuiteId()
        {
            var principal = Principal;
            var claims = principal.Claims.Where(x => x.Type == "testsuite" && !string.IsNullOrEmpty(x.Value)).ToList();
            if (claims.Count == 0)
            {
                return null;
            }
            var projectIdString = claims.First().Value;
            if (long.TryParse(projectIdString, out var id))
            {
                return id;
            }
            return null;
        }
        public long? GetTestRunId()
        {
            var principal = Principal;
            var claims = Principal.Claims.Where(x => x.Type == "run" && !string.IsNullOrEmpty(x.Value)).ToList();
            if (claims.Count == 0)
            {
                return null;
            }
            var projectIdString = claims.First().Value;
            if (long.TryParse(projectIdString, out var id))
            {
                return id;
            }
            return null;
        }
        public long? GetProjectId()
        {
            var principal = Principal;
            var claims = Principal.Claims.Where(x => x.Type == "project" && !string.IsNullOrEmpty(x.Value)).ToList();
            if (claims.Count == 0)
            {
                return null;
            }
            var projectIdString = claims.First().Value;
            if (long.TryParse(projectIdString, out var projectId))
            {
                return projectId;
            }
            return null;
        }
        public string? GetTenantId()
        {
            var principal = Principal;
            var claims = Principal.Claims.Where(x => x.Type == "tenant" && !string.IsNullOrEmpty(x.Value)).ToList();
            if (claims.Count == 0)
            {
                return null;
            }
            return claims.First().Value;
        }
    }
}
