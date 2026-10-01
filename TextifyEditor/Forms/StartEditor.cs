using DarkModeForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Textify_Editor.Forms
{
    public partial class StartEditor : Form
    {
        private readonly DarkModeCS dm;
        private List<string> _recentFilesCache = [];
        private string _currentSearchQuery = "";
        private HashSet<string> _pinnedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private static readonly string PinnedFilesPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Textify Editor", "pinned.txt");

        const int DWMWA_BORDER_COLOR = 34;
        const int EM_SETCUEBANNER = 0x1501;

        private static readonly Color AccentBlue = Color.FromArgb(0, 122, 204);
        private static readonly Color InactiveBorder = Color.FromArgb(60, 60, 60);
        private static readonly Color SearchBorder = Color.FromArgb(70, 70, 74);
        private static readonly Color MutedText = Color.FromArgb(150, 150, 150);

        public StartEditor()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();

            dm = new DarkModeCS(this)
            {
                ColorMode = DarkModeCS.DisplayMode.SystemDefault
            };

            DarkModeCS.ExcludeFromProcessing(genxLabel2);
            DarkModeCS.ExcludeFromProcessing(searchHost);
            DarkModeCS.ExcludeFromProcessing(searchBox);
            DarkModeCS.ExcludeFromProcessing(searchIcon);
            DarkModeCS.ExcludeFromProcessing(searchClear);
            DarkModeCS.ExcludeFromProcessing(RecentList);
            //DarkModeCS.ExcludeFromProcessing(genxLabel1);
            DarkModeCS.ExcludeFromProcessing(panel1);
            DarkModeCS.ExcludeFromProcessing(createNewFile);
            DarkModeCS.ExcludeFromProcessing(chooseTemplate);
            DarkModeCS.ExcludeFromProcessing(openFile);

            StyleGhostButton(chooseTemplate);
            StyleGhostButton(createNewFile);
            StyleGhostButton(openFile);

            StylePrimaryButton(continueWithoutFile);

            SetupSearchBar();

            LoadPinnedFiles();

            RecentList.MouseDown += RecentList_MouseDown;
            toolStripMenuItem1.Click += RemoveSelectedRecent;
            pinMenuItem.Click += TogglePinSelected;
            recentListContextMenu.Opening += RecentListContextMenu_Opening;

            // Round every button on the form (recurses into panel1, so
            // continueWithoutFile is covered too), without needing to name
            // each one individually.
            ApplyRoundedCornersToAllButtons(this, 10);
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);
        private enum DwmSystemBackdropType
        {
            Auto = 0,
            None = 1,
            Mica = 2,
            Acrylic = 3,
            Tabbed = 4
        }

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, string appName, string idList);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        // ------------------------------------------------------------------
        //  Window lifecycle
        // ------------------------------------------------------------------

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            //SetBorderColor(AccentBlue);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e); // raises Load -> StartEditor_Load
            SetWindowTheme(RecentList.Handle, "DarkMode_Explorer", null);

            SendMessage(searchBox.Handle, EM_SETCUEBANNER, (IntPtr)1, "Search recent files (Ctrl+F)");

            // Vertically centre the search bar's contents once DPI scaling has been applied.
            CenterInSearchHost(searchIcon);
            CenterInSearchHost(searchBox);
            CenterInSearchHost(searchClear);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            //SetBorderColor(AccentBlue);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            SetBorderColor(InactiveBorder);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                searchBox.Focus();
                searchBox.SelectAll();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        void SetBorderColor(Color color)
        {
            int colorRef = ColorTranslator.ToWin32(color);
            DwmSetWindowAttribute(this.Handle, DWMWA_BORDER_COLOR, ref colorRef, sizeof(int));
        }

        // ------------------------------------------------------------------
        //  Search bar
        // ------------------------------------------------------------------

        private void SetupSearchBar()
        {
            searchBox.Enter += (s, e) => searchHost.BorderColor = AccentBlue;
            searchBox.Leave += (s, e) => searchHost.BorderColor = SearchBorder;

            EventHandler focusSearch = (s, e) => searchBox.Focus();
            searchHost.Click += focusSearch;
            searchIcon.Click += focusSearch;

            // Clear (x) glyph inside the bar.
            searchClear.Click += (s, e) =>
            {
                searchBox.Clear();
                searchBox.Focus();
            };
            searchClear.MouseEnter += (s, e) => searchClear.ForeColor = Color.White;
            searchClear.MouseLeave += (s, e) => searchClear.ForeColor = MutedText;

            // "Clear all" link next to the subtitle.
            //clearAllButton.MouseEnter += (s, e) => clearAllButton.ForeColor = Color.White;
            //clearAllButton.MouseLeave += (s, e) => clearAllButton.ForeColor = MutedText;

            searchHost.Resize += (s, e) =>
            {
                CenterInSearchHost(searchIcon);
                CenterInSearchHost(searchBox);
                CenterInSearchHost(searchClear);
            };
        }

        private void CenterInSearchHost(Control c)
        {
            c.Top = Math.Max(0, (searchHost.ClientSize.Height - c.Height) / 2);
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            _currentSearchQuery = searchBox.Text.Trim();
            searchClear.Visible = searchBox.TextLength > 0;
            ApplySearchFilter();
        }

        private void searchBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (searchBox.TextLength > 0)
                        searchBox.Clear();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Down:
                    if (RecentList.Enabled && RecentList.Items.Count > 0)
                    {
                        RecentList.Focus();
                        if (RecentList.SelectedIndex == -1)
                            RecentList.SelectedIndex = 0;
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Enter:
                    // Open the highlighted result, or the first match.
                    if (RecentList.Enabled && RecentList.Items.Count > 0)
                    {
                        if (RecentList.SelectedIndex == -1)
                            RecentList.SelectedIndex = 0;
                        OpenSelectedRecent();
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        // ------------------------------------------------------------------
        //  Recent files list
        // ------------------------------------------------------------------

        public void LoadRecentFiles()
        {
            // Read + validate once; the search filter always works from this cache.
            _recentFilesCache = RecentFileManager.GetRecentFiles()
                .Where(File.Exists)
                .ToList();

            // A pinned file that's been deleted from disk shouldn't linger in storage forever.
            if (_pinnedFiles.RemoveWhere(p => !File.Exists(p)) > 0)
                SavePinnedFiles();

            label1.Text = "Last synced " + DateTime.Now.ToString("MMM d, yyyy");

            ApplySearchFilter();
        }

        private void ApplySearchFilter()
        {
            IEnumerable<string> files = _recentFilesCache;

            if (!string.IsNullOrEmpty(_currentSearchQuery))
            {
                // Match on the file name or any part of its path.
                files = files.Where(f =>
                    f.IndexOf(_currentSearchQuery, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            // Pinned files float to the top. OrderByDescending is a stable sort, so
            // recency order is preserved within the pinned and unpinned groups.
            files = files.OrderByDescending(f => _pinnedFiles.Contains(f));

            UpdateRecentList(files.ToList());
        }

        void UpdateRecentList(List<string> files)
        {
            RecentList.Items.Clear();
            //clearAllButton.Visible = _recentFilesCache.Count > 0;

            if (files == null || files.Count == 0)
            {
                RecentList.Enabled = false;
                RecentList.Items.Add(new RecentFileItem
                {
                    FileName = string.IsNullOrEmpty(_currentSearchQuery) ? "No recent files" : "No matches",
                    FullPath = ""
                });
                RecentList.SelectedIndex = -1;
                //genxLabel1.Text = string.IsNullOrEmpty(_currentSearchQuery) ? "No recent files yet" : "No matches";
                return;
            }

            RecentList.Enabled = true;
            foreach (var file in files)
            {
                RecentList.Items.Add(new RecentFileItem
                {
                    FileName = Path.GetFileName(file),
                    FullPath = file
                });
            }
            RecentList.SelectedIndex = -1;
            /*genxLabel1.Text = string.IsNullOrEmpty(_currentSearchQuery)
                ? $"{files.Count} recent file{(files.Count == 1 ? "" : "s")}"
                : $"{files.Count} match{(files.Count == 1 ? "" : "es")}";*/
        }

        private void RemoveSelectedRecent(object sender, EventArgs e)
        {
            var item = RecentList.SelectedItem as RecentFileItem;
            if (item == null || string.IsNullOrWhiteSpace(item.FullPath))
                return;

            _recentFilesCache.RemoveAll(f => string.Equals(f, item.FullPath, StringComparison.OrdinalIgnoreCase));

            if (_pinnedFiles.Remove(item.FullPath))
                SavePinnedFiles();

            ApplySearchFilter();
        }

        // ------------------------------------------------------------------
        //  Pinning
        // ------------------------------------------------------------------

        private void RecentListContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var item = RecentList.SelectedItem as RecentFileItem;
            bool hasFile = item != null && !string.IsNullOrWhiteSpace(item.FullPath);

            pinMenuItem.Enabled = hasFile;
            toolStripMenuItem1.Enabled = hasFile;

            bool pinned = hasFile && _pinnedFiles.Contains(item.FullPath);
            pinMenuItem.Text = pinned ? "Unpin" : "Pin";
            pinMenuItem.Checked = pinned;
        }

        private void TogglePinSelected(object sender, EventArgs e)
        {
            var item = RecentList.SelectedItem as RecentFileItem;
            if (item == null || string.IsNullOrWhiteSpace(item.FullPath))
                return;

            if (!_pinnedFiles.Remove(item.FullPath))
                _pinnedFiles.Add(item.FullPath);

            SavePinnedFiles();
            ApplySearchFilter();
        }

        private void LoadPinnedFiles()
        {
            try
            {
                if (File.Exists(PinnedFilesPath))
                {
                    _pinnedFiles = new HashSet<string>(
                        File.ReadAllLines(PinnedFilesPath).Where(l => !string.IsNullOrWhiteSpace(l)),
                        StringComparer.OrdinalIgnoreCase);
                }
            }
            catch
            {
                // Pinning is a convenience, not critical data; start with an empty set on failure.
                _pinnedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            // RecentList only draws pin state, it doesn't own it - point it at the same
            // set so later Add/Remove/RemoveWhere calls on _pinnedFiles show up for free.
            //RecentList.PinnedPaths = _pinnedFiles;
        }

        private void SavePinnedFiles()
        {
            try
            {
                string dir = Path.GetDirectoryName(PinnedFilesPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllLines(PinnedFilesPath, _pinnedFiles);
            }
            catch
            {
                // Non-critical; pins just won't survive to the next session.
            }
        }

        private void clearAllButton_Click(object sender, EventArgs e)
        {
            if (Messenger.MessageBox(
                "Clear all recent files?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.None) == DialogResult.Yes)
            {
                RecentFileManager.Clear();
                searchBox.Clear();
                LoadRecentFiles();
            }
        }

        private void RecentList_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = RecentList.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                    RecentList.SelectedIndex = index;
            }
        }

        private void RecentList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OpenSelectedRecent();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                RemoveSelectedRecent(sender, e);
                e.Handled = true;
            }
        }

        private void RecentList_DoubleClick(object sender, EventArgs e)
        {
            // Only react when the double-click actually landed on an item,
            // not on the empty area below the last row.
            int hit = RecentList.IndexFromPoint(RecentList.PointToClient(Cursor.Position));
            if (hit == ListBox.NoMatches)
                return;

            OpenSelectedRecent();
        }

        private void OpenSelectedRecent()
        {
            if (!RecentList.Enabled) return;
            if (RecentList.SelectedIndex == -1) return;

            // Get the actual RecentFileItem object
            var item = RecentList.SelectedItem as RecentFileItem;
            if (item == null) return;

            string arcPath = item.FullPath;
            if (string.IsNullOrWhiteSpace(arcPath))
                return;

            if (!File.Exists(arcPath))
            {
                Messenger.MessageBox(
                    "This file no longer exists and will be removed from the list.",
                    "Missing file",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.None);

                // LoadRecentFiles re-validates every path, so the missing file drops out.
                LoadRecentFiles();
                return;
            }

            // Open the file in editor
            Hide();
            TextEditor arcS = new TextEditor();
            arcS.FormClosed += (s, args) => this.Close();
            arcS.Show();
            arcS.OpenFile(arcPath);
        }

        private void StartEditor_Load(object sender, EventArgs e)
        {
            LoadRecentFiles();
        }

        private void continueWithoutFile_Click(object sender, EventArgs e)
        {
            Hide();
            TextEditor te = new TextEditor();
            te.FormClosed += (s, args) => this.Close();
            te.Show();
        }

        private void createNewFile_Click(object sender, EventArgs e)
        {
            Hide();
            TextEditor te = new TextEditor();
            te.FormClosed += (s, args) => this.Close();
            te.Show();
            te.StartWithoutCode_();
        }

        private void openFile_Click(object sender, EventArgs e)
        {
            Hide();
            TextEditor te = new TextEditor();
            te.FormClosed += (s, args) => this.Close();
            te.Show();
            te.OpenMenuItem_Click(sender, e);
        }

        private void chooseTemplate_Click(object sender, EventArgs e)
        {
            using (var tf = new TemplateForm())
            {
                if (tf.ShowDialog() != DialogResult.OK)
                    return;
                var template = tf.SelectedTemplate;
                if (template == null)
                    return;
                Hide();
                string title = "Untitled" + template.Extension + " - Textify Editor";
                string docName = "MyApp";
                string code = Templates.GetTemplateText(template, docName);
                var te = new TextEditor();
                te.FormClosed += (s, args) => this.Close();
                te.Show();
                te.NewDoc(title, code, template.Language);
            }
        }

        private static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));

            if (diameter <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                if (rect.Width > 0 && rect.Height > 0)
                    path.AddRectangle(rect);
                return path;
            }

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void SetRoundedRegion(Control control, int radius)
        {
            if (control.Width <= 0 || control.Height <= 0)
                return;

            using (var path = CreateRoundedPath(control.ClientRectangle, radius))
            {
                control.Region = new Region(path);
            }
        }

        private static void ApplyRoundedCornersToAllButtons(Control root, int radius)
        {
            foreach (Control child in root.Controls)
            {
                if (child is Button button)
                    RoundButtonWithSmoothBorder(button, radius);

                if (child.Controls.Count > 0)
                    ApplyRoundedCornersToAllButtons(child, radius);
            }
        }

        private static void RoundButtonWithSmoothBorder(Button button, int radius)
        {
            Color borderColor = button.FlatAppearance.BorderColor;
            int borderSize = button.FlatAppearance.BorderSize;
            button.FlatAppearance.BorderSize = 0;

            void ApplyRegion()
            {
                SetRoundedRegion(button, radius);
                button.Invalidate();
            }

            button.Resize += (s, e) => ApplyRegion();
            ApplyRegion();

            if (borderSize <= 0)
                return;

            button.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                float inset = borderSize / 2f;
                var rect = Rectangle.Round(RectangleF.Inflate(button.ClientRectangle, -inset, -inset));

                using (var path = CreateRoundedPath(rect, Math.Max(radius - (int)inset, 1)))
                using (var pen = new Pen(borderColor, borderSize))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            };
        }

        private static void StyleGhostButton(Button button)
        {
            button.BackColor = Color.FromArgb(45, 45, 48);
            button.ForeColor = Color.White;
            button.Cursor = Cursors.Hand;
            button.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 74);
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 122, 204);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 99, 166);
        }

        private static void StylePrimaryButton(Button button)
        {
            button.Cursor = Cursors.Hand;
            button.Font = new Font(button.Font, FontStyle.Bold);
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 99, 166);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 82, 138);
        }
    }
}