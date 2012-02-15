using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImgResize
{
    class Program
    {
        static void Main(string [] args)
        {
            string inputDir = @"F:\images\";
            string outputDir = @"F:\images\temp\"; // directory must exist!
            string[] files = Directory.GetFiles(inputDir, "*.jpg");
            int width = 200;
            int height = 200;
            int quality = 90;

            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            EncoderParameters encParams = new EncoderParameters(1);
            encParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            foreach (var file in files)
            {
                using (Bitmap resized = Image.FromFile(file).Scale(new Size(width, height)))
                {
                    string fileName = Path.Combine(outputDir, Path.GetFileName(file));
                    resized.Save(fileName, jgpEncoder, encParams);

                    FileInfo fInfo = new FileInfo(fileName);
                    Console.WriteLine("{0} {1} KB", 
                        fInfo.Name,
                        Math.Round((decimal)fInfo.Length / 1024m, 2).ToString());
                }
            }
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
