using DarkModeForms;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Textify_Editor.Classes;

namespace Textify_Editor.Forms
{
    public partial class Settings : Form
    {
        private DarkModeCS dm = null;
        private readonly TextEditor _owner;
        private AppSettings Cfg => _owner.Config;
        private bool _initializing;
        private int _rowY;

        public Settings(TextEditor owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();

            DwmHelper.SetBorderColor(this.Handle, Cfg.AccentColor);
            DwmHelper.SetBackdrop(this.Handle, Cfg.Backdrop);
            DwmHelper.SetImmersiveDarkMode(this.Handle, Cfg.ImmersiveDarkTitleBar);

            dm = new DarkModeCS(this)
            {
                ColorMode = DarkModeCS.DisplayMode.DarkMode
            };

            pnlSidebar.BackColor = Color.FromArgb(24, 24, 28);
            pnlHeader.BackColor = Color.FromArgb(30, 30, 34);
            pnlFooter.BackColor = Color.FromArgb(24, 24, 28);
            pnlContent.BackColor = Color.FromArgb(34, 30, 34);
            pnlAppearance.BackColor = pnlContent.BackColor;
            pnlEditor.BackColor = pnlContent.BackColor;
            pnlFiles.BackColor = pnlContent.BackColor;
            pnlAbout.BackColor = pnlContent.BackColor;
            lblBrand.ForeColor = Color.FromArgb(140, 140, 150);
            lblHeaderTitle.ForeColor = Color.WhiteSmoke;
            pnlNavIndicator.BackColor = Cfg.AccentColor;

            btnClose.BackColor = Cfg.AccentColor;
            btnClose.FlatAppearance.BorderSize = 0;
            btnRestoreDefaults.ForeColor = Color.Gainsboro;
            btnRestoreDefaults.FlatAppearance.BorderColor = Color.FromArgb(90, 90, 100);

            StyleNavButton(btnNavAppearance);
            StyleNavButton(btnNavEditor);
            StyleNavButton(btnNavFiles);
            StyleNavButton(btnNavAbout);

            BuildAppearancePanel();
            BuildEditorPanel();
            BuildFilesPanel();
            BuildAboutPanel();

            SelectNav(btnNavAppearance, pnlAppearance, "Appearance");
        }

        void StyleNavButton(Button b)
        {
            b.BackColor = pnlSidebar.BackColor;
            b.ForeColor = Color.Gainsboro;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 40, 47);
            b.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 50, 58);
        }

        private void NavButton_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;

            if (btn == btnNavEditor)
                SelectNav(btn, pnlEditor, "Editor");
            else if (btn == btnNavFiles)
                SelectNav(btn, pnlFiles, "Files && Integrations");
            else if (btn == btnNavAbout)
                SelectNav(btn, pnlAbout, "About");
            else
                SelectNav(btn, pnlAppearance, "Appearance");
        }

        void SelectNav(Button selected, Panel panel, string title)
        {
            pnlAppearance.Visible = panel == pnlAppearance;
            pnlEditor.Visible = panel == pnlEditor;
            pnlFiles.Visible = panel == pnlFiles;
            pnlAbout.Visible = panel == pnlAbout;

            foreach (Control c in pnlSidebar.Controls)
            {
                if (c is Button navBtn)
                    navBtn.Font = new Font(navBtn.Font, FontStyle.Regular);
            }
            selected.Font = new Font(selected.Font, FontStyle.Bold);

            pnlNavIndicator.Top = selected.Top;
            lblHeaderTitle.Text = title;
        }

        void BeginPanel(Panel target)
        {
            target.Controls.Clear();
            _rowY = 0;
        }

        void AddHeader(Panel target, string text)
        {
            var lbl = new Label
            {
                Text = text,
                AutoSize = false,
                Location = new Point(0, _rowY),
                Size = new Size(600, 26),
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.WhiteSmoke
            };
            target.Controls.Add(lbl);
            _rowY += 36;
        }

        void AddSpacer(int px = 16) => _rowY += px;

        void AddRow(Panel target, string label, Control input, string caption = null)
        {
            if (!string.IsNullOrEmpty(label))
            {
                var lbl = new Label
                {
                    Text = label,
                    Location = new Point(0, _rowY + 4),
                    Size = new Size(280, 20),
                    Font = new Font("Segoe UI", 9.5f),
                    ForeColor = Color.Gainsboro
                };
                target.Controls.Add(lbl);
            }

            if (!(input is Button))
                DarkifyInput(input);

            input.Location = new Point(300, _rowY);
            target.Controls.Add(input);

            int rowHeight = Math.Max(28, input.Height + 4);
            _rowY += rowHeight;

            if (!string.IsNullOrEmpty(caption))
            {
                var cap = new Label
                {
                    Text = caption,
                    Location = new Point(0, _rowY),
                    Size = new Size(630, 32),
                    Font = new Font("Segoe UI", 8f),
                    ForeColor = Color.FromArgb(140, 140, 150)
                };
                target.Controls.Add(cap);
                _rowY += 32;
            }
            else
            {
                _rowY += 6;
            }
        }

        static void DarkifyInput(Control c)
        {
            if (c is CheckBox chk)
            {
                chk.ForeColor = Color.Gainsboro;
                chk.BackColor = Color.Transparent;
            }
            else
            {
                c.BackColor = Color.FromArgb(45, 45, 50);
                c.ForeColor = Color.Gainsboro;
            }
        }

        CheckBox AddCheckboxRow(Panel target, string label, bool initial, Action<bool> onChanged, string caption = null)
        {
            var chk = new CheckBox { Checked = initial, AutoSize = true };
            chk.CheckedChanged += (s, e) =>
            {
                if (_initializing) return;
                onChanged(chk.Checked);
            };
            AddRow(target, label, chk, caption);
            return chk;
        }

        NumericUpDown AddNumericRow(Panel target, string label, int min, int max, int initial, Action<int> onChanged, string caption = null)
        {
            var num = new NumericUpDown
            {
                Minimum = min,
                Maximum = max,
                Value = Math.Max(min, Math.Min(max, initial)),
                Size = new Size(80, 24)
            };
            num.ValueChanged += (s, e) =>
            {
                if (_initializing) return;
                onChanged((int)num.Value);
            };
            AddRow(target, label, num, caption);
            return num;
        }

        Button CreateColorSwatch(Color initial, Action<Color> onPicked)
        {
            var btn = new Button
            {
                Size = new Size(96, 26),
                FlatStyle = FlatStyle.Flat,
                BackColor = initial,
                ForeColor = ContrastColor(initial),
                Text = "Change...",
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(90, 90, 100);
            btn.Click += (s, e) =>
            {
                using (var dlg = new ColorDialog { Color = btn.BackColor, FullOpen = true })
                {
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        btn.BackColor = dlg.Color;
                        btn.ForeColor = ContrastColor(dlg.Color);
                        onPicked(dlg.Color);
                    }
                }
            };
            return btn;
        }

        static Color ContrastColor(Color c)
        {
            double luminance = (0.299 * c.R + 0.587 * c.G + 0.114 * c.B) / 255.0;
            return luminance > 0.55 ? Color.Black : Color.White;
        }

        void BuildAppearancePanel()
        {
            BeginPanel(pnlAppearance);
            _initializing = true;

            AddHeader(pnlAppearance, "Window (Windows 11)");

            AddRow(pnlAppearance, "Accent color",
                CreateColorSwatch(Cfg.AccentColor, c =>
                {
                    _owner.SetAccentColor(c);
                    pnlNavIndicator.BackColor = c;
                    btnClose.BackColor = c;
                    DwmHelper.SetBorderColor(this.Handle, c);
                    Cfg.Save();
                }),
                "Colors the thin window border (DWMWA_BORDER_COLOR) on both this dialog and the editor.");

            var backdropCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Size = new Size(220, 24) };
            backdropCombo.Items.AddRange(new object[] { "Auto", "None", "Mica", "Acrylic", "Tabbed" });
            backdropCombo.SelectedIndex = (int)Cfg.Backdrop;
            backdropCombo.SelectedIndexChanged += (s, e) =>
            {
                if (_initializing) return;
                var backdrop = (AppBackdropType)backdropCombo.SelectedIndex;
                _owner.SetBackdrop(backdrop);
                DwmHelper.SetBackdrop(this.Handle, backdrop);
                Cfg.Save();
            };
            AddRow(pnlAppearance, "Backdrop material", backdropCombo,
                "Native DWM backdrop material (DWMWA_SYSTEMBACKDROP_TYPE). Requires Windows 11; ignored on older systems.");

            AddCheckboxRow(pnlAppearance, "Dark title bar", Cfg.ImmersiveDarkTitleBar, v =>
            {
                _owner.SetImmersiveDarkTitleBar(v);
                DwmHelper.SetImmersiveDarkMode(this.Handle, v);
                Cfg.Save();
            }, "DWMWA_USE_IMMERSIVE_DARK_MODE - keeps the native frame/title bar dark.");

            AddSpacer();
            AddHeader(pnlAppearance, "Editor background (Still in Development)");

            AddCheckboxRow(pnlAppearance, "Animated gradient background", Cfg.GradientBackgroundEnabled, v =>
            {
                Cfg.GradientBackgroundEnabled = v;
                //_owner.ApplyGradientSetting();
                //Cfg.Save();
            });

            AddRow(pnlAppearance, "Gradient top color", CreateColorSwatch(Cfg.GradientTopColor, c =>
            {
                Cfg.GradientTopColor = c;
                //_owner.ApplyGradientSetting();
                //Cfg.Save();
            }));

            AddRow(pnlAppearance, "Gradient bottom color", CreateColorSwatch(Cfg.GradientBottomColor, c =>
            {
                Cfg.GradientBottomColor = c;
                //_owner.ApplyGradientSetting();
                //Cfg.Save();
            }));

            AddSpacer();
            AddHeader(pnlAppearance, "Syntax colors");

            AddRow(pnlAppearance, "Keywords", CreateColorSwatch(Cfg.KeywordColor, c => { Cfg.KeywordColor = c; _owner.ApplySyntaxColors(); Cfg.Save(); }));
            AddRow(pnlAppearance, "Strings", CreateColorSwatch(Cfg.StringColor, c => { Cfg.StringColor = c; _owner.ApplySyntaxColors(); Cfg.Save(); }));
            AddRow(pnlAppearance, "Comments", CreateColorSwatch(Cfg.CommentColor, c => { Cfg.CommentColor = c; _owner.ApplySyntaxColors(); Cfg.Save(); }));
            AddRow(pnlAppearance, "Numbers", CreateColorSwatch(Cfg.NumberColor, c => { Cfg.NumberColor = c; _owner.ApplySyntaxColors(); Cfg.Save(); }));
            AddRow(pnlAppearance, "Preprocessor", CreateColorSwatch(Cfg.PreprocessorColor, c => { Cfg.PreprocessorColor = c; _owner.ApplySyntaxColors(); Cfg.Save(); }));
            AddRow(pnlAppearance, "Operators", CreateColorSwatch(Cfg.OperatorColor, c => { Cfg.OperatorColor = c; _owner.ApplySyntaxColors(); Cfg.Save(); }));

            _initializing = false;
        }

        void BuildEditorPanel()
        {
            BeginPanel(pnlEditor);
            _initializing = true;

            AddHeader(pnlEditor, "Font");

            var fontCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Size = new Size(220, 24) };
            var fontNames = FontFamily.Families.Select(f => f.Name).OrderBy(n => n).ToArray();
            fontCombo.Items.AddRange(fontNames);
            int fontIndex = Array.IndexOf(fontNames, Cfg.FontFamily);
            fontCombo.SelectedIndex = fontIndex >= 0 ? fontIndex : 0;
            fontCombo.SelectedIndexChanged += (s, e) =>
            {
                if (_initializing) return;
                Cfg.FontFamily = fontCombo.SelectedItem.ToString();
                _owner.ApplyEditorSettings();
                Cfg.Save();
            };
            AddRow(pnlEditor, "Font family", fontCombo);

            AddNumericRow(pnlEditor, "Font size", 6, 72, Cfg.FontSize, v =>
            {
                Cfg.FontSize = v;
                _owner.ApplyEditorSettings();
                Cfg.Save();
            });

            AddSpacer();
            AddHeader(pnlEditor, "Indentation");

            AddNumericRow(pnlEditor, "Tab width", 1, 8, Cfg.TabWidth, v =>
            {
                Cfg.TabWidth = v;
                _owner.ApplyTabAndIndentSettings();
                Cfg.Save();
            });

            AddNumericRow(pnlEditor, "Indent width", 0, 8, Cfg.IndentWidth, v =>
            {
                Cfg.IndentWidth = v;
                _owner.ApplyTabAndIndentSettings();
                Cfg.Save();
            });

            AddCheckboxRow(pnlEditor, "Show indent guides", Cfg.ShowIndentGuides, v =>
            {
                Cfg.ShowIndentGuides = v;
                _owner.ApplyIndentGuidesSetting();
                Cfg.Save();
            });

            AddSpacer();
            AddHeader(pnlEditor, "Behavior");

            AddCheckboxRow(pnlEditor, "Word wrap", Cfg.WordWrap, v =>
            {
                Cfg.WordWrap = v;
                _owner.ApplyWordWrap();
                Cfg.Save();
            });

            AddCheckboxRow(pnlEditor, "Auto-indent new lines", Cfg.AutoIndent, v =>
            {
                Cfg.AutoIndent = v;
                Cfg.Save();
            });

            AddCheckboxRow(pnlEditor, "Auto-close brackets && quotes", Cfg.AutoCloseBrackets, v =>
            {
                Cfg.AutoCloseBrackets = v;
                Cfg.Save();
            });

            AddCheckboxRow(pnlEditor, "Show line numbers", Cfg.ShowLineNumbers, v =>
            {
                Cfg.ShowLineNumbers = v;
                _owner.ApplyLineNumbersSetting();
                Cfg.Save();
            });

            AddCheckboxRow(pnlEditor, "Show whitespace", Cfg.ShowWhitespace, v =>
            {
                Cfg.ShowWhitespace = v;
                _owner.ApplyWhitespaceSetting();
                Cfg.Save();
            });

            var eolCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Size = new Size(220, 24) };
            eolCombo.Items.AddRange(new object[] { "LF (Unix/macOS)", "CRLF (Windows)", "CR (Classic Mac)" });
            eolCombo.SelectedIndex = (int)Cfg.EolMode;
            eolCombo.SelectedIndexChanged += (s, e) =>
            {
                if (_initializing) return;
                Cfg.EolMode = (AppEolMode)eolCombo.SelectedIndex;
                _owner.ApplyEolSetting();
                Cfg.Save();
            };
            AddRow(pnlEditor, "Line endings", eolCombo);

            var zoomCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Size = new Size(220, 24) };
            int[] zoomLevels = { 50, 75, 80, 100, 125, 150, 200, 400 };
            foreach (var z in zoomLevels) zoomCombo.Items.Add(z + " %");
            int zIndex = Array.IndexOf(zoomLevels, Cfg.DefaultZoomPercent);
            zoomCombo.SelectedIndex = zIndex >= 0 ? zIndex : 2;
            zoomCombo.SelectedIndexChanged += (s, e) =>
            {
                if (_initializing) return;
                Cfg.DefaultZoomPercent = zoomLevels[zoomCombo.SelectedIndex];
                Cfg.Save();
            };
            AddRow(pnlEditor, "Default zoom", zoomCombo, "Used the next time a document is opened.");

            _initializing = false;
        }

        void BuildFilesPanel()
        {
            BeginPanel(pnlFiles);
            _initializing = true;

            AddHeader(pnlFiles, "Files");

            AddCheckboxRow(pnlFiles, "Open files as read-only", Cfg.OpenFilesReadOnly, v =>
            {
                Cfg.OpenFilesReadOnly = v;
                Cfg.Save();
            }, "Applies the next time a file is opened.");

            AddNumericRow(pnlFiles, "Recent files to remember", 1, 25, Cfg.MaxRecentFiles, v =>
            {
                Cfg.MaxRecentFiles = v;
                _owner.LoadRecentFiles();
                Cfg.Save();
            });

            var clearBtn = new Button
            {
                Text = "Clear Recent Files List",
                Size = new Size(190, 28),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Gainsboro,
                BackColor = Color.FromArgb(45, 45, 50)
            };
            clearBtn.FlatAppearance.BorderColor = Color.FromArgb(90, 90, 100);
            clearBtn.Click += (s, e) => _owner.ClearRecentFiles();
            AddRow(pnlFiles, string.Empty, clearBtn);

            AddSpacer();
            AddHeader(pnlFiles, "Integrations");

            _initializing = false;
        }

        void BuildAboutPanel()
        {
            BeginPanel(pnlAbout);

            AddHeader(pnlAbout, "Textify Editor");

            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            var lblVersion = new Label
            {
                Text = "Version " + version,
                AutoSize = true,
                Location = new Point(0, _rowY),
                ForeColor = Color.Gainsboro,
                Font = new Font("Segoe UI", 9.5f)
            };
            pnlAbout.Controls.Add(lblVersion);
            _rowY += 30;

            var lblCredits = new Label
            {
                Text = "Built with ScintillaNET Engine for code editing\r\n\r\n" +
                       "Settings are stored in:\r\n%AppData%\\Textify Editor\\settings.xml\r\n\r\n" +
                       "Made by DescZ",
                AutoSize = true,
                Location = new Point(0, _rowY),
                ForeColor = Color.FromArgb(150, 150, 160),
                Font = new Font("Segoe UI", 9f)
            };
            pnlAbout.Controls.Add(lblCredits);
            _rowY += 80;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnRestoreDefaults_Click(object sender, EventArgs e)
        {
            DialogResult confirm = Messenger.MessageBox(
                "Reset all settings to their defaults? This can't be undone.",
                "Restore Defaults",
                MessageBoxButtons.YesNo,
                MsgIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            Cfg.ResetToDefaults();
            Cfg.Save();

            _owner.ApplyAppearanceSettings();
            _owner.ApplyEditorSettings();

            DwmHelper.SetBorderColor(this.Handle, Cfg.AccentColor);
            DwmHelper.SetBackdrop(this.Handle, Cfg.Backdrop);
            DwmHelper.SetImmersiveDarkMode(this.Handle, Cfg.ImmersiveDarkTitleBar);
            pnlNavIndicator.BackColor = Cfg.AccentColor;
            btnClose.BackColor = Cfg.AccentColor;

            bool wasEditor = pnlEditor.Visible;
            bool wasFiles = pnlFiles.Visible;
            bool wasAbout = pnlAbout.Visible;

            BuildAppearancePanel();
            BuildEditorPanel();
            BuildFilesPanel();
            BuildAboutPanel();

            if (wasEditor) SelectNav(btnNavEditor, pnlEditor, "Editor");
            else if (wasFiles) SelectNav(btnNavFiles, pnlFiles, "Files && Integrations");
            else if (wasAbout) SelectNav(btnNavAbout, pnlAbout, "About");
            else SelectNav(btnNavAppearance, pnlAppearance, "Appearance");
        }
    }
}