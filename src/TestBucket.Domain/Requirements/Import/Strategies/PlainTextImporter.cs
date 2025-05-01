using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Requirements;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Projects.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Domain.Requirements.Import.Strategies
{
    class PlainTextImporter : IDocumentImportStrategy
    {
        public Task ImportAsync(RequirementSpecificationDto spec, FileResource fileResource)
        {
            var text = Encoding.UTF8.GetString(fileResource.Data);
            spec.Description = text;
            return Task.FromResult(spec);
        }
    }
}
