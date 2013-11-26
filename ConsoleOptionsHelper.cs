using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using NDesk.Options;

namespace ImgResize
{
    public static class ConsoleOptionsHelper
    {
        public static OptionSet DefineOptions(Settings settings)
        {
            return new OptionSet
            {
                {
                    "i|input=",
                    "{PATH} to input directory",
                    x => settings.InputDir = x
                },
                {
                    "o|output=",
                    "{PATH} to output directory",
                    x => settings.OutputDir = x
                },
                {
                    "q|quality=",
                    "quality of scaled images {0-100}. Default is 90.",
                    x => 
                    {
                        int temp;
                        settings.QualityError = !int.TryParse(x, out temp) && temp > 0 && temp <= 100;
                        settings.Quality = temp;
                    }
                },
                {
                    "w|width=",
                    "width {INTEGER} of scaled images",
                    x => 
                    {
                        int temp;
                        settings.WidthError = !int.TryParse(x, out temp) && temp > 0;
                        settings.Width = temp;
                    }
                },
                {
                    "h|height=",
                    "height {INTEGER} of scaled images",
                    x =>
                    {
                        int temp;
                        settings.HeightError = !int.TryParse(x, out temp) && temp > 0;
                        settings.Height = temp;
                    }
                },
                {
                    "l|logo=",
                    "{PATH} to the logo image to be drawn on every transformed image",
                    x => settings.LogoPath = x
                },
                {
                    "t|text=",
                    "custom {TEXT} to be drawn on every transformed image",
                    x => settings.CustomString = x
                },
                {
                    "c|corner=",
                    "{CORNER} of the image in which logo or custom string will be drawn. Default is bottom right. List of valid values: " +
                    "UpperLeft, UpperRight, BottomLeft, BottomRight",
                    x =>
                        {
                            Corner corner;
                            Corner.TryParse(x, true, out corner);
                            settings.Corner = corner;
                        }
                },
                {
                    "ff|FontFamily=",
                    "custom text font family {NAME}",
                    x => settings.FontFamily = x
                },
                {
                    "fc|FontColor=",
                    "custom text font color. Can be a name or in {alpha;red;green;blue} format: [0-254;0-254;0-254;0-254].",
                    x => settings.FontColor = x
                },
                {
                    "fs|FontSize=",
                    "custom text font {SIZE}",
                    x =>
                    {
                        int temp;
                        settings.FontSizeError = int.TryParse(x, out temp) && temp > 0;
                        settings.FontSize = temp;
                    }
                },
                {
                    "v|verbose",
                    "verbose mode",
                    x => settings.Verbose = x != null
                },
                {
                    "help",
                    "show help",
                    x => settings.ShowHelp = x != null
                }
            };
        }
    }
}
