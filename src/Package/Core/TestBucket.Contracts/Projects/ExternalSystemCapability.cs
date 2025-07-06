using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Integrations
{
    [Flags]
    public enum ExternalSystemCapability
    {
        None = 0,

        /// <summary>
        /// Extension can create a new pipeline
        /// Implemented by IExternalPipelineRunner
        /// </summary>
        CreatePipeline = 0x01,

        /// <summary>
        /// Extension cannot create a pipeline, but can read
        /// Implemented by IExternalPipelineRunner
        /// </summary>
        GetPipelines = 0x02,

        /// <summary>
        /// Extension can read issues
        /// Implemented by IExternalIssueProvider
        /// </summary>
        GetIssues               = 0x04,

        /// <summary>
        /// Extension can read, update and create issues
        /// Implemented by IExternalIssueProvider
        /// </summary>
        CreateIssues = 0x08,

        /// <summary>
        /// Extension can download artifacts after a pipeline job completes
        /// Implemented by IExternalPipelineRunner
        /// </summary>
        ReadPipelineArtifacts = 0x10,

        /// <summary>
        /// Extension can read releases. 
        /// Implemented by IProjectDataSource using TraitType.Release
        /// </summary>
        GetReleases = 0x20,

        /// <summary>
        /// Extension can read milestones. 
        /// Implemented by IProjectDataSource using TraitType.Milestone
        /// </summary>
        GetMilestones = 0x40,

        /// <summary>
        /// Extension can read requirements
        /// Implemented by IExternalRequirementProvider
        /// </summary>
        GetRequirements = 0x80,


        /// <summary>
        /// Extension can read code repo
        /// Implemented by IExternalCodeRepository
        /// </summary>
        ReadCodeRepository = 0x100,

        /// <summary>
        /// Extension can read labels. 
        /// Implemented by IProjectDataSource using TraitType.Label
        /// </summary>
        GetLabels = 0x200,
    }
}
