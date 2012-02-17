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
        static int width = 0;
        static int height = 0;
        static int quality = 90;
        static bool verbose = false;
        static bool showHelp = false;
        static string logoPath = "";
        static string customString = "";
        static string corner = "br";
        static string fontFamily = "";
        static int fontSize = 0;
        static string fontColor = "";
        static Font font = new Font("Arial", 12);
        static Color color = Color.LightGray;

        static bool qualityParsed = true;
        static bool widthParsed = true;
        static bool heightParsed = true;
        static bool fontSizeParsed = true;

        static bool overwriteFiles = false;

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

            if (showHelp)
            {
                ShowHelp(options);
                return;
            }

            if (!ValidateOptions())
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
                Console.WriteLine("number of files...{0}", files.Length);
                Console.WriteLine("quality...........{0}", quality);
                Console.WriteLine("width.............{0}", width);
                Console.WriteLine("height............{0}", height);
            }

            foreach (var file in files)
            {
                using (Bitmap original = (Bitmap)Image.FromFile(file))
                {
                    string fileName = Path.Combine(outputDir, Path.GetFileName(file));

                    if (!overwriteFiles && File.Exists(fileName))
                    {
                        Console.WriteLine("Destination file exists. Do you wish to overwrite all conflicting files? [Y/N]");
                        string answer = Console.ReadLine();
                        if (answer.ToLower() == "y")
                            overwriteFiles = true;
                        else
                            return;
                    }

                    Corner cornerEnum = Corner.BottomRight;
                    if (corner == "ul")
                        cornerEnum = Corner.UpperLeft;
                    else if (corner == "ur")
                        cornerEnum = Corner.UpperRight;
                    else if (corner == "bl")
                        cornerEnum = Corner.BottomLeft;
                    else if (corner == "br")
                        cornerEnum = Corner.BottomRight;

                    if (width > 0 && height > 0) // with scaling
                    {
                        using (Bitmap scaled = original.Scale(new Size(width, height)))
                        {
                            if (!string.IsNullOrWhiteSpace(logoPath)) // add logo
                            {
                                using (Image logoImg = Image.FromFile(logoPath))
                                {
                                    scaled.DrawLogo(logoImg, cornerEnum);
                                }
                            }
                            else if (!string.IsNullOrWhiteSpace(customString)) // add text
                            {
                                scaled.DrawString(customString, cornerEnum, font, color);
                            }
                            scaled.Save(fileName, jgpEncoder, encParams);
                        }
                    }
                    else // no scaling
                    {
                        if (!string.IsNullOrWhiteSpace(logoPath))
                        {
                            using (Image logoImg = Image.FromFile(logoPath)) // add logo
                            {
                                original.DrawLogo(logoImg, cornerEnum);
                            }
                        }
                        else // add text
                        {
                            original.DrawString(customString, cornerEnum, font, color);
                        }
                        original.Save(fileName, jgpEncoder, encParams);
                    }

                    if (verbose)
                    {
                        FileInfo fileInfo = new FileInfo(fileName);
                        Console.WriteLine("{0} {1}",
                            fileInfo.Name,
                            fileInfo.HumanReadableLength());
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
                    "{PATH} to input directory",
                    x => inputDir = x
                },
                {
                    "o|output=",
                    "{PATH} to output directory",
                    x => outputDir = x
                },
                {
                    "q|quality=",
                    "quality of scaled images {0-100}. Default is 90.",
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
                    "width {INTEGER} of scaled images",
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
                    "height {INTEGER} of scaled images",
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
                    "l|logo=",
                    "{PATH} to the logo image to be drawn on every transformed image",
                    x => logoPath = x
                },
                {
                    "t|text=",
                    "custom {TEXT} to be drawn on every transformed image",
                    x => customString = x
                },
                {
                    "c|corner=",
                    "{CORNER} of the image in which logo or custom string will be drawn. Default is bottom right. List of valid values:" +
                    "ul - upper left, " +
                    "ur - upper right, " +
                    "bl - bottom left, " +
                    "br - bottom right, ",
                    x => corner = x
                },
                {
                    "ff|FontFamily=",
                    "custom text font family {NAME}",
                    x => fontFamily = x
                },
                {
                    "fc|FontColor=",
                    "custom text font color. Can be a name or in {alpha;red;green;blue} format: [0-254;0-254;0-254;0-254].",
                    x => fontColor = x
                },
                {
                    "fs|FontSize=",
                    "custom text font {SIZE}",
                    x =>
                    {
                        int temp;
                        if (int.TryParse(x, out temp) && temp > 0)
                            fontSize = temp;
                        else
                            fontSizeParsed = false;
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
            else if (!Directory.Exists(inputDir))
            {
                ShowError("Specified input directory does not exist.");
                result = false;
            }

            if (string.IsNullOrEmpty(outputDir))
            {
                ShowError("Please specify output directory.");
                result = false;
            }
            else if (!Directory.Exists(outputDir))
            {
                ShowError("Specified output directory does not exist.");
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

            if ((width == 0 || height == 0)
                && string.IsNullOrWhiteSpace(logoPath)
                && string.IsNullOrWhiteSpace(customString))
            {
                ShowError("I've got nothing to do. Please specify some transformation.");
                result = false;
            }

            if (!string.IsNullOrWhiteSpace(logoPath) && !File.Exists(logoPath))
            {
                ShowError("Specified logo file does not exist.");
                result = false;
            }

            if (!string.IsNullOrWhiteSpace(logoPath) && File.Exists(logoPath))
            {
                FileInfo fileInfo = new FileInfo(logoPath);
                if (fileInfo.Extension != ".jpg")
                {
                    ShowError("Logo image must be a jpg file.");
                    result = false;
                }
            }

            if (!(corner == "ul" || corner == "ur" || corner == "bl" || corner == "br"))
            {
                ShowError("Incorrect corner option.");
                result = false;
            }

            if (!fontSizeParsed)
            {
                ShowError("Font size must be a positive integer.");
                result = false;
            }
            else
            {
                if (fontSize != 0)
                {
                    try
                    {
                        font = new Font(fontFamily, fontSize);
                    }
                    catch (Exception ex)
                    {
                        ShowError("Cannot parse font size");
                        ShowError(ex.Message);
                        result = false;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(fontFamily))
            {
                try
                {
                    font = new Font(fontFamily, font.Size);
                }
                catch (Exception ex)
                {
                    ShowError("Cannot parse font family.");
                    ShowError(ex.Message);
                    result = false;
                }
            }

            if (!string.IsNullOrWhiteSpace(fontColor))
            {
                string[] rgb = fontColor.Split(';');
                if (rgb.Length != 4)
                {
                    try
                    {
                        color = Color.FromName(fontColor);
                    }
                    catch (Exception ex)
                    {
                        ShowError("Cannot parse font color.");
                        ShowError(ex.Message);
                        result = false;
                    }
                }
                else
                {
                    int a = 0;
                    int r = 0;
                    int g = 0;
                    int b = 0;

                    if (!int.TryParse(rgb[0], out a) ||
                        !int.TryParse(rgb[1], out r) ||
                        !int.TryParse(rgb[2], out g) ||
                        !int.TryParse(rgb[3], out b))
                    {
                        ShowError("Cannot parse font color.");
                        result = false;
                    }
                    else
                    {
                        if (a < 0 || a > 254 ||
                            r < 0 || r > 254 ||
                            g < 0 || g > 254 ||
                            b < 0 || g > 254)
                        {
                            ShowError("Wrong argb values.");
                            result = false;
                        }
                        else
                        {
                            color = Color.FromArgb(a, r, g, b);
                        }
                    }

                }
            }

            return result;
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: {0} [OPTIONS]", programName);
            Console.WriteLine("Transformes all .jpg images within given directory.");
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
