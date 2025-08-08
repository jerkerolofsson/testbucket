using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Milestones;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.IntegrationTests.Labels
{
    /// <summary>
    /// TEsts related managing labels
    /// </summary>
    /// <param name="Fixture"></param>
    [IntegrationTest]
    [FunctionalTest]
    [Component("Labels")]
    [Feature("Fields")]
    public class LabelManagerTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that field options when the field trait is Label contains the label added by a user
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetFieldOptions_AfterAddedLabel_ContainsLabel()
        {
            using var scope = Fixture.Services.CreateScope();
            var fieldDefinitionManager = scope.ServiceProvider.GetRequiredService<IFieldDefinitionManager>();
            var principal = Fixture.App.SiteAdministrator;

            await Fixture.Labels.AddLabelAsync("GetFieldOptions_ContainsLabel");

            // Act
            var fields = await fieldDefinitionManager.GetDefinitionsAsync(principal, Fixture.ProjectId);
            var milestoneField = fields.Where(x => x.TraitType == TraitType.Label).First();
            var options = await fieldDefinitionManager.GetOptionsAsync(principal, milestoneField);

            // Assert
            Assert.NotEmpty(options);
            Assert.Contains("GetFieldOptions_ContainsLabel", options.Select(x=>x.Title));
        }

        /// <summary>
        /// Verifies that GetLabelByNameAsync returns a label after it has been added
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetLabelByNameAsync_AfterAddedLabel_ReturnsLabel()
        {

            await Fixture.Labels.AddLabelAsync("GetLabelByNameAsync_AfterAddedLabel_ReturnsLabel");

            // Act
            var label = await Fixture.Labels.GetLabelByNameAsync("GetLabelByNameAsync_AfterAddedLabel_ReturnsLabel");

            // Assert
            Assert.NotNull(label);
        }

        /// <summary>
        /// Verifies that GetLabelsAsync returns a label after it has been added
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetLabelsAsync_AfterAddedLabel_ContainsAddedLabel()
        {

            await Fixture.Labels.AddLabelAsync("GetLabelsAsync_AfterAddedLabel_ContainsAddedLabel");

            // Act
            var labels = await Fixture.Labels.GetLabelsAsync();

            // Assert
            Assert.NotNull(labels.Where(x=>x.Title == "GetLabelsAsync_AfterAddedLabel_ContainsAddedLabel"));
        }

        /// <summary>
        /// Verifies that GetLabelsAsync does not return a label after it has been delweted
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetLabelsAsync_AfterDeletedLabel_DoesNotContainsLabel()
        {
            var label = await Fixture.Labels.AddLabelAsync("GetLabelsAsync_AfterDeletedLabel_DoesNotContainsLabel");
            await Fixture.Labels.DeleteLabelAsync(label);

            // Act
            var labels = await Fixture.Labels.GetLabelsAsync();

            // Assert
            Assert.Null(labels.FirstOrDefault(x => x.Title == "GetLabelsAsync_AfterDeletedLabel_DoesNotContainsLabel"));
        }


        /// <summary>
        /// Verifies that GetLabelsAsync returns the latest label after it has been updated
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetLabelsAsync_AfterUpdatedDescription_ContainsUpdatedDescription()
        {

            var label = await Fixture.Labels.AddLabelAsync("GetLabelsAsync_AfterUpdatedDescription_ContainsUpdatedDescription");
            label.Description = "asdasd";
            await Fixture.Labels.UpdateLabelAsync(label);

            // Act
            var labels = await Fixture.Labels.GetLabelsAsync();

            // Assert
            var labelAfter = labels.FirstOrDefault(x => x.Title == "GetLabelsAsync_AfterUpdatedDescription_ContainsUpdatedDescription");
            Assert.NotNull(labelAfter);
            Assert.Equal(label.Description, labelAfter.Description);
        }

        /// <summary>
        /// Verifies that GetLabelsAsync returns the latest label after it has been updated
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetLabelByNameAsync_AfterUpdatedColor_ContainsUpdatedColor()
        {

            var label = await Fixture.Labels.AddLabelAsync("GetLabelByNameAsync_AfterUpdatedColor_ContainsUpdatedColor");
            label.Color = "#FF00FF";
            await Fixture.Labels.UpdateLabelAsync(label);

            // Act
            var labelAfter = await Fixture.Labels.GetLabelByNameAsync("GetLabelByNameAsync_AfterUpdatedColor_ContainsUpdatedColor");

            // Assert
            Assert.NotNull(labelAfter);
            Assert.Equal(label.Color, labelAfter.Color);
        }
    }
}
