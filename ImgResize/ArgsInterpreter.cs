using NDesk.Options;

namespace ImgResize
{
    public static class ArgsInterpreter
    {
        public static Settings GetSettings(string[] args)
        {
            var settings = new Settings();
            var options = ConsoleOptions.DefineOptions(settings);

            try
            {
                options.Parse(args);
            }
            catch (OptionException e)
            {
                ConsoleWriter.WriteOptionsException(e);
                return null;
            }

            if (settings.ShowHelp)
            {
                ConsoleWriter.ShowHelp(options);
                return null;
            }

            return settings;
        }
    }
}
