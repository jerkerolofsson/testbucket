using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Integrations
{
    public record class ConfiguredExternalService<T>(ExternalSystemDto Config, T Service);
}
