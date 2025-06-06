﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Automation;
using TestBucket.Domain.Testing.Models;
using TestBucket.Formats;

namespace TestBucket.Domain.Automation.Runners.Models
{
    /// <summary>
    /// A test runner job for the internal Test Bucket Runner
    /// </summary>
    [Index(nameof(Guid))]
    [Index(nameof(Status), nameof(Priority))]
    public class Job : ProjectEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Guid
        /// </summary>
        public required string Guid { get; set; }

        /// <summary>
        /// Priority is used to prioritize jobs that are evaluated in the UI
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// job duration
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Status of the job
        /// </summary>
        public PipelineJobStatus Status { get; set; }

        /// <summary>
        /// Script to run
        /// </summary>
        public required string Script { get; set; }

        /// <summary>
        /// Language/Shell for runner
        /// </summary>
        public required string Language { get; set; }

        /// <summary>
        /// Environment variables will be set from the run/environment/tc etc..
        /// </summary>
        [Column(TypeName = "jsonb")]
        public required Dictionary<string,string> EnvironmentVariables { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Standard out
        /// </summary>
        public string? StdOut { get; set; }

        /// <summary>
        /// Standard err
        /// </summary>
        public string? StdErr { get; set; }

        /// <summary>
        /// Serialized test result (e.g. a JUnitXml)
        /// </summary>
        public string? Result { get; set; }

        /// <summary>
        /// Test result format (of Result)
        /// </summary>
        public TestResultFormat? Format { get; set; }

        /// <summary>
        /// Other artifacts added by the runner
        /// </summary>
        [Column(TypeName = "json")]
        public Dictionary<string, byte[]>? ArtifactContent { get; set; }

        // Navigation
        public long? TestRunId { get; set; }
        public TestRun? TestRun { get; set; }
        
    }
}
