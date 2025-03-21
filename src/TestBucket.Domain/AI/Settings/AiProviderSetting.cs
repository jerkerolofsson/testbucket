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
    class AiProviderSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;

        public AiProviderSetting(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            Metadata.Name = "AI Provider";
            Metadata.Description = "Provider of AI";
            Metadata.Category.Name = "AI";
            Metadata.Category.Icon = SettingIcon.AI;
            Metadata.Section.Name = "Provider";
            Metadata.Type = FieldType.SingleSelection;
            Metadata.ShowDescription = true;
            Metadata.AccessLevel = Identity.Models.AccessLevel.SuperAdmin;

            Metadata.Options = ["ollama", "github-models", "azure-ai"];
        }

        public override async Task<FieldValue> ReadAsync(SettingContext principal)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();
            return new FieldValue { StringValue = settings.AiProvider, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext principal, FieldValue value)
        {
            var settings = await _settingsProvider.LoadGlobalSettingsAsync();

            if (settings.AiProvider != value.StringValue)
            {
                settings.AiProvider = value.StringValue ?? "ollama";

                switch(settings.AiProvider)
                {
                    case "ollama":
                        settings.AiProviderUrl = Environment.GetEnvironmentVariable("OLLAMA_BASE_URL") ?? "http://localhost:11434";
                        break;
                    default:
                        settings.AiProviderUrl = "https://models.inference.ai.azure.com";
                        break;
                }
                await _settingsProvider.SaveGlobalSettingsAsync(settings);
            }
        }
    }
}
