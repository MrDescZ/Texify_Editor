using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Textify_Editor.Classes
{

    internal static class DwmHelper
    {
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_BORDER_COLOR = 34;
        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

        /// <summary>Sets the thin accent-colored border Windows 11 draws around the window.</summary>
        public static void SetBorderColor(IntPtr handle, Color color)
        {
            int colorRef = ColorTranslator.ToWin32(color);
            DwmSetWindowAttribute(handle, DWMWA_BORDER_COLOR, ref colorRef, sizeof(int));
        }

        /// <summary>Sets the window's backdrop material (None / Mica / Acrylic / Tabbed). Windows 11 22H2+ only; a no-op elsewhere.</summary>
        public static void SetBackdrop(IntPtr handle, AppBackdropType backdrop)
        {
            int value = (int)backdrop;
            DwmSetWindowAttribute(handle, DWMWA_SYSTEMBACKDROP_TYPE, ref value, sizeof(int));
        }

        /// <summary>Toggles the native dark title bar/frame chrome. Windows 10 1809+ / Windows 11.</summary>
        public static void SetImmersiveDarkMode(IntPtr handle, bool enabled)
        {
            int value = enabled ? 1 : 0;
            DwmSetWindowAttribute(handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int));
        }
    }
}