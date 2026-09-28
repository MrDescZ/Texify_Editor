using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Textify_Editor.Classes
{
    internal sealed class DarkMenuRenderer : ToolStripProfessionalRenderer
    {
        public static readonly Color MenuBackColor = Color.FromArgb(32, 32, 32);
        public static readonly Color BorderColor = Color.FromArgb(63, 63, 70);
        public static readonly Color SeparatorColor = Color.FromArgb(63, 63, 70);
        public static readonly Color TextColor = Color.Gainsboro;
        public static readonly Color DisabledTextColor = Color.FromArgb(120, 120, 120);
        public static readonly Color BarHoverColor = Color.FromArgb(45, 45, 48);
        public static readonly Color ItemHoverColor = Color.FromArgb(0, 122, 204);

        public DarkMenuRenderer() : base(new ProfessionalColorTable())
        {
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using (var brush = new SolidBrush(MenuBackColor))
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            // Only outline actual dropdown popups, not the menu bar itself
            // (the bar sits on the app's own dark title area).
            if (!(e.ToolStrip is ToolStripDropDown))
                return;

            using (var pen = new Pen(BorderColor))
                e.Graphics.DrawRectangle(pen, 0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            using (var brush = new SolidBrush(MenuBackColor))
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            int y = e.Item.Height / 2;
            int left = e.Item.IsOnDropDown ? 30 : 4;
            using (var pen = new Pen(SeparatorColor))
                e.Graphics.DrawLine(pen, left, y, e.Item.Width - 6, y);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var item = e.Item;
            if (!item.Selected && !item.Pressed)
                return;

            Rectangle bounds = new Rectangle(Point.Empty, item.Size);
            bounds.Inflate(item.IsOnDropDown ? -2 : -1, item.IsOnDropDown ? -2 : -1);
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            Color fill = item.IsOnDropDown ? ItemHoverColor : BarHoverColor;

            using (var brush = new SolidBrush(fill))
            using (var path = RoundedRect(bounds, 4))
                e.Graphics.FillPath(brush, path);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            bool highlighted = e.Item.IsOnDropDown && (e.Item.Selected || e.Item.Pressed);
            e.TextColor = !e.Item.Enabled ? DisabledTextColor : (highlighted ? Color.White : TextColor);
            base.OnRenderItemText(e);
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = e.Item.Enabled ? TextColor : DisabledTextColor;
            base.OnRenderArrow(e);
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

        private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
        private enum DwmWindowCornerPreference
        {
            Default = 0,
            DoNotRound = 1,
            Round = 2,
            RoundSmall = 3
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int valueSize);

        /// <summary>Rounds one dropdown's corners. Safe to call every time it opens.</summary>
        public static void ApplyRoundedCorners(ToolStripDropDownItem item)
        {
            var dropDown = item.DropDown;
            int preference = (int)DwmWindowCornerPreference.RoundSmall;
            // Harmless no-op on Windows 10, where this attribute doesn't exist.
            DwmSetWindowAttribute(dropDown.Handle, DWMWA_WINDOW_CORNER_PREFERENCE, ref preference, sizeof(int));
        }

        /// <summary>Wires rounded corners onto every dropdown in a menu, including nested submenus.</summary>
        public static void ApplyRoundedCornersRecursive(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                if (item is ToolStripDropDownItem dropDownItem && dropDownItem.HasDropDownItems)
                {
                    dropDownItem.DropDownOpening += (s, e) => ApplyRoundedCorners(dropDownItem);
                    ApplyRoundedCornersRecursive(dropDownItem.DropDownItems);
                }
            }
        }
    }
}