using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace ImgResize
{
    public static class Extensions
    {
        public static Bitmap Scale(this Image image, Size size)
        {
            int sourceWidth = image.Width;
            int sourceHeight = image.Height;

            float nPercentW = (size.Width / (float)sourceWidth);
            float nPercentH = (size.Height / (float)sourceHeight);

            float nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;

            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);

            var bitmapDest = new Bitmap(destWidth, destHeight, image.PixelFormat);

            bitmapDest.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(bitmapDest))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(image, 0, 0, bitmapDest.Width, bitmapDest.Height);
            }

            return bitmapDest;
        }

        public static void DrawLogo(this Image image, Image logo, Corner corner)
        {
            if (logo.Width > image.Width || logo.Height > image.Height)
                throw new ArgumentException("Logo image must be smaller than the original image.");

            using (var graphics = Graphics.FromImage(image))
            {
                Point point = new Point();

                switch (corner)
                {
                    case Corner.UpperLeft:
                        point.X = 0;
                        point.Y = 0;
                        break;
                    case Corner.UpperRight:
                        point.X = image.Width - logo.Width;
                        point.Y = 0;
                        break;
                    case Corner.BottomLeft:
                        point.X = 0;
                        point.Y = image.Height - logo.Height;
                        break;
                    case Corner.BottomRight:
                        point.X = image.Width - logo.Width;
                        point.Y = image.Height - logo.Height;
                        break;
                    default:
                        break;
                }

                graphics.DrawImage(logo, point);
            }
        }

        public static void DrawString(this Image image, string text, Corner corner, Font font, Color color)
        {
            using (var graphics = Graphics.FromImage(image))
            {
                StringFormat stringFormat = new StringFormat();
                stringFormat.SetMeasurableCharacterRanges(new CharacterRange[] { new CharacterRange(0, text.Length) });
                Region[] r = graphics.MeasureCharacterRanges(text, font, new Rectangle(0, 0, image.Width, image.Height), stringFormat);
                RectangleF rect = r[0].GetBounds(graphics);
                rect.Width += (int)Math.Ceiling((double)rect.Width * 0.05d);

                PointF point = new PointF();
                switch (corner)
                {
                    case Corner.UpperLeft:
                        point.X = 0;
                        point.Y = 0;
                        break;
                    case Corner.UpperRight:
                        point.X = image.Width - rect.Width;
                        point.Y = 0;
                        break;
                    case Corner.BottomLeft:
                        point.X = 0;
                        point.Y = image.Height - rect.Height;
                        break;
                    case Corner.BottomRight:
                        point.X = image.Width - rect.Width;
                        point.Y = image.Height - rect.Height;
                        break;
                    default:
                        break;
                }

                graphics.DrawString(text, font, new SolidBrush(color), point);
            }
        }

        public static string HumanReadableLength(this FileInfo fileInfo)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            long length = fileInfo.Length;
            while (length >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                length = length / 1024;
            }
            return string.Format("{0:0.##} {1}", length, sizes[order]);
        }
    }

    public enum Corner
    {
        UpperLeft,
        UpperRight,
        BottomLeft,
        BottomRight
    }
}
