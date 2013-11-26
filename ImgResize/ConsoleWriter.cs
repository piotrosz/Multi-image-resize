using System;
using NDesk.Options;

namespace ImgResize
{
    public static class ConsoleWriter
    {
        const string ProgramName = "imgres.exe";

        public static void WriteSettingsInfo(Settings settings, string[] files)
        {
            if (!settings.Verbose) return;
            Console.WriteLine("{0,-20}{1,20}", "Number of files", files.Length);
            Console.WriteLine("{0,-20}{1,20}", "Quality", settings.Quality);
            Console.WriteLine("{0,-20}{1,20}", "Width", settings.Width);
            Console.WriteLine("{0,-20}{1,20}", "Height", settings.Height);
        }

        public static void WriteProcessFileResult(ProcessFileResult processFileResult, Settings settings)
        {
            if (!string.IsNullOrEmpty(processFileResult.ErrorMessage))
            {
                ShowError(processFileResult.ErrorMessage);
            }
            else if (settings.Verbose)
            {
                Console.WriteLine("{0,-20}{1,20}",
                    processFileResult.FileInfo.Name,
                    processFileResult.FileInfo.GetHumanReadableLength());
            }
        }

        public static void WriteOptionsException(Exception exception)
        {
            Console.Write("{0}: ", ProgramName);
            ShowError(exception.Message);
            Console.WriteLine("Try '{0} --help' for more information.", ProgramName);
        }

        public static void ShowHelp(OptionSet optionSet)
        {
            Console.WriteLine("Usage: {0} [OPTIONS]", ProgramName);
            Console.WriteLine("Transformes all .jpg images within given directory.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            optionSet.WriteOptionDescriptions(Console.Out);
        }

        public static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: {0}", message);
            Console.ResetColor();
        }
    }
}
