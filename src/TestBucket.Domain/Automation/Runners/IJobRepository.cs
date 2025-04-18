using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Automation;
using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Domain.Automation.Runners
{
    public interface IJobRepository
    {
        Task AddAsync(Job job);
        Task<Job?> GetByGuidAsync(string guid);        
        Task<Job?> GetByIdAsync(long id);

        /// <summary>
        /// Returns one job that matches the tenantId, languages and proejctId.
        /// 
        /// If projectId is null, that means that the access token for the runner was only bound to a tenant and all projects
        /// are allowed.
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="projectId">If null, any project will be accepted.</param>
        /// <param name="languages">The job must match one of these languages4</param>
        /// <returns></returns>
        Task<Job?> GetOneAsync(string tenantId, long? projectId, PipelineJobStatus status, string[] languages);
        Task UpdateAsync(Job job);
    }
}
