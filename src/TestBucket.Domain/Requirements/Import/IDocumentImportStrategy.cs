using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Requirements.Import
{
    public interface IDocumentImportStrategy
    {
        Task ImportAsync(RequirementSpecification spec, FileResource fileResource);
    }
}
