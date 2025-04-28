using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Code.Yaml;

namespace TestBucket.Domain.Code.Services;
public interface IArchitectureManager
{
    Task<ProjectArchitectureModel> GetProductArchitectureAsync(ClaimsPrincipal principal, TestProject project);
    Task ImportProductArchitectureAsync(ClaimsPrincipal principal, TestProject project, ProjectArchitectureModel model);
}
