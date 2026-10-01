using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Textify_Editor;

public class CustomListBox : ListBox
{
    public int ItemPadding { get; set; } = 12;

    public Color NormalBackColor { get; set; } = Color.FromArgb(30, 30, 30);
    public Color NormalTextColor { get; set; } = Color.Gainsboro;
    public Color DescTextColor { get; set; } = Color.FromArgb(160, 160, 160);

    public Color SelectedBackColor { get; set; } = Color.FromArgb(0, 122, 204);
    public Color SelectedTextColor { get; set; } = Color.White;
    public Color SelectedDescColor { get; set; } = Color.FromArgb(235, 235, 235);

    public Color SeparatorColor { get; set; } = Color.FromArgb(45, 45, 45);

    public CustomListBox()
    {
        DrawMode = DrawMode.OwnerDrawFixed;
        IntegralHeight = false;
        ItemHeight = 50;

        BorderStyle = BorderStyle.FixedSingle;

        BackColor = NormalBackColor;
        ForeColor = NormalTextColor;

        DoubleBuffered = true;
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index < 0 || e.Index >= Items.Count)
            return;

        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

        Color back = selected ? SelectedBackColor : NormalBackColor;
        Color titleColor = selected ? SelectedTextColor : NormalTextColor;
        Color descColor = selected ? SelectedDescColor : DescTextColor;

        // Background
        using (var b = new SolidBrush(back))
            g.FillRectangle(b, e.Bounds);

        // Item text
        var item = Items[e.Index] as TemplateInfo;

        string title = item != null ? item.Name : Items[e.Index].ToString();
        string desc = item != null ? item.Description : "";

        Rectangle bounds = e.Bounds;
        bounds.Inflate(-2, -2);

        int x = bounds.X + ItemPadding;
        int y = bounds.Y + 8;

        Rectangle titleRect = new Rectangle(x, y, bounds.Width - ItemPadding * 2, 22);
        Rectangle descRect = new Rectangle(x, titleRect.Bottom + 2, bounds.Width - ItemPadding * 2, 20);

        using (var titleFont = new Font(Font.FontFamily, 10f, FontStyle.Bold))
        using (var descFont = new Font(Font.FontFamily, 8.5f, FontStyle.Regular))
        using (var titleBrush = new SolidBrush(titleColor))
        using (var descBrush = new SolidBrush(descColor))
        {
            g.DrawString(title, titleFont, titleBrush, titleRect);
            g.DrawString(desc, descFont, descBrush, descRect);
        }

        // Separator line
        using (var p = new Pen(SeparatorColor))
            g.DrawLine(p, bounds.X, bounds.Bottom, bounds.Right, bounds.Bottom);
    }
}

public class RecentFilesListBox : ListBox
{
    public int ItemPadding { get; set; } = 12;

    public Color NormalBackColor { get; set; } = Color.FromArgb(30, 30, 30);
    public Color NormalTitleColor { get; set; } = Color.Gainsboro;
    public Color NormalPathColor { get; set; } = Color.FromArgb(150, 150, 150);

    public Color HoverBackColor { get; set; } = Color.FromArgb(45, 45, 48);

    public Color SelectedBackColor { get; set; } = Color.FromArgb(0, 122, 204);
    public Color SelectedTitleColor { get; set; } = Color.White;
    public Color SelectedPathColor { get; set; } = Color.FromArgb(235, 235, 235);

    public Color SeparatorColor { get; set; } = Color.FromArgb(45, 45, 45);
    private int hoveredIndex = -1;

    public RecentFilesListBox()
    {
        DrawMode = DrawMode.OwnerDrawFixed;
        ItemHeight = 48;

        BorderStyle = BorderStyle.None;
        BackColor = NormalBackColor;

        DoubleBuffered = true;
        MouseMove += ListBox_MouseMove;
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index < 0 || e.Index >= Items.Count)
            return;

        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        using (var baseBrush = new SolidBrush(NormalBackColor))
            g.FillRectangle(baseBrush, e.Bounds);

        var item = Items[e.Index] as RecentFileItem;
        string title = item != null ? item.FileName : Items[e.Index].ToString();
        string path = item != null ? item.FullPath : "";

        bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
        bool hovered = e.Index == hoveredIndex;

        Rectangle card = e.Bounds;
        card.Inflate(-4, -2);

        if (selected || hovered)
        {
            Color fill = selected ? SelectedBackColor : HoverBackColor;
            using (var cardBrush = new SolidBrush(fill))
            using (var cardPath = RoundedRect(card, 8))
                g.FillPath(cardBrush, cardPath);
        }

        Color titleColor = selected ? SelectedTitleColor : NormalTitleColor;
        Color pathColor = selected ? SelectedPathColor : NormalPathColor;
        Color iconColor = selected ? SelectedTitleColor : NormalPathColor;

        int iconSize = 20;
        int x = card.X + ItemPadding;
        int iconY = card.Y + (card.Height - iconSize) / 2;
        DrawFileIcon(g, new Rectangle(x, iconY, iconSize, iconSize), iconColor);

        int textX = x + iconSize + 10;
        int textWidth = card.Right - textX - ItemPadding;
        int textBlockHeight = 20 + 2 + 16;
        int textY = card.Y + (card.Height - textBlockHeight) / 2;

        Rectangle titleRect = new Rectangle(textX, textY, textWidth, 20);
        Rectangle pathRect = new Rectangle(textX, titleRect.Bottom + 2, textWidth, 16);

        using (var titleFont = new Font(Font.FontFamily, 10f, FontStyle.Bold))
        using (var pathFont = new Font(Font.FontFamily, 8f, FontStyle.Regular))
        using (var titleBrush = new SolidBrush(titleColor))
        using (var pathBrush = new SolidBrush(pathColor))
        using (var format = new StringFormat { Trimming = StringTrimming.EllipsisPath, FormatFlags = StringFormatFlags.NoWrap })
        {
            g.DrawString(title, titleFont, titleBrush, titleRect, format);
            g.DrawString(path, pathFont, pathBrush, pathRect, format);
        }

        if (!selected && !hovered)
        {
            using (var p = new Pen(SeparatorColor))
                g.DrawLine(p, e.Bounds.X + ItemPadding, e.Bounds.Bottom - 1, e.Bounds.Right - ItemPadding, e.Bounds.Bottom - 1);
        }

        e.DrawFocusRectangle();
    }

    private static GraphicsPath RoundedRect(Rectangle rect, int radius)
    {
        int d = radius * 2;
        var path = new GraphicsPath();
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    private static void DrawFileIcon(Graphics g, Rectangle rect, Color color)
    {
        int fold = rect.Width / 3;
        using (var body = new GraphicsPath())
        {
            body.AddLine(rect.Left, rect.Top, rect.Right - fold, rect.Top);
            body.AddLine(rect.Right - fold, rect.Top, rect.Right, rect.Top + fold);
            body.AddLine(rect.Right, rect.Top + fold, rect.Right, rect.Bottom);
            body.AddLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom);
            body.CloseFigure();

            using (var pen = new Pen(color, 1.3f))
                g.DrawPath(pen, body);
        }

        using (var pen = new Pen(color, 1.1f))
        {
            g.DrawLine(pen, rect.Right - fold, rect.Top, rect.Right - fold, rect.Top + fold);
            g.DrawLine(pen, rect.Right - fold, rect.Top + fold, rect.Right, rect.Top + fold);
        }
    }

    private void ListBox_MouseMove(object sender, MouseEventArgs e)
    {
        int index = IndexFromPoint(e.Location);

        if (index != hoveredIndex)
        {
            int oldIndex = hoveredIndex;
            hoveredIndex = index;

            if (oldIndex >= 0)
                Invalidate(GetItemRectangle(oldIndex));
            if (hoveredIndex >= 0)
                Invalidate(GetItemRectangle(hoveredIndex));
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        int oldIndex = hoveredIndex;
        hoveredIndex = -1;
        if (oldIndex >= 0)
            Invalidate(GetItemRectangle(oldIndex));
        base.OnMouseLeave(e);
    }
}