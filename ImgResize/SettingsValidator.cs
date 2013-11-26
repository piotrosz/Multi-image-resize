using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace ImgResize
{
    public class SettingsValidator
    {
        private readonly Settings _settings;
        private ValidationResult _result;

        public SettingsValidator(Settings settings)
        {
            this._settings = settings;
        }

        public ValidationResult Validate()
        {
            _result = new ValidationResult();

            if (_settings.Width == 0 
                && _settings.Height == 0
                && string.IsNullOrWhiteSpace(_settings.LogoPath)
                && string.IsNullOrWhiteSpace(_settings.CustomString))
            {
                _result.AddErrorMessage("I've got nothing to do. Please specify some transformation.");
            }

            ValidateInputDir();
            ValidateOutputDir();
            ValidateSize();
            ValidateQuality();
            ValidateLogoPath();
            ValidateCorner();
            ValidateFontSize();
            ValidateFontFamily();
            ValidateFontColor();

            return _result;
        }

        private void ValidateQuality()
        {
            if (_settings.QualityError)
            {
                _result.AddErrorMessage("Quality must be a integer between 0 and 100.");
            }
        }

        private void ValidateInputDir()
        {
            if (string.IsNullOrEmpty(_settings.InputDir))
            {
                _result.AddErrorMessage("Please specify input directory.");
            }
            else if (!Directory.Exists(_settings.InputDir))
            {
                _result.AddErrorMessage("Specified input directory does not exist.");
            }
        }

        private void ValidateOutputDir()
        {
            if (string.IsNullOrEmpty(_settings.OutputDir))
            {
                _result.AddErrorMessage("Please specify output directory.");
            }
            else if (!Directory.Exists(_settings.OutputDir))
            {
                _result.AddErrorMessage("Specified output directory does not exist.");
            }
        }

        private void ValidateSize()
        {
            if (!_settings.HeightError || !_settings.WidthError)
            {
                return;
            }

            if (_settings.HeightError)
            {
                _result.AddErrorMessage("Height must be a positive integer.");
            }

            if (_settings.WidthError)
            {
                _result.AddErrorMessage("Width must be a positive integer.");
            }
        }

        private void ValidateLogoPath()
        {
            if (!string.IsNullOrWhiteSpace(_settings.LogoPath) && !File.Exists(_settings.LogoPath))
            {
                _result.AddErrorMessage("Specified logo file does not exist.");
            }

            if (!string.IsNullOrWhiteSpace(_settings.LogoPath) && File.Exists(_settings.LogoPath))
            {
                var fileInfo = new FileInfo(_settings.LogoPath);
                if (fileInfo.Extension != ".jpg")
                {
                    _result.AddErrorMessage("Logo image must be a jpg file.");
                }
            }
        }

        private void ValidateCorner()
        {
            if (!Enum.GetNames(typeof(Corner)).Contains(_settings.Corner.ToString()))
            {
                _result.AddErrorMessage("Incorrect corner option.");
            }
        }

        private void ValidateFontSize()
        {
            if (_settings.FontSizeError)
            {
                _result.AddErrorMessage("Font size must be a positive integer.");
            }
            else
            {
                if (_settings.FontSize != 0)
                {
                    try
                    {
                        _settings.Font = new Font(_settings.FontFamily, _settings.FontSize);
                    }
                    catch (Exception ex)
                    {
                        _result.AddErrorMessage("Cannot parse font size");
                        _result.AddErrorMessage(ex.Message);
                    }
                }
            }
        }

        private void ValidateFontFamily()
        {
            if (string.IsNullOrWhiteSpace(_settings.FontFamily))
            {
                return;
            }

            try
            {
                _settings.Font = new Font(_settings.FontFamily, _settings.Font.Size);
            }
            catch (Exception ex)
            {
                _result.AddErrorMessage("Cannot parse font family.");
                _result.AddErrorMessage(ex.Message);
            }
        }

        private void ValidateFontColor()
        {
            if (string.IsNullOrWhiteSpace(_settings.FontColor))
            {
                return;
            }

            string[] rgb = _settings.FontColor.Split(';');
            if (rgb.Length != 4)
            {
                try
                {
                    _settings.Color = Color.FromName(_settings.FontColor);
                }
                catch (Exception ex)
                {
                    _result.AddErrorMessage("Cannot parse font color.");
                    _result.AddErrorMessage(ex.Message);
                }
            }
            else
            {
                int a, r, g, b;

                if (!int.TryParse(rgb[0], out a) ||
                    !int.TryParse(rgb[1], out r) ||
                    !int.TryParse(rgb[2], out g) ||
                    !int.TryParse(rgb[3], out b))
                {
                    _result.AddErrorMessage("Cannot parse font color.");
                }
                else
                {
                    if (a < 0 || a > 254 ||
                        r < 0 || r > 254 ||
                        g < 0 || g > 254 ||
                        b < 0 || g > 254)
                    {
                        _result.AddErrorMessage("Wrong argb values.");
                    }
                    else
                    {
                        _settings.Color = Color.FromArgb(a, r, g, b);
                    }
                }
            }
        }
    }
}
