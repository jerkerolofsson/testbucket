using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Files.Models;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Requirements.Import.Strategies;
using TestBucket.Domain.Files;
using Microsoft.Extensions.Logging;

namespace TestBucket.Domain.Requirements.Import
{
    internal class RequirementImporter : IRequirementImporter
    {
        private readonly ILogger<RequirementImporter> _logger;
        private readonly IRequirementRepository _requirementRepository;

        public RequirementImporter(ILogger<RequirementImporter> logger, IRequirementRepository requirementRepository)
        {
            _logger = logger;
            _requirementRepository = requirementRepository;
        }

        public async Task<RequirementSpecification?> ImportAsync(string tenantId, long testProjectId, FileResource fileResource)
        {
            try
            {
                var extension = Path.GetExtension(fileResource.Name);
                if (extension == ".pdf")
                {
                    var spec = await ImportPdfAsync(tenantId, testProjectId, fileResource);
                    spec.FileResourceId = fileResource.Id;

                    await _requirementRepository.AddRequirementSpecificationAsync(tenantId, spec);
                    return spec;
                }
                if (extension == ".md" || extension == ".txt")
                {
                    var spec = await ImportTextAsync(tenantId, testProjectId, fileResource, fileResource.Name ?? "spec");
                    spec.FileResourceId = fileResource.Id;

                    await _requirementRepository.AddRequirementSpecificationAsync(tenantId, spec);
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

        private async Task<RequirementSpecification> ImportTextAsync(string tenantId, long testProjectId, FileResource fileResource, string name)
        {
            var spec = new RequirementSpecification
            {
                Name = "",
                TenantId = tenantId,
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


        private async Task<RequirementSpecification> ImportPdfAsync(string tenantId, long testProjectId, FileResource fileResource)
        {
            var spec = new RequirementSpecification
            {
                Name = "",
                TenantId = tenantId,
                TestProjectId = testProjectId,
                Description = ""
            };

            await new PdfImporter().ImportAsync(spec, fileResource);
            return spec;
        }
    }
}
