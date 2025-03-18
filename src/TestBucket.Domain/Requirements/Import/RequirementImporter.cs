using System.Security.Claims;

using Microsoft.Extensions.Logging;

using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Requirements.Import.Strategies;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Requirements.Import
{
    internal class RequirementImporter : IRequirementImporter
    {
        private readonly ILogger<RequirementImporter> _logger;

        public RequirementImporter(ILogger<RequirementImporter> logger)
        {
            _logger = logger;
        }

        public async Task<RequirementSpecification?> ImportAsync(ClaimsPrincipal principal, long? teamId, long? testProjectId, FileResource fileResource)
        {
            try
            {
                var extension = Path.GetExtension(fileResource.Name);
                if (extension == ".pdf")
                {
                    var spec = await ImportPdfAsync(principal, teamId, testProjectId, fileResource);
                    spec.FileResourceId = fileResource.Id;
                    return spec;
                }
                if (extension == ".md" || extension == ".txt")
                {
                    var spec = await ImportTextAsync(principal, teamId, testProjectId, fileResource, fileResource.Name ?? "spec");
                    spec.FileResourceId = fileResource.Id;
                    return spec;
                }
                return null;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to import requirement specification");
                throw;
            }
        }

        private async Task<RequirementSpecification> ImportTextAsync(ClaimsPrincipal principal, long? teamId, long? testProjectId, FileResource fileResource, string name)
        {
            var tenantId = principal.GetTentantIdOrThrow();
            var spec = new RequirementSpecification
            {
                Name = name,
                TenantId = tenantId,
                TeamId = teamId,
                TestProjectId = testProjectId,
                Description = ""
            };

            if (name.ToLower().Contains("rfc"))
            {
                await new RfcImporter().ImportAsync(spec, fileResource);
            }
            else
            {
                await new PlainTextImporter().ImportAsync(spec, fileResource);
            }
            return spec;
        }


        private async Task<RequirementSpecification> ImportPdfAsync(ClaimsPrincipal principal, long? teamId, long? testProjectId, FileResource fileResource)
        {
            var tenantId = principal.GetTentantIdOrThrow();
            var spec = new RequirementSpecification
            {
                Name = fileResource.Name ?? "",
                TenantId = tenantId,
                TestProjectId = testProjectId,
                Description = "",
                TeamId = teamId,
            };

            await new PdfImporter().ImportAsync(spec, fileResource);
            return spec;
        }
    }
}
