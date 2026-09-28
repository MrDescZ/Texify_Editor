using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class GradientToolStripRenderer : ToolStripProfessionalRenderer
{
    private readonly int _cornerRadius;

    /// <summary>Draws a thin accent line under selected top-level toolbar items.</summary>
    public bool UseAccentUnderline { get; set; } = true;

    public GradientToolStripRenderer() : this(cornerRadius: 4)
    {
    }

    public GradientToolStripRenderer(int cornerRadius) : base(new TextifyColorTable())
    {
        RoundedEdges = true;
        _cornerRadius = Math.Max(0, cornerRadius);
    }

    // ==============================
    // Helpers
    // ==============================
    private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        if (radius <= 0 || bounds.Width <= 0 || bounds.Height <= 0)
        {
            path.AddRectangle(bounds);
            return path;
        }

        int d = radius * 2;
        d = Math.Min(d, Math.Min(bounds.Width, bounds.Height));

        path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
        path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
        path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    private void FillItemBackground(Graphics g, Rectangle bounds, Color top, Color bottom, LinearGradientMode mode, bool rounded)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        using var brush = new LinearGradientBrush(
            new Rectangle(bounds.X, bounds.Y, Math.Max(bounds.Width, 1), Math.Max(bounds.Height, 1)),
            top, bottom, mode);

        if (rounded && _cornerRadius > 0)
        {
            using var path = RoundedRect(bounds, _cornerRadius);
            SmoothingMode old = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillPath(brush, path);
            g.SmoothingMode = old;
        }
        else
        {
            g.FillRectangle(brush, bounds);
        }
    }

    // ==============================
    // ToolStrip / MenuStrip / StatusStrip background
    // ==============================
    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        Rectangle rect = e.AffectedBounds;
        if (rect.Width <= 0 || rect.Height <= 0) return;

        using var brush = new LinearGradientBrush(
            rect,
            Color.FromArgb(32, 32, 38),
            Color.FromArgb(22, 22, 26),
            LinearGradientMode.Vertical);

        e.Graphics.FillRectangle(brush, rect);
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        // Fixed: disabled items previously rendered with the same bright color as enabled ones.
        e.TextColor = e.Item.Enabled ? Color.Gainsboro : Color.FromArgb(110, 110, 118);
        e.TextFont = new Font("Segoe UI", 9f);
        base.OnRenderItemText(e);
    }

    protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
    {
        Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);

        if (e.Item.Pressed)
        {
            FillItemBackground(e.Graphics, rect,
                Color.FromArgb(60, 60, 100), Color.FromArgb(45, 45, 80),
                LinearGradientMode.Vertical, rounded: true);
        }
        else if (e.Item.Selected)
        {
            FillItemBackground(e.Graphics, rect,
                Color.FromArgb(80, 90, 130), Color.FromArgb(55, 65, 110),
                LinearGradientMode.Horizontal, rounded: true);
        }
    }

    // ==============================
    // Menu & toolbar item hover / pressed / checked
    // ==============================
    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
        bool isChecked = (e.Item as ToolStripButton)?.Checked == true;

        if (e.Item.Pressed)
        {
            FillItemBackground(e.Graphics, rect,
                Color.FromArgb(70, 70, 110), Color.FromArgb(50, 50, 90),
                LinearGradientMode.Vertical, rounded: true);
        }
        else if (e.Item.Selected)
        {
            FillItemBackground(e.Graphics, rect,
                Color.FromArgb(90, 100, 150), Color.FromArgb(60, 70, 120),
                LinearGradientMode.Vertical, rounded: true);

            // New: accent underline for top-level toolbar items (not dropdown menu rows).
            if (UseAccentUnderline && !(e.Item.Owner is ToolStripDropDown))
            {
                using var accent = new Pen(Color.FromArgb(140, 160, 230), 2f);
                e.Graphics.DrawLine(accent, rect.Left + 2, rect.Bottom - 1, rect.Right - 2, rect.Bottom - 1);
            }
        }
        else if (isChecked)
        {
            // New: toggled menu items now look visibly different when idle.
            FillItemBackground(e.Graphics, rect,
                Color.FromArgb(55, 60, 90), Color.FromArgb(40, 45, 70),
                LinearGradientMode.Vertical, rounded: true);

            using var accentBrush = new SolidBrush(Color.FromArgb(140, 160, 230));
            e.Graphics.FillRectangle(accentBrush, rect.Left, rect.Top, 3, rect.Height);
        }
    }

    protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
    {
        if (!e.Item.Enabled) return;

        // Fixed: was e.Item.Bounds, which is in the wrong coordinate space here and
        // caused the fill to be drawn offset from the actual button.
        Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
        bool isChecked = (e.Item as ToolStripButton)?.Checked == true;

        if (e.Item.Pressed)
        {
            // New: pressed state was previously unhandled (buttons gave no click feedback).
            FillItemBackground(e.Graphics, rect,
                Color.FromArgb(55, 60, 95), Color.FromArgb(40, 45, 75),
                LinearGradientMode.Vertical, rounded: true);
        }
        else if (e.Item.Selected)
        {
            FillItemBackground(e.Graphics, rect,
                Color.FromArgb(80, 90, 130), Color.FromArgb(50, 60, 100),
                LinearGradientMode.Vertical, rounded: true);
        }
        else if (isChecked)
        {
            // New: toggled buttons now stay visually "on" when the mouse isn't over them.
            FillItemBackground(e.Graphics, rect,
                Color.FromArgb(55, 60, 90), Color.FromArgb(40, 45, 70),
                LinearGradientMode.Vertical, rounded: true);

            using var border = new Pen(Color.FromArgb(140, 160, 230));
            using var path = RoundedRect(Rectangle.Inflate(rect, -1, -1), Math.Max(0, _cornerRadius - 1));
            SmoothingMode old = e.Graphics.SmoothingMode;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.DrawPath(border, path);
            e.Graphics.SmoothingMode = old;
        }
    }

    // ==============================
    // New: submenu / dropdown arrows
    // ==============================
    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
        // Previously fell back to the base renderer's default (near-black) arrow color,
        // which was almost invisible against this dark theme.
        e.ArrowColor = e.Item.Enabled ? Color.Gainsboro : Color.FromArgb(90, 90, 96);
        base.OnRenderArrow(e);
    }

    // ==============================
    // New: overflow ("»") button
    // ==============================
    protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
    {
        Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);

        if (e.Item.Pressed)
        {
            FillItemBackground(e.Graphics, rect,
                Color.FromArgb(60, 60, 100), Color.FromArgb(45, 45, 80),
                LinearGradientMode.Vertical, rounded: false);
        }
        else if (e.Item.Selected)
        {
            FillItemBackground(e.Graphics, rect,
                Color.FromArgb(80, 90, 130), Color.FromArgb(55, 65, 110),
                LinearGradientMode.Vertical, rounded: false);
        }
        else
        {
            FillItemBackground(e.Graphics, rect,
                Color.FromArgb(30, 30, 36), Color.FromArgb(22, 22, 26),
                LinearGradientMode.Vertical, rounded: false);
        }
    }

    // ==============================
    // New: move-handle grip
    // ==============================
    protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
    {
        Rectangle rect = e.GripBounds;
        if (rect.Width <= 0 || rect.Height <= 0) return;

        using var dotBrush = new SolidBrush(Color.FromArgb(90, 90, 100));
        bool vertical = e.GripDisplayStyle == ToolStripGripDisplayStyle.Vertical;

        const int dot = 2;
        const int gap = 3;

        if (vertical)
        {
            int x = rect.X + rect.Width / 2 - dot / 2;
            for (int y = rect.Y + 4; y < rect.Bottom - 4; y += gap + dot)
                e.Graphics.FillEllipse(dotBrush, x, y, dot, dot);
        }
        else
        {
            int y = rect.Y + rect.Height / 2 - dot / 2;
            for (int x = rect.X + 4; x < rect.Right - 4; x += gap + dot)
                e.Graphics.FillEllipse(dotBrush, x, y, dot, dot);
        }
    }

    // ==============================
    // Custom separators (toolbar + menus)
    // ==============================
    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        Rectangle r = e.Item.ContentRectangle;
        using var pen = new Pen(Color.FromArgb(70, 70, 85));

        if (e.Item.Owner is ToolStripDropDown)
        {
            int y = r.Top + r.Height / 2;
            e.Graphics.DrawLine(pen, r.Left + 8, y, r.Right - 8, y);
        }
        else
        {
            int x = r.Left + r.Width / 2;
            e.Graphics.DrawLine(pen, x, r.Top + 4, x, r.Bottom - 4);
        }
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        using var pen = new Pen(Color.FromArgb(51, 51, 55));
        e.Graphics.DrawRectangle(
            pen,
            0,
            0,
            e.ToolStrip.Width - 1,
            e.ToolStrip.Height - 1);
    }
}

internal class TextifyColorTable : ProfessionalColorTable
{
    public override Color ToolStripBorder => Color.FromArgb(45, 45, 55);
    public override Color MenuBorder => Color.FromArgb(45, 45, 55);

    public override Color ToolStripDropDownBackground => Color.FromArgb(28, 28, 34);

    public override Color MenuItemSelected => Color.Transparent;
    public override Color MenuItemBorder => Color.Transparent;

    public override Color SeparatorDark => Color.FromArgb(70, 70, 85);
    public override Color SeparatorLight => Color.FromArgb(30, 30, 36);

    public override Color ImageMarginGradientBegin => Color.FromArgb(26, 26, 32);
    public override Color ImageMarginGradientMiddle => Color.FromArgb(26, 26, 32);
    public override Color ImageMarginGradientEnd => Color.FromArgb(26, 26, 32);
}