﻿
using Mediator;

using TestBucket.Domain.AI.Models;
using TestBucket.Domain.AI.Ollama;

namespace TestBucket.Domain.AI.Settings.LLM
{
    class AiLlmEmbeddingModelSetting : SettingAdapter
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IMediator _mediator;

        public AiLlmEmbeddingModelSetting(ISettingsProvider settingsProvider, IMediator progressManager)
        {
            _settingsProvider = settingsProvider;
            _mediator = progressManager;

            Metadata.Name = "ai-embedding-model";
            Metadata.Description = null;
            Metadata.Category.Name = "AI";
            Metadata.Category.Icon = SettingIcons.AI;
            Metadata.Section.Name = "ai-models";
            Metadata.SearchText = "ai-models";
            Metadata.ShowDescription = true;
            Metadata.Type = FieldType.String;
            Metadata.AccessLevel = Identity.Models.AccessLevel.Admin;
            Metadata.Options = LlmModels.GetNames(ModelCapability.Embedding);
        }

        public override async Task<FieldValue> ReadAsync(SettingContext context)
        {
            var settings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(context.Principal.GetTenantIdOrThrow(), null);
            settings ??= new();
            return new FieldValue { StringValue = settings.LlmEmbeddingModel, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            context.Principal.ThrowIfNoPermission(PermissionEntityType.Tenant, PermissionLevel.Write);
            var settings = await _settingsProvider.GetDomainSettingsAsync<LlmSettings>(context.Principal.GetTenantIdOrThrow(), null);
            settings ??= new();

            if (settings.LlmEmbeddingModel != value.StringValue)
            {
                settings.LlmEmbeddingModel = value.StringValue;
                await _settingsProvider.SaveDomainSettingsAsync(context.Principal.GetTenantIdOrThrow(), null, settings);

                if (settings.AiProvider == "ollama" && settings.AiProviderUrl != null && settings.LlmEmbeddingModel is not null)
                {
                    if (!string.IsNullOrEmpty(settings.LlmEmbeddingModel) && settings.AiProvider == "ollama" && !string.IsNullOrEmpty(settings.AiProviderUrl))
                    {
                        await _mediator.Send(new PullModelRequest(context.Principal, settings.LlmEmbeddingModel));
                    }
                }
            }
        }
    }
}
