using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.AI.Models;
using TestBucket.Domain.AI.Services.Classifier;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCases;

namespace TestBucket.Domain.Testing.Services.Classification
{
    /// <summary>
    /// Classifies empty test case fields
    /// </summary>
    public class BackgroundClassificationService : BackgroundService
    {
        private readonly ILogger<BackgroundClassificationService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public BackgroundClassificationService(
            ILogger<BackgroundClassificationService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        private ClaimsPrincipal Impersonate(TestCase testCase, string username)
        {
            return Impersonation.Impersonate(builder =>
            {
                builder.UserName = username;
                builder.Email = username;
                builder.TenantId = testCase.TenantId;
                builder.Add(PermissionEntityType.Project, PermissionLevel.ReadWrite);
                builder.Add(PermissionEntityType.Team, PermissionLevel.Read);
                builder.Add(PermissionEntityType.TestCase, PermissionLevel.ReadWrite);
                builder.Add(PermissionEntityType.Issue, PermissionLevel.ReadWrite);
                builder.Add(PermissionEntityType.Requirement, PermissionLevel.ReadWrite);
                builder.Add(PermissionEntityType.Architecture, PermissionLevel.Read);
            });
        }

        private async Task AssignGeneratedFieldsAsync(IServiceScope scope, TestCase testCase)
        {
            if (testCase.Description is null)
            {
                return;
            }
            var classifier = scope.ServiceProvider.GetRequiredService<IClassifier>();
            var username = await classifier.GetModelNameAsync(ModelType.Classification) ?? "ai-classifier";
            var principal = Impersonate(testCase, username);
            var fieldDefinitionManager = scope.ServiceProvider.GetRequiredService<IFieldDefinitionManager>();
            var fieldManager = scope.ServiceProvider.GetRequiredService<IFieldManager>();

            var definitions = await fieldDefinitionManager.GetDefinitionsAsync(principal, testCase.TestProjectId, FieldTarget.TestCase);
            var fields = await fieldManager.GetTestCaseFieldsAsync(principal, testCase.Id, definitions);
            foreach (var field in fields)
            {
                var hasValue = field.HasValue();
                if (!hasValue &&  field.FieldDefinition?.UseClassifier == true)
                {
                    var options = await fieldDefinitionManager.GetOptionsAsync(principal, field.FieldDefinition);
                    if (options.Count == 1)
                    {
                        field.StringValue = options[0];
                        field.Inherited = false;
                        await fieldManager.UpsertTestCaseFieldAsync(principal, field);
                    }
                    else if (options.Count >= 2)
                    {
                        var result = await classifier.ClassifyAsync(principal, field.FieldDefinition.Name, options.ToArray(), testCase);
                        if (result.Length > 0)
                        {
                            _logger.LogInformation("Classified {test} with {category}", testCase.Name, result[0]);
                            if (FieldValueConverter.TryAssignValue(field.FieldDefinition, field, result))
                            {
                                field.Inherited = false;
                                await fieldManager.UpsertTestCaseFieldAsync(principal, field);
                            }
                        }
                    }
                }
            }
            
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            using var scope = _serviceProvider.CreateScope();
            var testRepo = scope.ServiceProvider.GetRequiredService<ITestCaseRepository>();

            FilterSpecification<TestCase>[] filters = [new FilterTestCasesThatRequireClassification()];

            while(!stoppingToken.IsCancellationRequested)
            {
                var result = await testRepo.SearchTestCasesAsync(0, 5, filters);
                if(result.Items.Length > 0)
                {
                    foreach (var testCase in result.Items)
                    {
                        try
                        {
                            testCase.ClassificationRequired = false;
                            await testRepo.UpdateTestCaseAsync(testCase);

                            await AssignGeneratedFieldsAsync(scope, testCase);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to classify test: {test}", testCase.Name);
                            await Task.Delay(60000, stoppingToken);
                        }
                    }
                }

                if (result.TotalCount == 0)
                {
                    await Task.Delay(30000, stoppingToken);
                }
                else
                {
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
    }
}
