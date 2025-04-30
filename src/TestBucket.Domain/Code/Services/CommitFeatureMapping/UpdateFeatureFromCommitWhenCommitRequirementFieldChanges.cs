using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using Microsoft.Extensions.Logging;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Fields.Events;

namespace TestBucket.Domain.Code.Services.CommitFeatureMapping;
public class UpdateFeatureFromCommitWhenCommitRequirementFieldChanges : INotificationHandler<RequirementFieldChangedNotification>
{
    private readonly ICommitManager _commitManager;
    private readonly IFieldManager _fieldManager;
    private readonly IArchitectureManager _architectureManager;
    private readonly IMediator _mediator;
    private readonly ILogger<UpdateFeatureFromCommitWhenCommitRequirementFieldChanges> _logger;

    public UpdateFeatureFromCommitWhenCommitRequirementFieldChanges(ICommitManager commitManager, IFieldManager fieldManager, IArchitectureManager architectureManager, ILogger<UpdateFeatureFromCommitWhenCommitRequirementFieldChanges> logger, IMediator mediator)
    {
        _commitManager = commitManager;
        _fieldManager = fieldManager;
        _architectureManager = architectureManager;
        _logger = logger;
        _mediator = mediator;
    }

    public async ValueTask Handle(RequirementFieldChangedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Field.FieldDefinition?.TestProjectId is null)
        {
            return;
        }
        if (string.IsNullOrEmpty(notification.Field.StringValue))
        {
            return;
        }
        var principal = notification.Principal;

        if (notification.Field.FieldDefinition.TraitType == Traits.Core.TraitType.Commit)
        {
            var sha = notification.Field.StringValue;
            Commit? commit = await _commitManager.GetCommitByShaAsync(principal, sha);
            if(commit is not null)
            {
                // See if the requirement has a feature linked
                var fields = await _fieldManager.GetRequirementFieldsAsync(principal, notification.Field.RequirementId, []);
                var featureField = fields.Where(x => x.FieldDefinition?.TraitType == Traits.Core.TraitType.Feature).FirstOrDefault();
                if(featureField?.StringValue is not null)
                {
                    var featureName = featureField.StringValue;
                    var feature = await _architectureManager.GetFeatureByNameAsync(principal, notification.Field.FieldDefinition.TestProjectId.Value, featureName);
                    if(feature is not null)
                    {
                        await _mediator.Send(new MapCommitFilesToFeatureRequest(principal, commit, feature));
                    }
                }
            }
        }
    }
}
