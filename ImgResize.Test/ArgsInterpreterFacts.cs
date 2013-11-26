using Xunit;

namespace ImgResize.Test
{
    public class ArgsInterpreterFacts
    {
        public class GetSettingsMethod
        {
            [Fact]
            public void should_return_null_for_show_help_option()
            {
                // Arrange
                var args = new[] {"--help"};

                // Act
                var result = ArgsInterpreter.GetSettings(args);

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void should_set_width_error_for_invalid_width_argument()
            {
                // Arrange
                var args = new[] { "-i", "test1", "-o", "test2", "-w", "-90" };

                // Act
                var result = ArgsInterpreter.GetSettings(args);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.WidthError);
                Assert.Equal("test1", result.InputDir);
                Assert.Equal("test2", result.OutputDir);
            }

            [Fact]
            public void should_set_quality_error_for_invalid_quality()
            {
                // Arrange
                var args = new[] { "-i", "test1", "-o", "test2", "-w", "90", "-q", "110" };

                // Act
                var result = ArgsInterpreter.GetSettings(args);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.QualityError);
            }

            [Fact]
            public void should_set_font_size_error_for_invalid_font_size_argument()
            {
                // Arrange
                var args = new[] { "-i", "test1", "-o", "test2", "-w", "90", "-fs", "asc" };

                // Act
                var result = ArgsInterpreter.GetSettings(args);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.FontSizeError);
                Assert.Equal("test1", result.InputDir);
                Assert.Equal("test2", result.OutputDir);
            }

            [Fact]
            public void should_return_settings_for_minimal_options()
            {
                // Arrange
                var args = new[] { "-i", "test1", "-o", "test2", "-w", "190" };

                // Act
                var result = ArgsInterpreter.GetSettings(args);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(190, result.Width);
                Assert.Equal(false, result.WidthError);
                Assert.Equal("test1", result.InputDir);
                Assert.Equal("test2", result.OutputDir);
                Assert.False(result.FontSizeError);
                Assert.False(result.HeightError);
                Assert.False(result.QualityError);
                Assert.False(result.WidthError);
                Assert.False(result.ShowHelp);
                Assert.False(result.Verbose);
            }
        }
    }
}
