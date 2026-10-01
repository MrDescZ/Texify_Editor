using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class GradientStatusStripRenderer : ToolStripProfessionalRenderer
{
    /// <summary>
    /// If true, draws a full rectangle border like a floating toolbar.
    /// If false (default), draws only a top divider line, which reads better
    /// for a StatusStrip docked flush to the bottom of a window.
    /// </summary>
    public bool FullBorder { get; set; } = false;

    /// <summary>Draws a thin vertical divider between status bar panels/labels.</summary>
    public bool ShowPanelSeparators { get; set; } = true;

    public GradientStatusStripRenderer() : base(new TextifyColorTable())
    {
    }

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        Rectangle rect = e.AffectedBounds;
        if (rect.Width <= 0 || rect.Height <= 0) return;

        using var brush = new LinearGradientBrush(
            rect,
            Color.FromArgb(30, 30, 30),   // top
            Color.FromArgb(45, 45, 45),   // bottom
            LinearGradientMode.Vertical);

        e.Graphics.FillRectangle(brush, rect);
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        using var pen = new Pen(Color.FromArgb(51, 51, 55));

        if (FullBorder)
        {
            e.Graphics.DrawRectangle(pen, 0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
        }
        else
        {
            // A rectangle here would draw against the window frame on 3 sides;
            // a top-only divider is the conventional status bar look.
            e.Graphics.DrawLine(pen, 0, 0, e.ToolStrip.Width - 1, 0);
        }
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        bool hasCustomColor = e.Item.ForeColor != SystemColors.ControlText;
        if (!hasCustomColor)
        {
            e.TextColor = e.Item.Enabled ? Color.Gainsboro : Color.FromArgb(120, 120, 126);
        }

        e.TextFont = new Font("Segoe UI", 9f);
        base.OnRenderItemText(e);
    }

    protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
    {
        base.OnRenderLabelBackground(e);

        if (!ShowPanelSeparators) return;
        if (e.Item is not ToolStripStatusLabel) return;

        ToolStrip owner = e.Item.Owner;
        if (owner == null) return;

        int myIndex = owner.Items.IndexOf(e.Item);
        bool isLastVisible = true;
        for (int i = myIndex + 1; i < owner.Items.Count; i++)
        {
            if (owner.Items[i].Visible)
            {
                isLastVisible = false;
                break;
            }
        }
        if (isLastVisible) return;

        Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
        using var pen = new Pen(Color.FromArgb(70, 70, 75));
        e.Graphics.DrawLine(pen, rect.Right - 1, rect.Top + 3, rect.Right - 1, rect.Bottom - 3);
    }

    protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
    {
        if (e.ToolStrip is not StatusStrip statusStrip || !statusStrip.SizingGrip)
            return;

        // StatusStrip doesn't expose its grip rectangle publicly, so compute the
        // conventional bottom corner square ourselves (bottom-left when RTL).
        const int gripSize = 16;
        Rectangle clientRect = statusStrip.ClientRectangle;
        Rectangle grip = statusStrip.RightToLeft == RightToLeft.Yes
            ? new Rectangle(clientRect.Left, clientRect.Bottom - gripSize, gripSize, gripSize)
            : new Rectangle(clientRect.Right - gripSize, clientRect.Bottom - gripSize, gripSize, gripSize);

        if (grip.Width <= 0 || grip.Height <= 0) return;

        using var dotBrush = new SolidBrush(Color.FromArgb(100, 100, 108));

        const int dot = 2;
        const int spacing = 4;
        const int rows = 3;

        for (int row = 0; row < rows; row++)
        {
            int dotsInRow = rows - row;
            for (int col = 0; col < dotsInRow; col++)
            {
                int x = grip.Right - spacing - (col * spacing) - dot;
                int y = grip.Bottom - spacing - (row * spacing) - dot;
                e.Graphics.FillEllipse(dotBrush, x, y, dot, dot);
            }
        }
    }
}