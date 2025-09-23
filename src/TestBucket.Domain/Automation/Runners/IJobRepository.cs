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
        /// <summary>
        /// Adds a job
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        Task AddAsync(Job job);

        /// <summary>
        /// Returns a job from guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        Task<Job?> GetByGuidAsync(string guid);        

        /// <summary>
        /// Returns a job from the ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Updates a job
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        Task UpdateAsync(Job job);
    }
}
