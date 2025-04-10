using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Projects;

namespace TestBucket.Contracts.Integrations
{
    public record class ConfiguredExternalService<T>(ExternalSystemDto Config, T Service);
}
