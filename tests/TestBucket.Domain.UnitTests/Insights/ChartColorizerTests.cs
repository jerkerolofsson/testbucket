using System.Collections.Generic;
using TestBucket.Contracts.Appearance.Models;
using TestBucket.Contracts.Insights;
using TestBucket.Domain.Insights;
using TestBucket.Domain.Insights.Model;
using Xunit;

namespace TestBucket.Domain.UnitTests.Insights
{
    /// <summary>
    /// Unit tests for the <see cref="ChartColorizer"/> class.
    /// </summary>
    [Component("Insights")]
    [Feature("Insights")]
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    public class ChartColorizerTests
    {
        /// <summary>
        /// Tests the ChartColorizer.GetColorway method for the ByLabel mode with the darkmode palette. 
        /// </summary>
        [Fact]
        public void GetColorway_ByLabelModeInDarkMode_ReturnsCorrectColors()
        {
            // Arrange
            var palette = new ThemePalette { Name = "TestPalette", Colors = new List<ThemeColor> { new ThemeColor("#ffffff"), new ThemeColor("#ffffff") } };
            var darkPalette = new ThemePalette { Name = "TestPalette", Colors = new List<ThemeColor> { new ThemeColor("#dd0000"), new ThemeColor("#00cc00") } };
            var lightPalette = new ThemePalette { Name = "TestPalette", Colors = new List<ThemeColor> { new ThemeColor("#ffffff"), new ThemeColor("#ffffff") } };
            var colorizer = new ChartColorizer(palette);
            var spec = new InsightsVisualizationSpecification
            {
                Name = "TestSpec",
                ColorMode = ChartColorMode.ByLabel,
                LightModeColors = new ChartColors { Palette = lightPalette },
                DarkModeColors = new ChartColors { Palette = darkPalette },
            };
            var data = new InsightsData<string, int>();
            var series = data.Add("Series1");
            series.Add("Label1", 10);
            series.Add("Label2", 20);

            // Act
            var colorway = colorizer.GetColorway(spec, data, isDarkMode: true);

            // Assert
            Assert.Equal("#dd0000ff", colorway["Label1"]);
            Assert.Equal("#00cc00ff", colorway["Label2"]);
        }

        /// <summary>
        /// Tests the ChartColorizer.GetColorway method for the ByLabel mode with the light mode palette.
        /// </summary>
        [Fact]
        public void GetColorway_ByLabelModeInLightMode_ReturnsCorrectColors()
        {
            // Arrange
            var palette = new ThemePalette { Name = "TestPalette", Colors = new List<ThemeColor> { new ThemeColor("#ffffff"), new ThemeColor("#ffffff") } };
            var darkPalette = new ThemePalette { Name = "TestPalette", Colors = new List<ThemeColor> { new ThemeColor("#ffffff"), new ThemeColor("#ffffff") } };
            var lightPalette = new ThemePalette { Name = "TestPalette", Colors = new List<ThemeColor> { new ThemeColor("#ee0000"), new ThemeColor("#00ee00") } };
            var colorizer = new ChartColorizer(palette);
            var spec = new InsightsVisualizationSpecification
            {
                Name = "TestSpec",
                ColorMode = ChartColorMode.ByLabel,
                LightModeColors = new ChartColors { Palette = lightPalette },
                DarkModeColors = new ChartColors { Palette = darkPalette },
            };
            var data = new InsightsData<string, int>();
            var series = data.Add("Series1");
            series.Add("Label1", 10);
            series.Add("Label2", 20);

            // Act
            var colorway = colorizer.GetColorway(spec, data, isDarkMode: false);

            // Assert
            Assert.Equal("#ee0000ff", colorway["Label1"]);
            Assert.Equal("#00ee00ff", colorway["Label2"]);
        }

        /// <summary>
        /// Tests the ChartColorizer.GetColorway method for the BySeries mode with the light-mode palette.
        /// </summary>
        [Fact]
        public void GetColorway_BySeriesModeInLightMode_ReturnsCorrectColors()
        {
            // Arrange
            var palette = new ThemePalette { Name = "TestPalette", Colors = new List<ThemeColor> { new ThemeColor("#ffffff"), new ThemeColor("#ffffff") } };
            var darkPalette = new ThemePalette { Name = "TestPalette", Colors = new List<ThemeColor> { new ThemeColor("#ffffff"), new ThemeColor("#ffffff") } };
            var lightPalette = new ThemePalette { Name = "TestPalette", Colors = new List<ThemeColor> { new ThemeColor("#ee0000"), new ThemeColor("#00ee00") } };
            var colorizer = new ChartColorizer(palette);
            var spec = new InsightsVisualizationSpecification 
            { 
                Name = "TestSpec", 
                ColorMode = ChartColorMode.BySeries ,
                LightModeColors = new ChartColors { Palette = lightPalette },
                DarkModeColors = new ChartColors { Palette = darkPalette },
            };
            var data = new InsightsData<string, int>();
            var series1 = data.Add("Series1");
            series1.Add("Label1", 10);
            series1.Add("Label2", 20);

            var series2 = data.Add("Series2");
            series2.Add("Label3", 30);
            series2.Add("Label4", 40);

            // Act
            var colorway = colorizer.GetColorway(spec, data, isDarkMode: false);

            // Assert
            Assert.Equal("#ee0000ff", colorway["Series1"]);
            Assert.Equal("#00ee00ff", colorway["Series2"]);
        }


        /// <summary>
        /// Tests the ChartColorizer.GetColorway method for the BySeries mode with the dark-mode palette.
        /// </summary>
        [Fact]
        public void GetColorway_BySeriesModeInDarkMode_ReturnsCorrectColors()
        {
            // Arrange
            var palette = new ThemePalette { Name = "TestPalette", Colors = new List<ThemeColor> { new ThemeColor("#ffffff"), new ThemeColor("#ffffff") } };
            var darkPalette = new ThemePalette { Name = "TestPalette", Colors = new List<ThemeColor> { new ThemeColor("#dd0000"), new ThemeColor("#00cc00") } };
            var lightPalette = new ThemePalette { Name = "TestPalette", Colors = new List<ThemeColor> { new ThemeColor("#ffffff"), new ThemeColor("#ffffff") } };
            var colorizer = new ChartColorizer(palette);
            var spec = new InsightsVisualizationSpecification
            {
                Name = "TestSpec",
                ColorMode = ChartColorMode.BySeries,
                LightModeColors = new ChartColors { Palette = lightPalette },
                DarkModeColors = new ChartColors { Palette = darkPalette },
            };
            var data = new InsightsData<string, int>();
            var series1 = data.Add("Series1");
            series1.Add("Label1", 10);
            series1.Add("Label2", 20);

            var series2 = data.Add("Series2");
            series2.Add("Label3", 30);
            series2.Add("Label4", 40);

            // Act
            var colorway = colorizer.GetColorway(spec, data, isDarkMode: true);

            // Assert
            Assert.Equal("#dd0000ff", colorway["Series1"]);
            Assert.Equal("#00cc00ff", colorway["Series2"]);
        }
    }
}
