using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Textify_Editor.Forms
{
    public class RoundedPanel : Panel
    {
        private Color _fillColor = Color.FromArgb(45, 45, 48);
        private Color _borderColor = Color.FromArgb(70, 70, 74);
        private int _cornerRadius = 20;

        public RoundedPanel()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            
        }

        public Color FillColor
        {
            get { return _fillColor; }
            set { _fillColor = value; Invalidate(); }
        }

        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; Invalidate(); }
        }

        public int CornerRadius
        {
            get { return _cornerRadius; }
            set { _cornerRadius = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Width <= 1 || Height <= 1)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Width-1 / Height-1 so the 1px stroke stays inside the control bounds.
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            using (var path = CreatePath(rect, _cornerRadius))
            using (var fill = new SolidBrush(_fillColor))
            using (var pen = new Pen(_borderColor, 1f))
            {
                e.Graphics.FillPath(fill, path);
                e.Graphics.DrawPath(pen, path);
            }
        }

        private static GraphicsPath CreatePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = System.Math.Min(radius * 2, System.Math.Min(rect.Width, rect.Height));

            if (d <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}