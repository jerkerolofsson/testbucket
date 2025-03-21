using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Settings.Models;

namespace TestBucket.Domain.Settings.Server
{
    class DefaultTenantSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;

        public DefaultTenantSetting(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
            Metadata.Name = "Default Tenant";
            Metadata.Description = "The default tenant when accessing the application without a specified tenant";
            Metadata.Category.Name = "Server";
            Metadata.Category.Icon = SettingIcon.Server;
            Metadata.Section.Name = "Defaults";
            Metadata.ShowDescription = true;
            Metadata.Type = FieldType.String;
            Metadata.AccessLevel = Identity.Models.AccessLevel.SuperAdmin;
        }

        public override async Task<FieldValue> ReadAsync(SettingContext context)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();
            return new FieldValue { StringValue = settings.DefaultTenant, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            if(value.StringValue is null)
            {
                return;
            }
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();
            settings.DefaultTenant = value.StringValue;
            await _settingsProvider.SaveGlobalSettingsAsync(settings);
        }
    }
}
