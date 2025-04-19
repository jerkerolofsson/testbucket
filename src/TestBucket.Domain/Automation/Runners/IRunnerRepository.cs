using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Domain.Automation.Runners
{
    public interface IRunnerRepository
    {
        Task AddAsync(Runner runner);
        Task<IReadOnlyList<Runner>> GetAllAsync(string tenantId);
        Task<Runner?> GetByIdAsync(string id);
        Task UpdateAsync(Runner runner);
        Task DeleteAsync(Runner runner);
    }
}
