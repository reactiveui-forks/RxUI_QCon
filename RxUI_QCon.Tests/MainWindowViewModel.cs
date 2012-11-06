using System;
using Xunit;

namespace RxUI_QCon.Tests
{
    public class MainWindowViewModelTests
    {
        [Fact]
        public void ColorShouldChangeWhenValuesChange()
        {
            var fixture = new MainWindowViewModel();

            fixture.Red = "255";

            Assert.NotNull(fixture.FinalColor);
            Assert.Equal(255, fixture.FinalColor.Color.R);
            Assert.Equal(0, fixture.FinalColor.Color.B);

            fixture.Blue = "300";

            Assert.NotNull(fixture.FinalColor);
            Assert.Equal(255, fixture.FinalColor.Color.R);
            Assert.Equal(0, fixture.FinalColor.Color.B);

            fixture.Blue = "128";

            Assert.NotNull(fixture.FinalColor);
            Assert.Equal(255, fixture.FinalColor.Color.R);
            Assert.Equal(128, fixture.FinalColor.Color.B);
        }
    }
}
