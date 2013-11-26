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
        const string ProgramName = "imgres.exe";
        
        static void Main(string[] args)
        {
            Settings settings = CollectSettings(args);

            if (settings == null)
            {
                return;
            }

            string[] files = Directory.GetFiles(settings.InputDir, "*.jpg");

            if (settings.Verbose)
            {
                Console.WriteLine("{0,-20}{1,20}", "number of files", files.Length);
                Console.WriteLine("{0,-20}{1,20}", "quality", settings.Quality);
                Console.WriteLine("{0,-20}{1,20}", "width", settings.Width);
                Console.WriteLine("{0,-20}{1,20}", "height", settings.Height);
            }

            var imageProcessor = new ImageProcessor();

            foreach (var file in imageProcessor.ProcessFiles(files, settings))
            {
                if (!string.IsNullOrEmpty(file.ErrorMessage))
                {
                    ShowError(file.ErrorMessage);
                }
                else if (settings.Verbose)
                {
                    Console.WriteLine("{0,-20}{1,20}",
                        file.FileInfo.Name,
                        file.FileInfo.GetHumanReadableLength());
                }
            }
        }

        private static Settings CollectSettings(string[] args)
        {
            var settings = new Settings();
            var options = ConsoleOptionsHelper.DefineOptions(settings);

            try
            {
                options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("{0}: ", ProgramName);
                Console.WriteLine(e.Message);
                Console.WriteLine("Try '{0} --help' for more information.", ProgramName);
                return null;
            }

            if (settings.ShowHelp)
            {
                ShowHelp(options);
                return null;
            }

            if (!ValidateSettings(settings, options))
            {
                return null;
            }

            return settings;
        }

        private static bool ValidateSettings(Settings settings, OptionSet optionSet)
        {
            var validator = new SettingsValidator(settings);

            var validationResult = validator.Validate();

            if (validationResult.ErrorMessages.Any())
            {
                validationResult.ErrorMessages.ForEach(ShowError);
                return false;
            }

            return true;
        }

        private static void ShowHelp(OptionSet optionSet)
        {
            Console.WriteLine("Usage: {0} [OPTIONS]", ProgramName);
            Console.WriteLine("Transformes all .jpg images within given directory.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            optionSet.WriteOptionDescriptions(Console.Out);
        }

        private static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("ERROR: {0}", message);
            Console.ResetColor();
        }
    }
}
