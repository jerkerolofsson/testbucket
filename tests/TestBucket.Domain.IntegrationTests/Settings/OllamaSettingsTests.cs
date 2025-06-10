using TestBucket.Domain.AI.Settings;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Models;

namespace TestBucket.Domain.IntegrationTests.Settings
{
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
            var provider = await aiProviderSetting.ReadAsync(new SettingContext() { Principal = Fixture.App.SiteAdministrator });
            Assert.Equal("ollama", provider.StringValue);

            var globalSettings = await settingsProvider.LoadGlobalSettingsAsync();
            Assert.Equal("ollama", globalSettings.AiProvider);

            // Note: This URL is defined in the fixture setup, so it should match the expected value
            Assert.Equal("http://localhost:11435", globalSettings.AiProviderUrl);
        }
    }
}
