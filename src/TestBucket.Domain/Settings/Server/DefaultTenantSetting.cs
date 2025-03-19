using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
            Metadata.Section.Name = "Defaults";
            Metadata.ShowDescription = true;
            Metadata.Type = FieldType.String;
        }

        public override async Task<FieldValue> ReadAsync(ClaimsPrincipal principal)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();
            return new FieldValue { StringValue = settings.DefaultTenant, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(ClaimsPrincipal principal, FieldValue value)
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
