using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using NDesk.Options;

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
