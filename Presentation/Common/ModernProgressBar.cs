using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Futtage.Presentation.Common
{
    public class ModernProgressBar : Control
    {
        private int _value = 0;
        private int _maximum = 100;
        private Color _progressColor = Color.FromArgb(0, 122, 255);
        private Color _backgroundColor = Color.FromArgb(240, 240, 240);
        private int _borderRadius = 10;
        private string _text = string.Empty;
        private bool _showPercentage = true;

        public int Value
        {
            get => _value;
            set
            {
                _value = Math.Max(0, Math.Min(_maximum, value));
                if (_showPercentage)
                    _text = $"{(_value * 100) / _maximum}%";
                Invalidate();
            }
        }

        public int Maximum
        {
            get => _maximum;
            set
            {
                _maximum = Math.Max(1, value);
                Value = _value; // Revalidar valor atual
            }
        }

        public Color ProgressColor
        {
            get => _progressColor;
            set { _progressColor = value; Invalidate(); }
        }

        public Color ProgressBackgroundColor
        {
            get => _backgroundColor;
            set { _backgroundColor = value; Invalidate(); }
        }

        public bool ShowPercentage
        {
            get => _showPercentage;
            set { _showPercentage = value; Invalidate(); }
        }

        public string ProgressText
        {
            get => _text;
            set { _text = value; Invalidate(); }
        }

        public ModernProgressBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            Size = new Size(200, 25);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width, Height);

            // Desenhar fundo
            using (var backgroundBrush = new SolidBrush(_backgroundColor))
            using (var backgroundPath = GetRoundedRectangle(rect, _borderRadius))
            {
                g.FillPath(backgroundBrush, backgroundPath);
            }

            // Calcular largura do progresso
            int progressWidth = (int)((double)_value / _maximum * Width);

            if (progressWidth > 0)
            {
                var progressRect = new Rectangle(0, 0, progressWidth, Height);

                // Desenhar progresso com gradiente
                using (var progressBrush = new LinearGradientBrush(
                    progressRect,
                    _progressColor,
                    Color.FromArgb(Math.Min(255, _progressColor.R + 30),
                                  Math.Min(255, _progressColor.G + 30),
                                  Math.Min(255, _progressColor.B + 30)),
                    LinearGradientMode.Vertical))
                using (var progressPath = GetRoundedRectangle(progressRect, _borderRadius))
                {
                    g.FillPath(progressBrush, progressPath);
                }
            }

            // Desenhar texto
            if (!string.IsNullOrEmpty(_text))
            {
                using (var textBrush = new SolidBrush(ForeColor))
                {
                    var textSize = g.MeasureString(_text, Font);
                    var textRect = new PointF(
                        (Width - textSize.Width) / 2,
                        (Height - textSize.Height) / 2
                    );

                    g.DrawString(_text, Font, textBrush, textRect);
                }
            }
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}