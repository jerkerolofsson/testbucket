﻿
using System.Collections.Generic;
using System.Security.Claims;

using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Issues.Types;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Requirements.Types;
using TestBucket.Contracts.Testing.States;

namespace TestBucket.Domain.States;

public interface IStateService
{
    /// <summary>
    /// Returns list of all states that can be set for am issue
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<IssueState>> GetIssueStatesAsync(ClaimsPrincipal principal, long projectId);


    /// <summary>
    /// Returns list of all states that can be set for a requirement
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<RequirementState>> GetRequirementStatesAsync(ClaimsPrincipal principal, long projectId);


    /// <summary>
    /// Returns list of all states that can be set for a test case
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TestState>> GetTestCaseRunStatesAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Returns the final/completed state for a test
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<TestState> GetProjectFinalStateAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Returns the initial state for a test
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<TestState> GetProjectInitialStateAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Returns requirement types for the project
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<RequirementType>> GetRequirementTypesAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Returns issue types for the project
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<IssueType>> GetIssueTypesAsync(ClaimsPrincipal principal, long projectId);
}