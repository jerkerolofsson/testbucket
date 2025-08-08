using TestBucket.Domain.AI.Settings.LLM;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Models;

namespace TestBucket.Domain.IntegrationTests.Settings
{
    /// <summary>
    /// Tests releated to ollama settings
    /// </summary>
    /// <param name="Fixture"></param>
    [IntegrationTest]
    [FunctionalTest]
    [Component("Settings")]
    public class OllamaSettingsTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Reads the ollama provider settings and verifies that it is correctly configured as the SeedConfiguration
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ReadOllamaSettings_WithSeededConfiguration_SettingsCorrect()
        {
            var settingsProvider = Fixture.App.Services.GetRequiredService<ISettingsProvider>();
            var settings = Fixture.App.Services.GetRequiredService<IEnumerable<ISetting>>();
            var aiProviderSetting = settings.FirstOrDefault(s => s is AiProviderSetting) as AiProviderSetting;
            Assert.NotNull(aiProviderSetting);

            // Verify the ISetting
            var provider = await aiProviderSetting.ReadAsync(new SettingContext() { Principal = Fixture.App.SiteAdministrator, TenantId = "tenant1" });
            Assert.Equal("ollama", provider.StringValue);

            var llmSettings = await settingsProvider.GetDomainSettingsAsync<LlmSettings>("tenant1", null);
            Assert.NotNull(llmSettings);
            Assert.Equal("ollama", llmSettings.AiProvider);

            // Note: This URL is defined in the fixture setup, so it should match the expected value
            Assert.Equal("http://localhost:11435", llmSettings.AiProviderUrl);
        }
    }
}
