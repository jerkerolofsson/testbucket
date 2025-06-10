using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using ModelContextProtocol.Protocol;

using TestBucket.Domain.Progress;
using TestBucket.Domain.Teams.Models;
using TestBucket.Domain.Testing.Mapping;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Domain.Testing.TestSuites;
using TestBucket.Formats;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Testing.Services.Import;

public record class ImportTestsRequest(ClaimsPrincipal Principal, Team Team, TestProject Project, TestRepositoryDto Repo, ImportHandlingOptions Options) : IRequest;

public class ImportTestsHandler : IRequestHandler<ImportTestsRequest>
{
    private readonly IProgressManager _progressManager;
    private readonly ITestSuiteManager _suiteManager;
    private readonly ITestCaseManager _testCaseManager;

    public ImportTestsHandler(IProgressManager progressManager, ITestSuiteManager suiteManager, ITestCaseManager testCaseManager)
    {
        _progressManager = progressManager;
        _suiteManager = suiteManager;
        _testCaseManager = testCaseManager;
    }

    public async ValueTask<Unit> Handle(ImportTestsRequest request, CancellationToken cancellationToken)
    {
        await using var task = _progressManager.CreateProgressTask("Importing..");

        var principal = request.Principal;

        Dictionary<string, TestSuite> suitesByName = [];
        Dictionary<string, TestSuite> suitesBySlug = [];
        foreach (var suiteDto in request.Repo.TestSuites)
        {
            TestSuite suite = await GetOrCreateSuiteAsync(request, suiteDto);
            suitesByName[suite.Name] = suite;
            if (suite.Slug is not null)
            {
                suitesBySlug[suite.Slug] = suite;
            }
        }

        foreach (var item in request.Repo.TestCases.Index())
        {
            var testDto = item.Item;
            await task.ReportStatusAsync($"Importing {testDto.TestCaseName}", item.Index * 100.0 / request.Repo.TestCases.Count);

            TestSuite? suite = null;

            if (!string.IsNullOrEmpty(testDto.TestSuiteSlug) && suitesBySlug.TryGetValue(testDto.TestSuiteSlug, out var foundSuiteBySlug))
            {
                suite = foundSuiteBySlug;
            }
            else if (!string.IsNullOrEmpty(testDto.TestSuiteName) && suitesByName.TryGetValue(testDto.TestSuiteName, out var foundSuiteByName))
            {
                suite = foundSuiteByName;
            }

            if(suite is null)
            {
                // If no suite information is found in the imported file, create one
                suite = new TestSuite { Name = "New Suite " + DateTime.Now.ToString("yyyy-MM-dd") };
                suite.TestProjectId = request.Project.Id;
                suite.TeamId = request.Team.Id;
                await _suiteManager.AddTestSuiteAsync(principal, suite);

            }

            await UpdateOrCreateTestAsync(request, suite, testDto);
        }

        return Unit.Value;
    }

    private async Task UpdateOrCreateTestAsync(ImportTestsRequest request, TestSuite suite, TestCaseDto testDto)
    {
        var principal = request.Principal;
        TestCase? testCase = null;

        if (!string.IsNullOrEmpty(testDto.Slug))
        {
            testCase = await _testCaseManager.GetTestCaseBySlugAsync(principal, request.Project.Id, testDto.Slug);
        }
        if (testCase is null && !string.IsNullOrEmpty(testDto.TestCaseName))
        {
            testCase = await _testCaseManager.GetTestCaseByNameAsync(principal, request.Project.Id, suite.Id, testDto.TestCaseName);
        }

        if (testCase is null)
        {
            testCase = TestCaseMapping.ToDbo(testDto);
            testCase.TestSuiteId = suite.Id;
            testCase.TestProjectId = request.Project.Id;
            testCase.TeamId = request.Team.Id;
            await _testCaseManager.AddTestCaseAsync(principal, testCase);
        }
        else
        {
            bool updated = false;
            if (testCase.Name != testDto.TestCaseName)
            {
                testCase.Name = testDto.TestCaseName;
                updated = true;
            }
            // Update Description if changed
            if (testCase.Description != testDto.Description)
            {
                testCase.Description = testDto.Description;
                updated = true;
            }
            // Update ExecutionType if changed
            if (testCase.ExecutionType != testDto.ExecutionType)
            {
                testCase.ExecutionType = testDto.ExecutionType;
                updated = true;
            }

            if (updated)
            { 
                await _testCaseManager.SaveTestCaseAsync(principal, testCase);
            }
        }
    }

    private async Task<TestSuite> GetOrCreateSuiteAsync(ImportTestsRequest request, TestSuiteDto suiteDto)
    {
        var principal = request.Principal;

        TestSuite? suite = null;
        if(!string.IsNullOrEmpty(suiteDto.Slug))
        {
            suite = await _suiteManager.GetTestSuiteBySlugAsync(principal, request.Project.Id, suiteDto.Slug);
        }
        if (suite is null)
        {
            suite = await _suiteManager.GetTestSuiteByNameAsync(principal, request.Team.Id, request.Project.Id, suiteDto.Name);
        }

        if (suite is null)
        {
            suite = TestSuiteMapper.ToDbo(suiteDto);
            suite.TestProjectId = request.Project.Id;
            suite.TeamId = request.Team.Id;
            await _suiteManager.AddTestSuiteAsync(principal, suite);
        }
        return suite;
    }
}