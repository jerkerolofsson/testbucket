using System.Security.Claims;
using System.Text.RegularExpressions;

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

        public async Task<RequirementSpecification?> ImportFileAsync(ClaimsPrincipal principal, long? teamId, long? testProjectId, FileResource fileResource)
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

        public async Task<IReadOnlyList<Requirement>> ExtractRequirementsAsync(RequirementSpecification specification, CancellationToken cancellationToken)
        {
            List<Requirement> results = new();

            if (specification.Description is not null)
            {
                await Task.Run(() =>
                {
                    foreach (var section in Markdown.MarkdownSectionParser.ReadSections(specification.Description, cancellationToken))
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }

                        if (section.Heading is not null && section.Text is not null && section.Text.Length > 0)
                        {
                            var requirement = new Requirement()
                            {
                                TenantId = "",
                                Name = section.Heading,
                                Path = string.Join(',', section.Path),
                                Description = section.Text
                            };

                            ExtractMetadataFromDescription(requirement);

                            results.Add(requirement);
                        }
                    }
                });
            }

            return results;
        }

        private readonly Regex s_requirementIdInBrackes = new Regex(@"\[.*\]");
        private void ExtractMetadataFromDescription(Requirement requirement)
        {
            if (requirement.Description is null)
            {
                return;
            }
            var match = s_requirementIdInBrackes.Match(requirement.Description);
            if (match.Success && match.Groups.Count >= 1)
            {
                requirement.ExternalId = match.Groups[0].Value.TrimStart('[').TrimEnd(']'); 
            }
        }
    }
}
