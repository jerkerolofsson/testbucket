﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.TestResources.Settings;

/// <summary>
/// Setting to delete a resource if it is not seen for a long time
/// </summary>
internal class DeleteResourceIfNotSeenFor : SettingAdapter
{
    private readonly ISettingsProvider _settingsProvider;

    public DeleteResourceIfNotSeenFor(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;

        Metadata.Name = "delete-resource-if-not-seen-for";
        Metadata.Category.Name = "tenant";
        Metadata.Category.Icon = SettingIcons.Tenant;
        Metadata.Section.Name = "test-resources";
        Metadata.Section.Icon = SettingIcons.TestResources;
        Metadata.Type = FieldType.TimeSpan;
    }

    public override async Task<FieldValue> ReadAsync(SettingContext context)
    {
        var resourceManagerSettings = await _settingsProvider.GetDomainSettingsAsync<TestResourceManagerSettings>(context.TenantId, null);
        resourceManagerSettings ??= new();

        return new FieldValue { TimeSpanValue = resourceManagerSettings.DeleteResourceIfNotSeenFor, FieldDefinitionId = 0 };
    }

    public override async Task WriteAsync(SettingContext context, FieldValue value)
    {
        var resourceManagerSettings = await _settingsProvider.GetDomainSettingsAsync<TestResourceManagerSettings>(context.TenantId, null);
        resourceManagerSettings ??= new();

        if (value.TimeSpanValue is not null)
        {
            resourceManagerSettings.DeleteResourceIfNotSeenFor = value.TimeSpanValue.Value;
            await _settingsProvider.SaveDomainSettingsAsync(context.TenantId, null, resourceManagerSettings);
        }
    }
}
