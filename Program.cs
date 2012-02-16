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
        const string programName = "imgres.exe";
        static string inputDir = "";
        static string outputDir = "";
        static int width = 100;
        static int height = 100;
        static int quality = 90;
        static bool verbose = false;
        static bool showHelp = false;

        static bool qualityParsed = true;
        static bool widthParsed = true;
        static bool heightParsed = true;

        static void Main(string[] args)
        {
            var options = DefineOptions();

            List<string> extra;
            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("{0}: ", programName);
                Console.WriteLine(e.Message);
                Console.WriteLine("Try '{0} --help' for more information.", programName);
                return;
            }

            if (!ValidateOptions())
            {
                ShowHelp(options);
                return;
            }

            if (showHelp)
            {
                ShowHelp(options);
                return;
            }

            string[] files = Directory.GetFiles(inputDir, "*.jpg");

            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            EncoderParameters encParams = new EncoderParameters(1);
            encParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            if (verbose)
            {
                Console.WriteLine("number of files: {0}", files.Length);
                Console.WriteLine("quality: {0}", quality);
                Console.WriteLine("width:   {0}", width);
                Console.WriteLine("height:  {0}", height);
            }

            foreach (var file in files)
            {
                using (Bitmap resized = Image.FromFile(file).Scale(new Size(width, height)))
                {
                    string fileName = Path.Combine(outputDir, Path.GetFileName(file));
                    resized.Save(fileName, jgpEncoder, encParams);

                    if (verbose)
                    {
                        FileInfo fileInfo = new FileInfo(fileName);
                        Console.WriteLine("{0} {1} KB",
                            fileInfo.Name,
                            Math.Round((decimal)fileInfo.Length / 1024m, 2).ToString());
                    }
                }
            }
        }

        private static OptionSet DefineOptions()
        {
            return new OptionSet
            {
                {
                    "i|input=",
                    "input directory",
                    x => inputDir = x
                },
                {
                    "o|output=",
                    "output directory",
                    x => outputDir = x
                },
                {
                    "q|quality=",
                    "quality of scaled images [0-100]",
                    x => 
                    {
                        int temp;
                        if (int.TryParse(x, out temp) && temp > 0 && temp <= 100)
                            quality = temp;
                        else
                            qualityParsed = false;
                    }
                },
                {
                    "w|width=",
                    "width of scaled images",
                    x => 
                    {
                        int temp;
                        if(int.TryParse(x, out temp) && temp > 0)
                            width = temp;
                        else
                            widthParsed = false;
                    }
                },
                {
                    "h|height=",
                    "height of scaled images",
                    x =>
                    {
                        int temp;
                        if (int.TryParse(x, out temp) && temp > 0)
                            height = temp;
                        else
                            heightParsed = false;
                    }
                },
                {
                    "v|verbose",
                    "verbose mode",
                    x => verbose = x != null
                },
                {
                    "help",
                    "show help",
                    x => showHelp = x != null
                }
            };
        }

        private static bool ValidateOptions()
        {
            bool result = true;

            if (string.IsNullOrEmpty(inputDir))
            {
                ShowError("Please specify input directory.");
                result = false;
            }

            if (string.IsNullOrEmpty(outputDir))
            {
                ShowError("Please specify output directory.");
                result = false;
            }

            if (!Directory.Exists(inputDir))
            {
                ShowError("Input directory does not exist.");
                result = false;
            }

            if (!Directory.Exists(outputDir))
            {
                ShowError("Output directory does not exist.");
                result = false;
            }

            if (!qualityParsed)
            {
                ShowError("Quality must be a integer between 0 and 100.");
                result = false;
            }

            if (!heightParsed)
            {
                ShowError("Height must be a positive integer.");
                result = false;
            }

            if (!widthParsed)
            {
                ShowError("Width must be a positive integer.");
                result = false;
            }

            return result;
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: {0} [OPTIONS]", programName);
            Console.WriteLine("Resizes all .jpg images within given directory.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("ERROR: {0}", message);
            Console.ResetColor();
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
