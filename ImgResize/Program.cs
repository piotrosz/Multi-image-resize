using System.Linq;
using System.IO;

namespace ImgResize
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings settings = ArgsInterpreter.GetSettings(args);

            if (settings == null)
            {
                return;
            }

            if (!ValidateSettings(settings))
            {
                return;
            }

            string[] files = Directory.GetFiles(settings.InputDir, "*.jpg");
            ConsoleWriter.WriteSettingsInfo(settings, files);

            foreach (var processFileResult in new ImageProcessor().ProcessFiles(files, settings))
            {
                ConsoleWriter.WriteProcessFileResult(processFileResult, settings);
            }
        }

        private static bool ValidateSettings(Settings settings)
        {
            var validator = new SettingsValidator(settings);

            var validationResult = validator.Validate();

            if (validationResult.ErrorMessages.Any())
            {
                validationResult.ErrorMessages.ForEach(ConsoleWriter.ShowError);
                return false;
            }

            return true;
        } 
    }
}
