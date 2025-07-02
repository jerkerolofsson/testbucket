using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.AI.Models;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCases;

namespace TestBucket.Domain.Features.Classification
{
    /// <summary>
    /// Classifies empty test case fields
    /// </summary>
    public class BackgroundTestClassificationService : BackgroundService
    {
        private readonly ILogger<BackgroundTestClassificationService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public BackgroundTestClassificationService(
            ILogger<BackgroundTestClassificationService> logger,
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
                builder.AddAllPermissions();
            });
        }

        private async Task AssignGeneratedFieldsAsync(IServiceScope scope, TestCase testCase)
        {
            if (testCase.Description is null)
            {
                return;
            }
            var classifier = scope.ServiceProvider.GetRequiredService<IClassifier>();
            var username = "classification-bot";
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
                        field.StringValue = options[0].Title;
                        field.Inherited = false;
                        await fieldManager.UpsertTestCaseFieldAsync(principal, field);
                    }
                    else if (options.Count >= 2)
                    {
                        var result = await classifier.ClassifyAsync(principal, field.FieldDefinition.Name, options, testCase);
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
            await Task.Delay(TimeSpan.FromSeconds(190), stoppingToken);

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
