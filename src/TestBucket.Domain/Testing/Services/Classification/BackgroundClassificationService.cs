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
using TestBucket.Domain.AI;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCases;

namespace TestBucket.Domain.Testing.Services.Classification
{
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

        private ClaimsPrincipal Impersonate(TestCase testCase)
        {
            return Impersonation.Impersonate(testCase.TenantId);
        }

        private async Task AssignGeneratedFieldsAsync(IServiceScope scope, TestCase testCase)
        {
            if (testCase.Description is null)
            {
                return;
            }
            var principal = Impersonate(testCase);
            var classifier = scope.ServiceProvider.GetRequiredService<IClassifier>();
            var fieldDefinitionManager = scope.ServiceProvider.GetRequiredService<IFieldDefinitionManager>();
            var fieldManager = scope.ServiceProvider.GetRequiredService<IFieldManager>();


            var definitions = await fieldDefinitionManager.GetDefinitionsAsync(principal, testCase.TestProjectId, FieldTarget.TestCase);
            var fields = await fieldManager.GetTestCaseFieldsAsync(principal, testCase.Id, definitions);
            bool changed = false;
            foreach (var field in fields)
            {
                var hasValue = field.HasValue();
                if (!hasValue && 
                    field.FieldDefinition?.Options is not null && 
                    field.FieldDefinition.Options.Count > 0 && 
                    field.FieldDefinition.UseClassifier)
                {
                    var result = await classifier.ClassifyAsync(field.FieldDefinition.Options.ToArray(), testCase.Description);
                    if (result.Length > 0)
                    {
                        _logger.LogInformation("Classified {test} with {category}", testCase.Name, result[0]);
                        if (FieldValueConverter.TryAssignValue(field.FieldDefinition, field, result))
                        {
                            changed = true;

                            await fieldManager.UpsertTestCaseFieldAsync(principal, field);
                        }
                    }
                }
            }
            if (changed)
            {
                //await fieldManager.SaveTestCaseFieldsAsync(principal, fields);
            }
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(120), stoppingToken);

            using var scope = _serviceProvider.CreateScope();
            var testRepo = scope.ServiceProvider.GetRequiredService<ITestCaseRepository>();

            FilterSpecification<TestCase>[] filters = [new FilterTestCasesThatRequireClassification()];

            while(!stoppingToken.IsCancellationRequested)
            {
                var result = await testRepo.SearchTestCasesAsync(0, 1, filters);
                if(result.Items.Length > 0)
                {
                    var testCase = result.Items[0];
                    try
                    {
                        testCase.ClassificationRequired = false;
                        await testRepo.UpdateTestCaseAsync(testCase);

                        await AssignGeneratedFieldsAsync(scope, testCase);
                    }
                    catch(Exception ex) 
                    {
                        _logger.LogError(ex, "Failed to classify test: {test}", testCase.Name);
                        await Task.Delay(60000, stoppingToken);
                    }
                }

                if (result.TotalCount == 0)
                {
                    await Task.Delay(180000, stoppingToken);
                }
                else
                {
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
    }
}
