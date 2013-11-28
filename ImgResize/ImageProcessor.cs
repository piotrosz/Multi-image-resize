using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Encoder = System.Drawing.Imaging.Encoder;

namespace ImgResize
{
    public class ImageProcessor
    {
        public IEnumerable<ProcessFileResult> ProcessFiles(string[] files, Settings settings)
        {
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, settings.Quality);

            foreach (var file in files)
            {
                using (var original = (Bitmap)Image.FromFile(file))
                {
                    string fileName = Path.Combine(settings.OutputDir, Path.GetFileName(file));

                    if (!settings.OverwriteFiles && File.Exists(fileName))
                    {
                        yield return new ProcessFileResult { ErrorMessage = "File name is the same." };
                    }

                    if (settings.Width > 0 || settings.Height > 0)
                    {
                        using (Bitmap scaled = original.Scale(settings.Size))
                        {
                            AddLogoOrText(scaled, settings);
                            scaled.Save(fileName, jgpEncoder, encoderParameters);
                        }
                    }
                    else
                    {
                        AddLogoOrText(original, settings);
                        original.Save(fileName, jgpEncoder, encoderParameters);
                    }

                    yield return new ProcessFileResult {FileInfo = new FileInfo(fileName)};
                }
            }
        }

        private void AddLogoOrText(Bitmap bitmap, Settings settings)
        {
            if (!string.IsNullOrWhiteSpace(settings.LogoPath))
            {
                using (Image logoImg = Image.FromFile(settings.LogoPath))
                {
                    bitmap.DrawLogo(logoImg, settings.Corner);
                }
            }
            
            if (!string.IsNullOrWhiteSpace(settings.CustomString))
            {
                bitmap.DrawString(settings.CustomString, settings.Corner, settings.Font, settings.Color);
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }
    }
}
