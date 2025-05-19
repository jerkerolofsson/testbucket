using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.IntegrationTests.Settings
{
    [IntegrationTest]
    [FunctionalTest]
    [Component("Settings")]
    public class SearchSettingsTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        private const string IncreasedContrastName = "Increased Contrast";

        [Fact]
        [TestDescription("""
            Verifies that a setting is found when searching for it's name
            """)]
        public void Search_WithName_SettingFound()
        {
            var settings = Fixture.Settings.Search("Increased Contrast");
            Assert.NotEmpty(settings);
            Assert.NotNull(settings.FirstOrDefault(x => x.Metadata.Name == IncreasedContrastName));
        }

        [Fact]
        [TestDescription("""
            Verifies that a setting is found when searching for it's partial name
            """)]
        public void Search_WithPartialName_SettingFound()
        {
            var settings = Fixture.Settings.Search("Contrast");
            Assert.NotEmpty(settings);
            Assert.NotNull(settings.FirstOrDefault(x => x.Metadata.Name == IncreasedContrastName));
        }

        [Fact]
        [TestDescription("""
            Verifies that a setting is found when searching for it's partial name in lower case
            """)]
        public void Search_WithPartialNameInLowerCase_SettingFound()
        {
            var settings = Fixture.Settings.Search("contrast");
            Assert.NotEmpty(settings);
            Assert.NotNull(settings.FirstOrDefault(x => x.Metadata.Name == IncreasedContrastName));
        }

        [Fact]
        [TestDescription("""
            Verifies that a setting is found when searching for it's partial name in lower case
            """)]
        public void Search_WithCategory_SettingFound()
        {
            var settings = Fixture.Settings.Search("Accessibility");
            Assert.NotEmpty(settings);
            Assert.NotNull(settings.FirstOrDefault(x => x.Metadata.Name == IncreasedContrastName));
        }


        [Fact]
        [TestDescription("""
            Verifies that a setting is found when searching when the search phrase matches the description
            """)]
        public void Search_WithDescription_SettingFound()
        {
            var settings = Fixture.Settings.Search("easier");
            Assert.NotEmpty(settings);
            Assert.NotNull(settings.FirstOrDefault(x => x.Metadata.Name == IncreasedContrastName));
        }

        [Fact]
        [TestDescription("""
            Verifies that there is a category for every setting
            """)]
        public void Categories_EverySettingBelongsToACategory()
        {
            var settings = Fixture.Settings.Search("");
            var categories = Fixture.Settings.Categories;

            foreach(var setting in settings)
            {
                var category = categories.FirstOrDefault(x => x.Name == setting.Metadata.Category.Name);
                Assert.NotNull(category);
            }
        }
    }
}
