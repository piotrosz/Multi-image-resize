using Xunit;

namespace ImgResize.Test
{
    public class SettingsValidatorFacts
    {
        public class ValidateMethod
        {
            [Fact]
            public void validation_should_fail_for_no_options()
            {
                // Arrange
                var settings = new Settings();
                var target = new SettingsValidator(settings);

                // Act
                var result = target.Validate();

                // Assert
                Assert.NotNull(result.ErrorMessages);
                Assert.Equal(3, result.ErrorMessages.Count);
            }

            [Fact]
            public void validation_should_pass_for_minimal_arguments()
            {
                // Arrange
                var settings = new Settings
                    {
                        InputDir = "C:\\",
                        OutputDir = "C:\\",
                        Width = 120
                    };
                var target = new SettingsValidator(settings);

                // Act
                var result = target.Validate();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(0, result.ErrorMessages.Count);
            }
        }
    }
}
