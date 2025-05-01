using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Requirements;
using TestBucket.Domain.Export.Handlers.Requirements;
using TestBucket.Domain.Export.Zip;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Requirements.Import.Strategies;
using TestBucket.Domain.Requirements.Mapping;
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

        public async Task<List<RequirementEntityDto>> ImportFileAsync(ClaimsPrincipal principal,  FileResource fileResource)
        {
            try
            {
                var extension = Path.GetExtension(fileResource.Name);
                if (extension == ".pdf")
                {
                    var spec = await ImportPdfAsync(principal,fileResource);
                    return [spec];
                }
                else if(extension == ".tbz")
                {
                    return await ImportBackupFileAsync(principal, fileResource);
                }
                else if (extension == ".md" || extension == ".txt")
                {
                    var spec = await ImportTextAsync(principal,  fileResource, fileResource.Name ?? "spec");
                    return [spec];
                }
                return [];
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to import requirement specification");
                throw;
            }
        }

        private async Task<List<RequirementEntityDto>> ImportBackupFileAsync(ClaimsPrincipal principal, FileResource fileResource)
        {
            var items = new List<RequirementEntityDto>();

            using var stream = new MemoryStream(fileResource.Data);
            var source = new ZipImporter(stream);

            foreach(var exportedEntity in source.ReadAll())
            {
                RequirementEntityDto? entity = await RequirementSerialization.DeserializeAsync(exportedEntity);
                if(entity is not null)
                {
                    if (entity is RequirementSpecificationDto specificationDto)
                    {
                        items.Add(specificationDto);
                    }
                    if (entity is RequirementDto requirementDto)
                    {
                        items.Add(requirementDto);
                    }
                }
            }
            return items;
        }


        private async Task<RequirementSpecificationDto> ImportTextAsync(ClaimsPrincipal principal, FileResource fileResource, string name)
        {
            var tenantId = principal.GetTenantIdOrThrow();
            var spec = new RequirementSpecificationDto
            {
                Name = name,
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


        private async Task<RequirementSpecificationDto> ImportPdfAsync(ClaimsPrincipal principal, FileResource fileResource)
        {
            var tenantId = principal.GetTenantIdOrThrow();
            var spec = new RequirementSpecificationDto
            {
                Name = fileResource.Name ?? "",
                Description = "",
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
                            if (section.Text.Trim().Length > 0)
                            {
                                var requirement = new Requirement()
                                {
                                    TenantId = "",
                                    Name = section.Heading,
                                    Path = string.Join('/', section.Path.Select(x => x.Replace("/", "_"))),
                                    Description = section.Text
                                };

                                ExtractMetadataFromDescription(requirement);

                                // In order to keep the mapping for test cases when re-importing, we prefer to have an external ID
                                if(string.IsNullOrEmpty(requirement.ExternalId))
                                {
                                    GenerateExternalId(requirement);
                                }

                                results.Add(requirement);
                            }
                        }
                    }
                });
            }

            return results;
        }

        private void GenerateExternalId(Requirement requirement)
        {
            var slug = new Slugify.SlugHelper().GenerateSlug(requirement.Name);
            requirement.ExternalId = slug;
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
