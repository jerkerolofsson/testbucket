using TestBucket.Blazor.Components;

namespace TestBucket.Blazor.Tests.Controls
{
    [UnitTest]
    [EnrichedTest]
    [Component("Blazor Components")]
    public class DockToggleTests : Bunit.TestContext
    {
        [Fact]
        [TestDescription("Verifies that the initial orientation is correct")]
        public void Render_WithDockLeft_RotationCorrect()
        {
            Services.AddMudServices();

            // Act
            var cut = RenderComponent<DockToggle>(parameters =>
            {
                parameters.Add(p => p.Dock, Dock.Left );
            });

            // Assert
            var element = cut.Find(".dock-toggle");
            Assert.NotNull( element );
            var style = element.GetAttribute("style");
            Assert.NotNull(style);
            Assert.Equal("transform: rotate(0turn)", style);
        }

        [Fact]
        [TestDescription("Verifies that the rotation is correct after clicking")]
        public void Render_AfterClick_RotationCorrect()
        {
            Services.AddMudServices();

            // Act
            var cut = RenderComponent<DockToggle>(parameters =>
            {
                parameters.Add(p => p.Dock, Dock.Left);
            });

            // Assert
            var element = cut.Find(".dock-toggle");
            Assert.NotNull(element);

            element.Click();
            var style = element.GetAttribute("style");
            Assert.NotNull(style);
            Assert.Equal("transform: rotate(0.25turn)", style);
        }

        [Fact]
        [TestDescription("Verifies that the rotation changes from left to top after clicking once")]
        public void Click_WhenAllAllowed_ValueIsTop()
        {
            Services.AddMudServices();
            Dock dock = Dock.Left;
            Dock allowed = Dock.Left | Dock.Right | Dock.Top | Dock.Bottom;
            var cut = RenderComponent<DockToggle>(parameters =>
            {
                parameters.Add(p => p.Allowed, allowed);
                parameters.Add(p => p.Dock, dock);
                parameters.Add(p => p.DockChanged, (d) => dock = d);
            });

            // Act
            var element = cut.Find(".dock-toggle");
            Assert.NotNull(element);
            element.Click();

            // Assert
            Assert.Equal(Dock.Top, dock);  
        }

        [Fact]
        [TestDescription("Verifies that the Dock value changes from Left > Top > Right > Bottom > Left clicking several times when all directions are allowed")]
        public void ClickFourTimes_WhenAllAllowed_ValueChangesToAllValid()
        {
            Services.AddMudServices();
            Dock dock = Dock.Left;
            Dock allowed = Dock.Left | Dock.Right | Dock.Top | Dock.Bottom;
            var cut = RenderComponent<DockToggle>(parameters =>
            {
                parameters.Add(p => p.Allowed, allowed);
                parameters.Add(p => p.Dock, dock);
                parameters.Add(p => p.DockChanged, (d) => dock = d);
            });

            // Act
            var element = cut.Find(".dock-toggle");
            Assert.NotNull(element);
            element.Click();
            Assert.Equal(Dock.Top, dock);

            element.Click();
            Assert.Equal(Dock.Right, dock);

            element.Click();
            Assert.Equal(Dock.Bottom, dock);

            element.Click();
            Assert.Equal(Dock.Left, dock);

        }

        [Fact]
        [TestDescription("Verifies that the Dock value changes from Left > Right > Left > Right clicking several times when Top and Bottom is not allowed")]
        public void ClickFourTimes_WhenAllTopAndBottomNotAllowed_ValueChangesToAllValid()
        {
            Services.AddMudServices();
            Dock dock = Dock.Left;
            Dock allowed = Dock.Left | Dock.Right;
            var cut = RenderComponent<DockToggle>(parameters =>
            {
                parameters.Add(p => p.Allowed, allowed);
                parameters.Add(p => p.Dock, dock);
                parameters.Add(p => p.DockChanged, (d) => dock = d);
            });

            // Act
            var element = cut.Find(".dock-toggle");
            Assert.NotNull(element);

            element.Click();
            Assert.Equal(Dock.Right, dock);

            element.Click();
            Assert.Equal(Dock.Left, dock);

            element.Click();
            Assert.Equal(Dock.Right, dock);

            element.Click();
            Assert.Equal(Dock.Left, dock);
        }

        [Fact]
        [TestDescription("Verifies that the Dock value changes from Left > Right > Bottom > Left clicking several times when Top is not allowed")]
        public void ClickThreeTimes_WhenAllTopNotAllowed_ValueChangesToAllValid()
        {
            Services.AddMudServices();
            Dock dock = Dock.Left;
            Dock allowed = Dock.Left | Dock.Right | Dock.Bottom;
            var cut = RenderComponent<DockToggle>(parameters =>
            {
                parameters.Add(p => p.Allowed, allowed);
                parameters.Add(p => p.Dock, dock);
                parameters.Add(p => p.DockChanged, (d) => dock = d);
            });

            // Act
            var element = cut.Find(".dock-toggle");
            Assert.NotNull(element);

            element.Click();
            Assert.Equal(Dock.Right, dock);

            element.Click();
            Assert.Equal(Dock.Bottom, dock);

            element.Click();
            Assert.Equal(Dock.Left, dock);
        }

        [Fact]
        [TestDescription("Verifies that the rotation changes from left to right after clicking once when the allowed rotation is Left|Right")]
        public void Click_WhenLeftWithLeftAndRightAllowed_ValueIsRight()
        {
            Services.AddMudServices();
            Dock dock = Dock.Left;
            Dock allowed = Dock.Left | Dock.Right;
            var cut = RenderComponent<DockToggle>(parameters =>
            {
                parameters.Add(p => p.Allowed, allowed);
                parameters.Add(p => p.Dock, dock);
                parameters.Add(p => p.DockChanged, (d) => dock = d);
            });

            // Act
            var element = cut.Find(".dock-toggle");
            Assert.NotNull(element);
            element.Click();

            // Assert
            Assert.Equal(Dock.Right, dock);
        }
    }
}

