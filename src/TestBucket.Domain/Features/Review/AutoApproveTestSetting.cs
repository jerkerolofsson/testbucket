using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Editor.Models;

namespace TestBucket.Domain.Features.Review;
internal class AutoApproveTestSetting : SettingAdapter
{
    private readonly ISettingsProvider _settingsProvider;

    public AutoApproveTestSetting(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;

        Metadata.Name = "auto-approve-tests";
        Metadata.Description = "auto-approve-tests-description";
        Metadata.Category.Name = "testing";
        Metadata.Category.Icon = SettingIcons.Testing;
        Metadata.Section.Name = "review";
        Metadata.Section.Icon = SettingIcons.Review;
        Metadata.SearchText = "review approve";
        Metadata.ShowDescription = true;
        Metadata.Type = FieldType.Boolean;
        Metadata.AccessLevel = Identity.Models.AccessLevel.Admin;
    }

    public override async Task<FieldValue> ReadAsync(SettingContext context)
    {
        context.Principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Read);
        var settings = await _settingsProvider.GetDomainSettingsAsync<ReviewSettings>(context.Principal.GetTenantIdOrThrow(), context.ProjectId);
        settings ??= new();

        return new FieldValue { BooleanValue = settings.AutomaticallyApproveTestIfReviewFinishedWithOnlyPositiveVotes, FieldDefinitionId = 0 };
    }

    public override async Task WriteAsync(SettingContext context, FieldValue value)
    {
        var settings = await _settingsProvider.GetDomainSettingsAsync<ReviewSettings>(context.Principal.GetTenantIdOrThrow(), context.ProjectId);
        settings ??= new();

        if (settings.AutomaticallyApproveTestIfReviewFinishedWithOnlyPositiveVotes != value.BooleanValue)
        {
            settings.AutomaticallyApproveTestIfReviewFinishedWithOnlyPositiveVotes = value.BooleanValue ?? false;
            await _settingsProvider.SaveDomainSettingsAsync(context.Principal.GetTenantIdOrThrow(), context.ProjectId, settings);
        }
    }
}
