using System.Drawing;

namespace ImgResize
{
    public class Settings
    {
        public Settings()
        {
            Quality = 90;
            Corner = Corner.BottomLeft;
            Font = new Font("Arial", 12);
            Color = Color.LightGray;
        }

        public string InputDir { get; set; }
        public string OutputDir { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Quality { get; set; }
        public bool Verbose { get; set; }
        public bool ShowHelp { get; set; }
        public string LogoPath { get; set; }
        public string CustomString { get; set; }
        public Corner Corner { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public string FontColor { get; set; }
        public Font Font { get; set; }
        public Color Color { get; set; }

        public bool QualityError { get; set; }
        public bool WidthError { get; set; }
        public bool HeightError { get; set; }
        public bool FontSizeError { get; set; }

        // TODO: Option to overwrite files is not implemented yet
        public bool OverwriteFiles { get; set; }

        public Size Size { get { return new Size(Width, Height); } }
    }
}
