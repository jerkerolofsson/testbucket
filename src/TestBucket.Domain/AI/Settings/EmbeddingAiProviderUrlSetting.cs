using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Settings;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Domain.AI.Settings
{
    class EmbeddingAiProviderUrlSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;

        public EmbeddingAiProviderUrlSetting(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            Metadata.Name = "embedding-ai-provider-url";
            Metadata.Description = null;
            Metadata.Category.Name = "AI";
            Metadata.Category.Icon = SettingIcon.AI;
            Metadata.Section.Name = "ai-provider";
            Metadata.ShowDescription = true;
            Metadata.Type = FieldType.String;
            Metadata.AccessLevel = Identity.Models.AccessLevel.SuperAdmin;

        }

        public override async Task<FieldValue> ReadAsync(SettingContext principal)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();
            return new FieldValue { StringValue = settings.EmbeddingAiProviderUrl, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext principal, FieldValue value)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();

            if (settings.EmbeddingAiProvider != value.StringValue)
            {
                settings.EmbeddingAiProviderUrl = value.StringValue;
                await _settingsProvider.SaveGlobalSettingsAsync(settings);
            }
        }
    }
}
