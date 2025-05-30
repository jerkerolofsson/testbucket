using TestBucket.Blazor.Components;

namespace TestBucket.Blazor.Tests.Controls
{
    [UnitTest]
    [EnrichedTest]
    [Component("Blazor Components")]

    public class LockedToggleTests : Bunit.TestContext
    {
        /// <summary>
        /// Verifies that the Locked and Unlocked labels are rendered with the custom labels assigned
        /// </summary>
        [Fact]
        public void Click_WithLockedLabels_RenderedCorrectly()
        {
            Services.AddMudServices();

            // Act
            var cut = RenderComponent<LockedToggle>(parameters =>
            {
                parameters.Add(p => p.Locked, true);
                parameters.Add(p => p.LockedLabel, "LOCK");
                parameters.Add(p => p.UnlockedLabel, "UNLOCK");
            });


            // Assert
            var lockedLabel = cut.Find(".locked-label");
            Assert.NotNull(lockedLabel);
            Assert.Equal("LOCK", lockedLabel.TextContent);

            var unlockedLabel = cut.Find(".unlocked-label");
            Assert.NotNull(unlockedLabel);
            Assert.Equal("UNLOCK", unlockedLabel.TextContent);
        }

        /// <summary>
        /// Verifies that the LockedChanged event is invoked with "false" when clicking when the initial state was Locked=true
        /// </summary>
        [Fact]
        public void Click_WithLocked_LockedChangedToFalse()
        {
            Services.AddMudServices();
            bool? valueFromLockedChanged = null;
            var cut = RenderComponent<LockedToggle>(parameters =>
            {
                parameters.Add(p => p.Locked, true);
                parameters.Add(p => p.LockedChanged, (isLocked) => valueFromLockedChanged = isLocked);
            });

            // Act
            var element = cut.Find(".tb-locked-toggle");
            Assert.NotNull(element);
            element.Click();

            // Assert
            Assert.False(valueFromLockedChanged);
        }

        /// <summary>
        /// Verifies that the LockedChanged event is invoked with "true" when clicking when the initial state was Locked=false
        /// </summary>
        [Fact]
        public void Click_WithUnocked_LockedChangedToTrue()
        {
            Services.AddMudServices();
            bool? valueFromLockedChanged = null;
            var cut = RenderComponent<LockedToggle>(parameters =>
            {
                parameters.Add(p => p.Locked, false);
                parameters.Add(p => p.LockedChanged, (isLocked) => valueFromLockedChanged = isLocked);
            });

            // Act
            var element = cut.Find(".tb-locked-toggle");
            Assert.NotNull(element);
            element.Click();

            // Assert
            Assert.True(valueFromLockedChanged);
        }

    }
}
