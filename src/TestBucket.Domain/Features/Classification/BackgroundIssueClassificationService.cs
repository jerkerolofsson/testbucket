using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.AI.Models;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCases;

namespace TestBucket.Domain.Features.Classification
{
    /// <summary>
    /// Classifies empty issue fields
    /// </summary>
    public class BackgroundIssueClassificationService : BackgroundService
    {
        private readonly ILogger<BackgroundIssueClassificationService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public BackgroundIssueClassificationService(
            ILogger<BackgroundIssueClassificationService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        private ClaimsPrincipal Impersonate(LocalIssue issue, string username)
        {
            return Impersonation.Impersonate(builder =>
            {
                builder.UserName = username;
                builder.Email = username;
                builder.TenantId = issue.TenantId;
                builder.Add(PermissionEntityType.Project, PermissionLevel.ReadWrite);
                builder.Add(PermissionEntityType.Team, PermissionLevel.Read);
                builder.Add(PermissionEntityType.TestCase, PermissionLevel.ReadWrite);
                builder.Add(PermissionEntityType.Issue, PermissionLevel.ReadWrite);
                builder.Add(PermissionEntityType.Requirement, PermissionLevel.ReadWrite);
                builder.Add(PermissionEntityType.Architecture, PermissionLevel.Read);
            });
        }

        private async Task AssignGeneratedFieldsAsync(IServiceScope scope, LocalIssue issue)
        {
            if (issue.Description is null)
            {
                return;
            }
            var classifier = scope.ServiceProvider.GetRequiredService<IClassifier>();
            var username = await classifier.GetModelNameAsync(ModelType.Classification) ?? "ai-classifier";
            var principal = Impersonate(issue, username);
            var fieldDefinitionManager = scope.ServiceProvider.GetRequiredService<IFieldDefinitionManager>();
            var fieldManager = scope.ServiceProvider.GetRequiredService<IFieldManager>();

            var definitions = await fieldDefinitionManager.GetDefinitionsAsync(principal, issue.TestProjectId, FieldTarget.Issue);
            var fields = await fieldManager.GetIssueFieldsAsync(principal, issue.Id, definitions);
            foreach (var field in fields)
            {
                var hasValue = field.HasValue();
                if (!hasValue && field.FieldDefinition?.UseClassifier == true)
                {
                    var options = await fieldDefinitionManager.GetOptionsAsync(principal, field.FieldDefinition);
                    if (options.Count == 1)
                    {
                        field.StringValue = options[0];
                        field.Inherited = false;
                        await fieldManager.UpsertIssueFieldAsync(principal, field);
                    }
                    else if (options.Count >= 2)
                    {
                        var result = await classifier.ClassifyAsync(principal, field.FieldDefinition.Name, options.ToArray(), issue);
                        if (result.Length > 0)
                        {
                            _logger.LogInformation("Classified {issue} with {category}", issue.ExternalDisplayId, result[0]);
                            if (FieldValueConverter.TryAssignValue(field.FieldDefinition, field, result))
                            {
                                field.Inherited = false;
                                await fieldManager.UpsertIssueFieldAsync(principal, field);
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
            var repo = scope.ServiceProvider.GetRequiredService<IIssueRepository>();

            List<FilterSpecification<LocalIssue>> filters = [new FilterIssuesThatRequireClassification()];

            while(!stoppingToken.IsCancellationRequested)
            {
                var result = await repo.SearchAsync(filters, 0, 5);
                if(result.Items.Length > 0)
                {
                    foreach (var issue in result.Items)
                    {
                        try
                        {
                            issue.ClassificationRequired = false;
                            await repo.UpdateLocalIssueAsync(issue);

                            await AssignGeneratedFieldsAsync(scope, issue);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to classify test: {issue}", issue.ExternalDisplayId);
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
