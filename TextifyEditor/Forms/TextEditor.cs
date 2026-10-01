using DarkModeForms;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Textify_Editor.Classes;
using Textify_Editor.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Textify_Editor
{
    public partial class TextEditor : Form
    {
        string fileName;    // current opened file
        int currentFontSize = 11;
        int pos, line, column;    // for detecting line and column number
        const int borderIndicator = 8;
        private DarkModeCS dm;

        internal AppSettings Config;

        Color[] _gradientBandColors = new Color[GradientBandCount];
        const int GradientMarkerBase = 2;
        const int GradientBandCount = 20;
        const int GradientMarginStyleBase = 40;
        int _lastGradientLineCount = -1;

        private readonly Dictionary<char, char>
            _autoPairs = new()
            {
                ['('] = ')',
                ['['] = ']',
                ['<'] = '>',
                ['{'] = '}',
                ['"'] = '"',
                ['\''] = '\''
            };

        bool _isFullScreen = false;
        FormBorderStyle _prevBorderStyle;
        FormWindowState _prevWindowState;
        Rectangle _prevBounds;

        Form _findReplaceDialog;
        TextBox _findBox;
        TextBox _replaceBox;
        CheckBox _matchCaseBox;

        public TextEditor()
        {
            InitializeComponent();

            Config = AppSettings.Load();
            currentFontSize = Config.FontSize;

            SetBorderColor(Config.AccentColor);
            ApplyBackdrop();
            ApplyImmersiveDarkMode();
            ApplyGradientSetting();
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.FixedHeight |
                 ControlStyles.FixedWidth |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.UserMouse |
                 ControlStyles.ContainerControl |
                 ControlStyles.SupportsTransparentBackColor |
                 ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            InitEditor();

            dm = new DarkModeCS(this)
            {
                ColorMode = DarkModeCS.DisplayMode.DarkMode
            };

            menuStrip1.Renderer = new DarkMenuRenderer();
            DarkMenuRenderer.ApplyRoundedCornersRecursive(menuStrip1.Items);
            statusStrip1.Renderer = new GradientStatusStripRenderer();
            toolStrip1.Renderer = new GradientToolStripRenderer();
            #region excluded
            DarkModeCS.ExcludeFromProcessing(label4);
            DarkModeCS.ExcludeFromProcessing(menuStrip1);
            DarkModeCS.ExcludeFromProcessing(createNewFile);
            DarkModeCS.ExcludeFromProcessing(textifyPicture);
            DarkModeCS.ExcludeFromProcessing(label2);
            DarkModeCS.ExcludeFromProcessing(label1);
            DarkModeCS.ExcludeFromProcessing(label1);
            DarkModeCS.ExcludeFromProcessing(StartWithoutCode);
            DarkModeCS.ExcludeFromProcessing(label5);
            DarkModeCS.ExcludeFromProcessing(toolStrip1);
            DarkModeCS.ExcludeFromProcessing(label3);
            DarkModeCS.ExcludeFromProcessing(statusStrip1);
            #endregion
            menuStrip1.ForeColor = Color.Gainsboro;
            this.TransparencyKey = Color.Empty;

            foreach (ToolStripItem item in statusStrip1.Items)
            {
                item.ForeColor = Color.Gainsboro;
            }
        }

        void SetBorderColor(Color color) => DwmHelper.SetBorderColor(this.Handle, color);

        internal void ApplyBackdrop() => DwmHelper.SetBackdrop(this.Handle, Config.Backdrop);

        internal void ApplyImmersiveDarkMode() => DwmHelper.SetImmersiveDarkMode(this.Handle, Config.ImmersiveDarkTitleBar);

        internal void SetAccentColor(Color color)
        {
            Config.AccentColor = color;
            SetBorderColor(color);
        }

        internal void SetBackdrop(AppBackdropType backdrop)
        {
            Config.Backdrop = backdrop;
            ApplyBackdrop();
        }

        internal void SetImmersiveDarkTitleBar(bool enabled)
        {
            Config.ImmersiveDarkTitleBar = enabled;
            ApplyImmersiveDarkMode();
        }

        internal void ApplyGradientSetting()
        {
            if (Config.GradientBackgroundEnabled)
                ApplyGradientBackground(Config.GradientTopColor, Config.GradientBottomColor);
            else
                ApplyGradientBackground(Color.FromArgb(34, 30, 34), Color.FromArgb(34, 30, 34));
        }

        internal void ApplySyntaxColors()
        {
            editor.Styles[Style.Cpp.Word].ForeColor = Config.KeywordColor;
            editor.Styles[Style.Cpp.String].ForeColor = Config.StringColor;
            editor.Styles[Style.Cpp.Comment].ForeColor = Config.CommentColor;
            editor.Styles[Style.Cpp.CommentLine].ForeColor = Config.CommentColor;
            editor.Styles[Style.Cpp.Number].ForeColor = Config.NumberColor;
            editor.Styles[Style.Cpp.Preprocessor].ForeColor = Config.PreprocessorColor;
            editor.Styles[Style.Cpp.Operator].ForeColor = Config.OperatorColor;
        }

        internal void ApplyAppearanceSettings()
        {
            SetBorderColor(Config.AccentColor);
            ApplyBackdrop();
            ApplyImmersiveDarkMode();
            //ApplyGradientSetting();
            ApplySyntaxColors();
        }

        internal void ApplyWordWrap()
        {
            editor.WrapMode = Config.WordWrap ? ScintillaNET.WrapMode.Word : ScintillaNET.WrapMode.None;
            wordWrapToolStripMenuItem.Checked = Config.WordWrap;
        }

        internal void ApplyWhitespaceSetting()
        {
            editor.ViewWhitespace = Config.ShowWhitespace ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
        }

        internal void ApplyIndentGuidesSetting()
        {
            editor.IndentationGuides = Config.ShowIndentGuides ? IndentView.LookBoth : IndentView.None;
        }

        internal void ApplyLineNumbersSetting()
        {
            editor.Margins[0].Width = Config.ShowLineNumbers ? 40 : 0;
        }

        internal void ApplyTabAndIndentSettings()
        {
            editor.TabWidth = Config.TabWidth;
            editor.IndentWidth = Config.IndentWidth;
        }

        internal void ApplyEolSetting()
        {
            switch (Config.EolMode)
            {
                case AppEolMode.CrLf:
                    editor.EolMode = Eol.CrLf;
                    break;
                case AppEolMode.Cr:
                    editor.EolMode = Eol.Cr;
                    break;
                default:
                    editor.EolMode = Eol.Lf;
                    break;
            }
        }

        internal void ApplyEditorSettings()
        {
            currentFontSize = Config.FontSize;
            ApplyFontSize();
            ApplyTabAndIndentSettings();
            ApplyWordWrap();
            ApplyWhitespaceSetting();
            ApplyIndentGuidesSetting();
            ApplyLineNumbersSetting();
            ApplyEolSetting();
        }

        internal void ClearRecentFiles()
        {
            var clearAll = typeof(RecentFileManager).GetMethod("ClearAll", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            clearAll?.Invoke(null, null);
            LoadRecentFiles();
        }

        private async void frmEditor_Load(object sender, EventArgs e)
        {
            LoadRecentFiles();
            await UpdateFileMenuStatus(false);
            wordWrapToolStripMenuItem.Checked = Config.WordWrap;
            openFileStripButton.Enabled = true;
            colorDialog1.AllowFullOpen = true;
            colorDialog1.SolidColorOnly = false;
            colorDialog1.ShowHelp = true;
            colorDialog1.AnyColor = true;
            boldStripButton3.Checked = false;
            italicStripButton.Checked = false;
            bulletListStripButton.Checked = false;

            zoomDropDownButton.DropDownItems.Clear();

            int[] zoomLevels = { 50, 75, 100, 125, 150, 200, 400 };
            foreach (int level in zoomLevels)
            {
                zoomDropDownButton.DropDownItems.Add(level + " %");
            }
            SetZoomPercent(Config.DefaultZoomPercent); // configurable default
            zoomDropDownButton.Text = Config.DefaultZoomPercent + " %";

            // fill font sizes in combo box
            for (int i = 8; i < 80; i += 2)
            {
                fontSizeComboBox.Items.Add(i);
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            SetBorderColor(Config.AccentColor);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            SetBorderColor(Color.FromArgb(60, 60, 60));
        }

        private void editor_CharAdded(object sender, CharAddedEventArgs e)
        {
            char c = (char)e.Char;

            if (c == '\n')
            {
                if (!Config.AutoIndent)
                    return;

                int line = editor.CurrentLine;
                int prevLine = line - 1;

                if (prevLine >= 0)
                {
                    int indent = editor.Lines[prevLine].Indentation;
                    editor.Lines[line].Indentation = indent;
                }

                return;
            }

            // Auto pairs
            if (!Config.AutoCloseBrackets)
                return;

            if (!_autoPairs.TryGetValue(c, out char closeChar))
                return;

            int pos = editor.CurrentPosition;

            // Wrap selection
            if (editor.SelectionStart != editor.SelectionEnd)
            {
                int start = editor.SelectionStart;
                int end = editor.SelectionEnd;

                editor.BeginUndoAction();

                editor.InsertText(end, closeChar.ToString());
                editor.InsertText(start, c.ToString());

                editor.EndUndoAction();

                editor.SetSelection(start + 1, end + 1);
                return;
            }

            if (pos < editor.TextLength && editor.GetCharAt(pos) == closeChar)
            {
                editor.CurrentPosition = pos + 1;
                return;
            }

            editor.BeginUndoAction();

            editor.InsertText(pos, closeChar.ToString());
            editor.CurrentPosition = pos;

            editor.EndUndoAction();

            if (c == '{')
            {
                int line = editor.CurrentLine + 1;

                if (line < editor.Lines.Count)
                    editor.Lines[line].Indentation = editor.IndentWidth;
            }
        }

        private void editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int pos = editor.CurrentPosition;
                if (pos > 0 && pos < editor.TextLength)
                {
                    char prev = (char)editor.GetCharAt(pos - 1);
                    char next = (char)editor.GetCharAt(pos);

                    if (prev == '{' && next == '}')
                    {
                        int line = editor.LineFromPosition(pos);
                        int indent = editor.Lines[line].Indentation;
                        editor.BeginUndoAction();

                        string indentStr = new string(' ', indent + editor.TabWidth);
                        string closingIndent = new string(' ', indent);
                        editor.InsertText(pos, " \n" + indentStr + "\n" + closingIndent);
                        editor.CurrentPosition = pos + 1 + indentStr.Length;

                        editor.EndUndoAction();
                        editor.Focus();
                        e.SuppressKeyPress = true;
                    }
                }
            }

            if (e.KeyCode == Keys.Back)
            {
                int pos = editor.CurrentPosition;

                if (pos > 0 && pos < editor.TextLength)
                {
                    char prev = (char)editor.GetCharAt(pos - 1);
                    char next = (char)editor.GetCharAt(pos);

                    if (Config.AutoCloseBrackets && _autoPairs.TryGetValue(prev, out char close) && close == next)
                    {
                        editor.BeginUndoAction();
                        editor.DeleteRange(pos - 1, 2);
                        editor.EndUndoAction();
                        e.SuppressKeyPress = true;
                    }
                }
            }
        }

        void InitEditor()
        {
            editor.Lexer = Lexer.Cpp;
            editor.IndentationGuides = IndentView.LookBoth;
            editor.Margins[0].Width = 40;

            // base
            editor.StyleResetDefault();
            editor.Styles[Style.Default].BackColor = Color.FromArgb(34, 30, 34);
            editor.Styles[Style.Default].ForeColor = Color.FromArgb(220, 220, 220);
            editor.Styles[Style.Default].Font = "Cascadia Mono";
            editor.Styles[Style.Default].Size = 11;
            editor.Markers[0].Symbol = MarkerSymbol.Circle;
            editor.Markers[0].SetBackColor(Color.Red);
            editor.Indicators[0].ForeColor = Color.Red;
            editor.BraceBadLight(pos);
            editor.StyleClearAll();

            ApplyGradientSetting();
            ApplyGradientBackground(Color.FromArgb(34, 30, 34), Color.FromArgb(34, 30, 34));

            // Line numbers
            editor.Colorize(0, -1);
            editor.Styles[Style.LineNumber].BackColor = Color.FromArgb(34, 30, 34);
            editor.Styles[Style.LineNumber].ForeColor = Color.FromArgb(120, 120, 125);
            editor.Margins[0].BackColor = Color.FromArgb(30, 30, 34);
            editor.Margins[0].Type = MarginType.Number;
            editor.Styles[Style.LineNumber].ForeColor = Color.FromArgb(120, 130, 150);

            // caret & selection
            editor.CaretLineVisible = true;
            editor.CaretForeColor = Color.FromArgb(255, 255, 255);
            editor.CaretWidth = 2;
            editor.CaretLineBackColor = Color.FromArgb(0, 40, 45);

            editor.SetSelectionBackColor(true, Color.FromArgb(60, 90, 140));
            editor.SetSelectionForeColor(true, ForeColor);

            // keywords and others 
            editor.Styles[Style.Cpp.Word].ForeColor = Color.FromArgb(97, 175, 239);
            editor.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(152, 195, 121);
            editor.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(255, 100, 0);
            editor.Styles[Style.Cpp.CommentLine].ForeColor = Color.ForestGreen;
            editor.Styles[Style.Cpp.Number].ForeColor = Color.FromArgb(209, 154, 102);
            editor.Styles[Style.Cpp.Preprocessor].ForeColor = Color.FromArgb(198, 120, 221);
            editor.Styles[Style.Cpp.Operator].ForeColor = Color.FromArgb(86, 182, 194);

            editor.Indicators[11].Style = IndicatorStyle.RoundBox;
            editor.Indicators[11].Under = true;
            editor.Margins[2].Sensitive = true;
            editor.Indicators[11].ForeColor = Color.FromArgb(80, 160, 255);
            editor.Indicators[0].OutlineAlpha = 50;
            editor.Indicators[11].Alpha = 0;
            editor.AutomaticFold = AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change;
            editor.SetProperty("fold", "1");
            editor.Margins[2].Type = MarginType.Symbol;
            editor.Margins[2].Mask = Marker.MaskFolders;

            var indicator = editor.Indicators[borderIndicator];
            indicator.Style = IndicatorStyle.RoundBox;
            indicator.ForeColor = Color.FromArgb(122, 122, 204);
            indicator.Under = false;
            indicator.Alpha = 50;
            indicator.OutlineAlpha = 250;

            editor.SetProperty("fold.compact", "1");
            editor.ViewWhitespace = WhitespaceMode.Invisible;
            editor.Styles[Style.IndentGuide].ForeColor = Color.FromArgb(80, Color.Gray);
            editor.ScrollWidth = 2;
            editor.ScrollWidthTracking = true;
            editor.EolMode = Eol.Lf;

            editor.CharAdded += editor_CharAdded;
            editor.UpdateUI += editor_UpdateUI;
            editor.KeyDown += editor_KeyDown;
            editor.KeyUp += editor_KeyUp;
            this.DragDrop += TextEditor_DragDrop;
            this.DragEnter += TextEditor_DragEnter;
            editor.MouseDown += editor_MouseDown;
            zoomDropDownButton.DropDownItemClicked += zoomDropDownButton_DropDownItemClicked;
            editor.MarginClick += (s, e) =>
            {
                if (e.Margin == 0)
                {
                    int line = editor.LineFromPosition(e.Position);
                    editor.Lines[line].MarkerAdd(0);
                }
            };

            // Set keywords
            CustomCSharpColor();
            CustomCSharpUsageColor();
            string keywords = "abstract as base add remove function bool def None nigga break byte case catch char async checked class const get set continue decimal default delegate var do double else enum event explicit extern false finally fixed float for foreach goto if partial implicit in int interface internal is lock long namespace new null object operator out override params private protected public readonly ref return sbyte sealed short sizeof stackalloc static string struct switch this throw true try typeof uint ulong unchecked unsafe ushort using virtual void volatile while";
            editor.SetKeywords(0, keywords);
        }

        void ApplyGradientBackground(Color topColor, Color bottomColor)
        {
            Color marginForeColor = editor.Styles[Style.LineNumber].ForeColor;
            for (int i = 0; i < GradientBandCount; i++)
            {
                double t = (double)i / (GradientBandCount - 1);
                Color bandColor = LerpColor(topColor, bottomColor, t);
                _gradientBandColors[i] = bandColor;

                var band = editor.Markers[GradientMarkerBase + i];
                band.Symbol = MarkerSymbol.Background;
                band.SetBackColor(bandColor);

                var marginStyle = editor.Styles[GradientMarginStyleBase + i];
                marginStyle.BackColor = bandColor;
                marginStyle.ForeColor = marginForeColor;
            }
            _lastGradientLineCount = -1;
            RefreshGradientBands();
            UpdateCaretLineHighlight();
        }

        void RefreshGradientBands()
        {
            int lineCount = editor.Lines.Count;
            if (lineCount == _lastGradientLineCount) return;

            for (int i = 0; i < GradientBandCount; i++)
                editor.MarkerDeleteAll(GradientMarkerBase + i);

            for (int ln = 0; ln < lineCount; ln++)
            {
                int band = GetGradientBand(ln);
                editor.Lines[ln].MarkerAdd(GradientMarkerBase + band);
                editor.Lines[ln].MarginStyle = GradientMarginStyleBase + band;
                editor.Lines[ln].MarginText = (ln + 1).ToString();
            }
            _lastGradientLineCount = lineCount;
            UpdateCaretLineHighlight();
        }

        static int GetGradientBand(int lineNumber)
        {
            int period = 2 * (GradientBandCount - 1);
            int posInCycle = lineNumber % period;
            return posInCycle < GradientBandCount ? posInCycle : period - posInCycle;
        }

        void UpdateCaretLineHighlight()
        {
            if (_gradientBandColors == null) return;
            int band = GetGradientBand(editor.CurrentLine);
            editor.CaretLineBackColor = LerpColor(_gradientBandColors[band], Color.White, 0.30);
        }

        static Color LerpColor(Color a, Color b, double t)
        {
            t = t < 0 ? 0 : (t > 1 ? 1 : t);
            return Color.FromArgb(
                (int)Math.Round(a.R + (b.R - a.R) * t),
                (int)Math.Round(a.G + (b.G - a.G) * t),
                (int)Math.Round(a.B + (b.B - a.B) * t));
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.SelectAll();     // select all text
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                pasteToolStripMenuItem1.Enabled = true;
                editor.Paste();     // paste text
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.Copy();      // copy text
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.Cut();     // cut text
        }

        private void boldStripButton3_Click(object sender, EventArgs e)
        {
            InitEditor();
            editor.Styles[Style.Default].Bold = !editor.Styles[Style.Default].Bold;
            editor.StyleClearAll();
            ReInitEditor();
        }

        private void underlineStripButton_Click(object sender, EventArgs e)
        {
            InitEditor();
            editor.Styles[Style.Default].Underline = true;
            editor.StyleClearAll();
            ReInitEditor();
        }

        private void italicStripButton_Click(object sender, EventArgs e)
        {
            InitEditor();
            editor.Styles[Style.Default].Italic = true;
            editor.StyleClearAll();
            ReInitEditor();
        }

        private void editor_UpdateUI(object sender, UpdateUIEventArgs e)
        {
            UpdateCaretLineHighlight();
            if ((e.Change & UpdateChange.Content) > 0)
            {
                if (editor.SelectionStart != editor.SelectionEnd)
                {
                    int start = editor.SelectionStart;
                    int length = editor.SelectionEnd - start;
                    editor.IndicatorCurrent = borderIndicator;
                    editor.IndicatorFillRange(start, length);
                }

                int pos = editor.CurrentPosition;
                int line = editor.LineFromPosition(pos);
                int column = pos - editor.Lines[line].Position;
                lineColumnStatusLabel.Text = $"Ln: {line + 1}, Ch: {column + 1} ";
            }
            if ((e.Change & UpdateChange.Selection) > 0)
            {
                editor.IndicatorCurrent = borderIndicator;
                editor.IndicatorClearRange(0, editor.TextLength);


                if (editor.SelectionStart != editor.SelectionEnd)
                {
                    int start = editor.SelectionStart;
                    int length = editor.SelectionEnd - start;
                    editor.IndicatorFillRange(start, length);
                }
            }
        }

        private void fontStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            editor.StyleResetDefault();
            editor.Font = new Font(fontSizeComboBox.Text, editor.Font.Size, editor.Font.Style);
            editor.StyleClearAll();
            ReInitEditor();
        }

        private void saveStripButton_Click(object sender, EventArgs e)
        {
            SaveCurrentFile();
        }

        private void openFileStripButton_Click(object sender, EventArgs e)
        {
            OpenMenuItem_Click(sender, e);
        }

        private void increaseStripButton_Click(object sender, EventArgs e)
        {
            currentFontSize += 1;
            ApplyFontSize();
        }

        void ApplyFontSize()
        {
            editor.StyleResetDefault();
            editor.Styles[Style.Default].BackColor = Color.FromArgb(34, 30, 34);
            editor.Styles[Style.Default].ForeColor = Color.FromArgb(220, 220, 220);
            editor.Styles[Style.Default].Font = Config.FontFamily;
            editor.Styles[Style.Default].Size = currentFontSize;
            Config.FontSize = currentFontSize;
            Config.Save();
            editor.StyleClearAll();
            editor.Styles[Style.LineNumber].BackColor = Color.FromArgb(34, 30, 34);
            editor.Styles[Style.LineNumber].ForeColor = Color.FromArgb(120, 120, 120);
            editor.Margins[0].BackColor = Color.FromArgb(34, 30, 34);
            editor.Margins[0].Type = MarginType.RightText;
            editor.Margins[0].Width = Config.ShowLineNumbers ? 40 : 0;
            editor.Markers[1].Symbol = MarkerSymbol.Circle;
            editor.Indicators[0].Style = IndicatorStyle.CompositionThin;
            editor.Indicators[0].ForeColor = Color.Red;
            editor.BraceBadLight(pos);
            editor.Styles[Style.LineNumber].ForeColor = Color.FromArgb(150, 150, 160);
            editor.CaretForeColor = Color.White;
            editor.SetSelectionBackColor(true, Color.FromArgb(60, 90, 140));
            editor.IndicatorClearRange(0, editor.TextLength);
            ApplySyntaxColors();
            editor.Indicators[11].Style = IndicatorStyle.FullBox;
            editor.Indicators[11].Under = true;
            editor.Indicators[11].ForeColor = Color.FromArgb(80, 160, 255);
            editor.Indicators[0].OutlineAlpha = 255;
            editor.Indicators[11].Alpha = 0;
            editor.AutomaticFold = AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change;
            editor.SetProperty("fold", "1");
            editor.SetProperty("fold.compact", "1");
            ApplyWhitespaceSetting();
            CustomCSharpColor();
            CustomCSharpUsageColor();
            ApplyGradientSetting();
        }

        private void decreaseStripButton_Click(object sender, EventArgs e)
        {
            if (currentFontSize > 6)
            {
                currentFontSize -= 1;
                ApplyFontSize();
            }
        }

        private void TextEditor_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void TextEditor_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 0)
                return;
            OpenFile(files[0]);
        }

        private void undoStripButton_Click(object sender, EventArgs e)
        {
            editor.Undo();     // undo move
        }

        private void redoStripButton_Click(object sender, EventArgs e)
        {
            editor.Redo();    // redo move
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.Undo();     // undo move
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.Redo();     // redo move
        }

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            editor.Cut();     // cut text
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            editor.Copy();     // copy text
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                pasteStripMenuItem.Enabled = true;
                editor.Paste();    // paste text
            }
        }

        private void selectAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            editor.SelectAll();    // select all text
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.ClearAll();
            editor.Focus();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (editor.SelectionStart != editor.SelectionEnd)
            {
                // delete the selected text
                editor.DeleteRange(editor.SelectionStart, editor.SelectionEnd - editor.SelectionStart);
            }
            else
            {
                // no selection: delete the rest of the current line from the caret
                int pos = editor.CurrentPosition;
                int line = editor.CurrentLine;
                int end = editor.Lines[line].EndPosition;
                if (end > pos)
                    editor.DeleteRange(pos, end - pos);
            }
        }

        public async void OpenMenuItem_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.CheckFileExists = true;
                ofd.RestoreDirectory = true;
                ofd.Filter = "Source Files (*.cs;*.cpp;*.js;*.h;*.html;*.py;*.css;*.txt;*.config;*.java;*.php;*.sln;*.swift;*.go;*.rs;*.ts;*.kt;*.hpp)|" + "*.cs; *.cpp; *.js; *.h; *.html; *.py; *.css; *.txt; *.config; *.java; *.php; *.sln; *.swift; *.go; *.rs; *.ts; *.kt; *.hpp|" + "All Files (*.*)|*.*";
                ofd.Title = "Open a file - Textify Editor";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    fileName = ofd.FileName;
                    SetStatus(Path.GetFileName(fileName));
                    RecentFileManager.Add(fileName);

                    OpenFile(fileName);
                }
            }
        }

        public void NewDoc(string title, string text, string lang)
        {
            this.Text = title;
            editor.Text = text;
            StartWithoutCode_();
        }

        public async void OpenFile(string path)
        {
            try
            {
                SetStatus(Path.GetFileName(path));
                label1.Hide();
                createNewFile.Hide();
                label3.Hide();
                toolStrip1.Visible = true;
                label4.Hide();
                label2.Hide();
                label5.Hide();
                textifyPicture.Hide();
                StartWithoutCode.Hide();
                recentList.Hide();
                statusStrip1.Visible = true;
                textifyPicture.Hide();
                fileName = path;
                await UpdateFileMenuStatus(true);
                lineColumnStatusLabel.Visible = true;
                toolStrip1.Enabled = true;
                statusStrip1.Visible = true;
                editor.Visible = true;
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fs, true))
                {
                    editor.Text = reader.ReadToEnd();
                }
                editor.EmptyUndoBuffer();
                editor.ReadOnly = Config.OpenFilesReadOnly;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void newMenuItem_Click(object sender, EventArgs e)
        {
            if (editor.Text != string.Empty)    // editor has contents - prompt user to save changes
            {
                // save changes message
                DialogResult result = Messenger.MessageBox("Would you like to save your changes? Editor is not empty.", "Save Changes?", MessageBoxButtons.YesNoCancel, MsgIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    string file;
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string filename = saveFileDialog1.FileName;
                        if (string.IsNullOrEmpty(filename)) { SaveCurrentFileAs(); return; }
                        File.WriteAllText(filename, editor.Text, Encoding.UTF8);
                        file = Path.GetFileName(filename); // get name of file
                        Messenger.MessageBox("File " + filename + " was saved successfully.", "Save Successful", MessageBoxButtons.OK, MsgIcon.Info);

                        // only clear the editor once the save actually succeeded
                        editor.ResetText();
                        editor.Focus();
                    }
                    // if the save dialog was cancelled, leave the unsaved text in place
                }
                else if (result == DialogResult.No)
                {
                    editor.ResetText();
                    editor.Focus();
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCurrentFile();
        }

        void SaveCurrentFile()
        {
            try
            {
                File.WriteAllText(fileName, editor.Text, Encoding.UTF8);
                string file = Path.GetFullPath(fileName).ToString();
                Messenger.MessageBox($"Done! {file}", "Success", MessageBoxButtons.OK, MsgIcon.Success);
                editor.SetSavePoint();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save file:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void SaveCurrentFileAs()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "All Files|*.*";
                sfd.DefaultExt = ".txt";
                sfd.Title = "Save As - Textify Editor";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    fileName = sfd.FileName;
                    SaveCurrentFile();
                }
            }
        }

        void SetZoomPercent(int percent)
        {
            int zoom = (percent - 100) / 10;
            zoom = Math.Max(-10, Math.Min(20, zoom));
            editor.Zoom = zoom;
        }

        private void zoomDropDownButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (int.TryParse(e.ClickedItem.Text.Replace("%", ""), out int percent))
            {
                SetZoomPercent(percent);
                zoomDropDownButton.Text = percent + "%";
            }
        }

        private void zoomDropDown_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (int.TryParse(e.ClickedItem.Text.Replace("%", ""), out int percent))
            {
                SetZoomPercent(percent);
                zoomDropDownButton.Text = percent + "%";
            }
        }

        private void uppercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.ReplaceSelection(editor.SelectedText.ToUpper());    // text to CAPS
        }

        private void lowercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.ReplaceSelection(editor.SelectedText.ToLower());    // text to lowercase
        }

        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.WordWrap = !Config.WordWrap;
            ApplyWordWrap();
            Config.Save();
            wordWrapToolStripMenuItem.Checked = Config.WordWrap;
        }

        void ReInitEditor()
        {
            editor.Styles[Style.LineNumber].BackColor = Color.FromArgb(34, 30, 34);
            editor.Styles[Style.LineNumber].ForeColor = Color.FromArgb(120, 120, 120);
            editor.Margins[0].BackColor = Color.FromArgb(34, 30, 34);
            editor.Margins[0].Type = MarginType.RightText;
            editor.Margins[0].Width = Config.ShowLineNumbers ? 40 : 0;
            editor.Markers[1].Symbol = MarkerSymbol.Circle;
            editor.Indicators[0].Style = IndicatorStyle.CompositionThin;
            editor.Indicators[0].ForeColor = Color.Red;
            editor.BraceBadLight(pos);
            editor.Styles[Style.LineNumber].ForeColor = Color.FromArgb(150, 150, 160);
            editor.CaretForeColor = Color.White;
            //editor.CaretLineBackColor = Color.FromArgb(30, 255, 255, 255);
            editor.SetSelectionBackColor(true, Color.FromArgb(60, 90, 140));
            editor.IndicatorClearRange(0, editor.TextLength);
            ApplySyntaxColors();
            editor.Indicators[11].Style = IndicatorStyle.FullBox;
            editor.Indicators[11].Under = true;
            editor.Indicators[11].ForeColor = Color.FromArgb(80, 160, 255);
            editor.Indicators[0].OutlineAlpha = 255;
            editor.Indicators[11].Alpha = 0;
            editor.AutomaticFold = AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change;
            editor.SetProperty("fold", "1");
            editor.SetProperty("fold.compact", "1");
            ApplyWhitespaceSetting();
            CustomCSharpColor();
            CustomCSharpUsageColor();

            // Every caller runs StyleClearAll() right before ReInitEditor(), which
            // wipes the custom margin gradient styles - redefine them here too.
            //ApplyGradientSetting();
        }

        void ApplyTextColor(Color color)
        {
            editor.Styles[Style.Default].ForeColor = color;
            editor.Styles[Style.LineNumber].ForeColor = color;
            editor.StyleClearAll();
            ReInitEditor();
        }

        private void clearFormattingStripButton_Click(object sender, EventArgs e)
        {
            editor.Styles[Style.Default].ForeColor = Color.FromArgb(220, 220, 220);
            editor.Styles[Style.Default].Font = "Cascadia Mono";    // set default font     
        }

        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // draws the string onto the print document
            e.Graphics.DrawString(editor.Text, editor.Font, Brushes.Black, 100, 20);
            e.Graphics.PageUnit = GraphicsUnit.Inch;
        }

        private void printStripButton_Click(object sender, EventArgs e)
        {
            // printDialog associates with PrintDocument
            printDialog.Document = printDocument;
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print(); // Print the document
            }
        }

        private void printPreviewStripButton_Click(object sender, EventArgs e)
        {
            printPreviewDialog.Document = printDocument;
            // Show PrintPreview Dialog 
            printPreviewDialog.ShowDialog();
        }

        private void printStripMenuItem_Click(object sender, EventArgs e)
        {
            // printDialog associates with PrintDocument
            printDialog.Document = printDocument;
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }

        private void printPreviewStripMenuItem_Click(object sender, EventArgs e)
        {
            printPreviewDialog.Document = printDocument;
            // Show PrintPreview Dialog 
            printPreviewDialog.ShowDialog();
        }

        private void colorOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                ApplyTextColor(colorDialog1.Color);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowFindReplaceDialog();
        }

        void ShowFindReplaceDialog()
        {
            if (_findReplaceDialog != null && !_findReplaceDialog.IsDisposed)
            {
                _findReplaceDialog.Activate();
                _findBox.Focus();
                return;
            }

            var dlg = new Form
            {
                Text = "Find & Replace",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false,
                ClientSize = new Size(360, 172),
                BackColor = Color.FromArgb(34, 30, 34)
            };

            var findLabel = new Label { Text = "Find:", ForeColor = Color.Gainsboro, AutoSize = true, Location = new Point(12, 15) };
            var findBox = new TextBox { Location = new Point(90, 12), Width = 258 };

            var replaceLabel = new Label { Text = "Replace:", ForeColor = Color.Gainsboro, AutoSize = true, Location = new Point(12, 45) };
            var replaceBox = new TextBox { Location = new Point(90, 42), Width = 258 };

            var matchCase = new CheckBox { Text = "Match case", ForeColor = Color.Gainsboro, AutoSize = true, Location = new Point(90, 72) };

            var findNext = new Button { Text = "Find Next", Location = new Point(12, 106), Width = 100, Height = 30};
            var replaceOne = new Button { Text = "Replace", Location = new Point(120, 106), Width = 100, Height = 30};
            var replaceAll = new Button { Text = "Replace All", Location = new Point(228, 106), Width = 100, Height = 30};

            findNext.Click += (s, e2) => FindNext(findBox.Text, matchCase.Checked);
            replaceOne.Click += (s, e2) => ReplaceCurrent(findBox.Text, replaceBox.Text, matchCase.Checked);
            replaceAll.Click += (s, e2) => ReplaceAll(findBox.Text, replaceBox.Text, matchCase.Checked);

            dlg.Controls.Add(findLabel);
            dlg.Controls.Add(findBox);
            dlg.Controls.Add(replaceLabel);
            dlg.Controls.Add(replaceBox);
            dlg.Controls.Add(matchCase);
            dlg.Controls.Add(findNext);
            dlg.Controls.Add(replaceOne);
            dlg.Controls.Add(replaceAll);
            dlg.AcceptButton = findNext;

            _findReplaceDialog = dlg;
            _findBox = findBox;
            _replaceBox = replaceBox;
            _matchCaseBox = matchCase;

            dm = new DarkModeCS(dlg)
            {
                ColorMode = DarkModeCS.DisplayMode.DarkMode
            };
            dlg.Show(this);
            findBox.Focus();
        }

        void FindNext(string query, bool matchCase)
        {
            if (string.IsNullOrEmpty(query)) return;

            string text = editor.Text;
            var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            int searchFrom = Math.Min(editor.SelectionEnd, text.Length);

            int index = text.IndexOf(query, searchFrom, comparison);
            if (index < 0 && searchFrom > 0)
                index = text.IndexOf(query, 0, comparison); // wrap around to the start

            if (index < 0)
            {
                //Messenger.MessageBox($"\"{query}\" was not found.", "Find & Replace", MessageBoxButtons.OK, MsgIcon.Info);
                return;
            }

            editor.SetSelection(index, index + query.Length);
            editor.ScrollCaret();
            editor.Focus();
        }

        void ReplaceCurrent(string query, string replacement, bool matchCase)
        {
            if (string.IsNullOrEmpty(query)) return;

            var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            if (editor.SelectionStart != editor.SelectionEnd &&
                string.Equals(editor.SelectedText, query, comparison))
            {
                editor.BeginUndoAction();
                editor.ReplaceSelection(replacement ?? string.Empty);
                editor.EndUndoAction();
            }
            FindNext(query, matchCase);
        }

        void ReplaceAll(string query, string replacement, bool matchCase)
        {
            if (string.IsNullOrEmpty(query)) return;

            replacement = replacement ?? string.Empty;
            var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            string text = editor.Text;
            var sb = new StringBuilder();
            int pos = 0;
            int count = 0;

            while (true)
            {
                int index = text.IndexOf(query, pos, comparison);
                if (index < 0)
                {
                    sb.Append(text, pos, text.Length - pos);
                    break;
                }
                sb.Append(text, pos, index - pos);
                sb.Append(replacement);
                pos = index + query.Length;
                count++;
            }

            if (count > 0)
            {
                editor.BeginUndoAction();
                editor.Text = sb.ToString();
                editor.EndUndoAction();
            }

            Messenger.MessageBox($"Replaced {count} occurrence(s).", "Replace All", MessageBoxButtons.OK, MsgIcon.Info);
        }

        async Task UpdateFileMenuStatus(bool fileOpen)
        {
            // Enable or disable menu items based on fileOpen status
            printStripMenuItem.Enabled = fileOpen;
            saveToolStripMenuItem.Enabled = fileOpen;
            printPreviewStripMenuItem.Enabled = fileOpen;
            runToolStripMenuItem.Enabled = fileOpen;
            findReplaceItem.Enabled = fileOpen;
            copyToolStripMenuItem1.Enabled = fileOpen;
            readOnlyMode.Enabled = fileOpen;
            undoToolStripMenuItem.Enabled = fileOpen;
            redoToolStripMenuItem.Enabled = fileOpen;
            copyToolStripMenuItem1.Enabled = fileOpen;
            pasteToolStripMenuItem1.Enabled = fileOpen;
            deleteToolStripMenuItem.Enabled = fileOpen;
            selectAllToolStripMenuItem1.Enabled = fileOpen;
            clearAllToolStripMenuItem.Enabled = fileOpen;
            saveAs.Enabled = fileOpen;
            newMenuItem.Enabled = fileOpen;
            cutToolStripMenuItem1.Enabled = fileOpen;
            wordWrapToolStripMenuItem.Enabled = fileOpen;
            fontToolStripMenuItem.Enabled = fileOpen;
            colorOptionsToolStripMenuItem.Enabled = fileOpen;
            closeCurrentFile.Enabled = fileOpen;
            pageSetupToolStripMenuItem.Enabled = fileOpen;
            goToLineToolStripMenuItem.Enabled = fileOpen;
            duplicateLineToolStripMenuItem.Enabled = fileOpen;
            deleteLineToolStripMenuItem.Enabled = fileOpen;
            moveLineUpToolStripMenuItem.Enabled = fileOpen;
            moveLineDownToolStripMenuItem.Enabled = fileOpen;
            toggleCommentToolStripMenuItem.Enabled = fileOpen;
            showLineNumbersToolStripMenuItem.Enabled = fileOpen;
            showWhitespaceToolStripMenuItem.Enabled = fileOpen;
            showIndentGuidesToolStripMenuItem.Enabled = fileOpen;
            zoomInToolStripMenuItem.Enabled = fileOpen;
            zoomOutToolStripMenuItem.Enabled = fileOpen;
            resetZoomToolStripMenuItem.Enabled = fileOpen;
            wordCountToolStripMenuItem.Enabled = fileOpen;
            insertDateTimeToolStripMenuItem.Enabled = fileOpen;
            trimTrailingWhitespaceToolStripMenuItem.Enabled = fileOpen;
            convertTabsToSpacesToolStripMenuItem.Enabled = fileOpen;
            convertSpacesToTabsToolStripMenuItem.Enabled = fileOpen;
            sortLinesToolStripMenuItem.Enabled = fileOpen;

            showLineNumbersToolStripMenuItem.Checked = Config.ShowLineNumbers;
            showWhitespaceToolStripMenuItem.Checked = Config.ShowWhitespace;
            showIndentGuidesToolStripMenuItem.Checked = Config.ShowIndentGuides;
        }

        private void saveAs_Click(object sender, EventArgs e)
        {
            SaveCurrentFileAs();
        }

        /*protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (var brush = new LinearGradientBrush(
                ClientRectangle,
                Color.FromArgb(0, 30, 32),
                Color.FromArgb(0, 32, 60),
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }
        }*/

        private void createNewFile_Click(object sender, EventArgs e)
        {
            StartWithoutCode_();
        }

        void SetStatus(string message) // current file status (opened or not)
        {
            if (message != null)
            {
                Text = message + " - Textify Editor";
            }
            else
            {
                Text = "Textify Editor";
            }
        }

        void CustomCSharpColor() // C# VS keywords 
        {
            editor.SetKeywords(1, "Task EventArgs KeyPressEventArgs ToolStripItemClickedEventArgs WriteLine ReadKey KeyEventArgs ToolStrip LinearGradientBrush FormClosingEventArgs CancelEventArgs MouseEventArgs PaintEventArgs Brushes PrintPageEventArgs Encoding Control MessageBox Exception InstalledFontCollection PropertyInfo ToolStripItem Form");
            editor.Styles[Style.Cpp.Word2].ForeColor = Color.LightSeaGreen;
        }

        void CustomCSharpUsageColor() // Other C# VS keywords 
        {
            editor.SetKeywords(1, "FileShare GraphicsPath ImageRectangle Visible DrawRectangle TryParse DrawLine Replace Length Horizontal Width Height Rectangle UpdateStyles ContainsKey $ SuppressKeyPress Focus ControlStyles Application Copy Paste Cut Message OnActivated InitializeComponent OnDeactivate Clear OnPaint ToLowerInvariant DialogResult Print MessageBoxButtons MessageBoxIcon ToLower ToUpper ToString FileAccess FileMode MessageBox WriteAllText ReadAllText FromArgb FillRectangle Hide Show Close ShowDialog TryGetValue FindAll Exists Add GetFileName GetFullPath OnFormClosing OnFormClosed ToWin32");
            editor.Styles[Style.Cpp.Word2].ForeColor = Color.Tan;
        }

        public async void StartWithoutCode_()
        {
            label1.Hide();
            createNewFile.Hide();
            label2.Hide();
            toolStrip1.Visible = true;
            label4.Hide();
            label5.Hide();
            label3.Hide();
            textifyPicture.Hide();
            StartWithoutCode.Hide();
            recentList.Hide();
            await UpdateFileMenuStatus(true);
            toolStrip1.Enabled = true;
            saveStripButton.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            statusStrip1.Visible = true;
            editor.ResetText();
            editor.Visible = true;
            editor.Focus();
        }

        private async void StartWithoutCode_Click(object sender, EventArgs e)
        {
            StartWithoutCode_();
        }

        private void label2_Click_1(object sender, EventArgs e)
        {
            OpenMenuItem_Click(sender, e);
        }

        private void enableReadOnly_Click(object sender, EventArgs e)
        {
            editor.ReadOnly = true;
        }

        private void disableReadOnly_Click(object sender, EventArgs e)
        {
            editor.ReadOnly = false;
        }

        private void closeCurrentFile_Click(object sender, EventArgs e)
        {
            CloseFile();
            StartEditor se = new StartEditor();
            se.ShowDialog();
        }

        async void CloseFile()
        {
            fileName = string.Empty;
            editor.ResetText();
            SetStatus(null);
            label1.Show();
            createNewFile.Show();
            label2.Show();
            toolStrip1.Visible = false;
            label4.Show();
            label5.Show();
            label3.Show();
            textifyPicture.Show();
            StartWithoutCode.Show();
            recentList.Show();
            await UpdateFileMenuStatus(false);
            toolStrip1.Enabled = false;
            saveStripButton.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            statusStrip1.Visible = false;
            editor.Visible = false;
            LoadRecentFiles();
        }

        public void LoadRecentFiles()
        {
            var list = RecentFileManager.GetRecentFiles();

            recentList.Items.Clear();

            list = list.FindAll(f => File.Exists(f));

            if (list.Count > Config.MaxRecentFiles)
                list = list.Take(Config.MaxRecentFiles).ToList();

            if (list.Count == 0)
            {
                recentList.Enabled = false;
                recentList.Items.Add(new RecentFileItem
                {
                    FileName = "No recent files",
                    FullPath = ""
                });
                recentList.SelectedIndex = -1;
                PopulateOpenRecentMenu(list);
                return;
            }

            recentList.Enabled = true;

            foreach (string file in list)
            {
                recentList.Items.Add(new RecentFileItem
                {
                    FileName = Path.GetFileName(file),
                    FullPath = file
                });
            }

            recentList.SelectedIndex = -1;
            PopulateOpenRecentMenu(list);
        }

        // Rebuilds the File > Open Recent submenu from the same list backing recentList.
        void PopulateOpenRecentMenu(List<string> list)
        {
            openRecentToolStripMenuItem.DropDownItems.Clear();

            if (list == null || list.Count == 0)
            {
                openRecentToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem("No recent files") { Enabled = false });
                return;
            }

            foreach (string file in list)
            {
                string path = file; // capture for closure
                var item = new ToolStripMenuItem(Path.GetFileName(path)) { ToolTipText = path };
                item.Click += (s, e) =>
                {
                    if (!File.Exists(path))
                    {
                        Messenger.MessageBox("This file no longer exists:\n" + path, "Missing file", MessageBoxButtons.OK, MsgIcon.Warning);
                        LoadRecentFiles();
                        return;
                    }
                    fileName = path;
                    RecentFileManager.Add(path);
                    OpenFile(path);
                };
                openRecentToolStripMenuItem.DropDownItems.Add(item);
            }

            openRecentToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            var clearItem = new ToolStripMenuItem("Clear Recent Files");
            clearItem.Click += (s, e) => ClearRecentFiles();
            openRecentToolStripMenuItem.DropDownItems.Add(clearItem);
        }

        private void recentList_DoubleClick(object sender, EventArgs e)
        {
            if (!recentList.Enabled) return;
            if (recentList.SelectedIndex == -1) return;

            // Get the actual RecentFileItem object
            var item = recentList.SelectedItem as RecentFileItem;
            if (item == null) return;

            string arcPath = item.FullPath;
            if (string.IsNullOrWhiteSpace(arcPath))
                return;

            if (!File.Exists(arcPath))
            {
                DialogResult result = Messenger.MessageBox(
                    "This file no longer exists, Would you like to Remove it?",
                    "Missing file",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.None);

                if (result == DialogResult.Yes)
                {
                    recentList.Items.Remove(item);
                }

                LoadRecentFiles(); // refresh the list
                return;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Settings s = new Settings(this))
            {
                s.ShowDialog(this);
            }
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            output.Show();
            RunWebProject(fileName);
        }

        void AppendOutput(string text, Color color)
        {
            output.SelectionStart = output.TextLength;
            output.SelectionLength = 0;
            output.SelectionColor = color;
            output.AppendText(text + Environment.NewLine);
            output.SelectionColor = output.ForeColor;
        }

        void RunWebProject(string htmlFile)
        {
            if (!File.Exists(htmlFile))
            {
                AppendOutput("Error: HTML file not found.", Color.Red);
                return;
            }
            string ext = Path.GetExtension(fileName).ToLower();
            if (ext != ".html" && ext != ".htm")
            {
                AppendOutput("Warning: Only HTML files can be run.", Color.Orange);
                AppendOutput("Please open an HTML file.", Color.Gray);
                return;
            }
            AppendOutput($"Build started at {DateTime.Now:HH:mm:ss}...", Color.LightGray);
            AppendOutput("Launching browser...", Color.White);
            Process.Start(new ProcessStartInfo()
            {
                FileName = htmlFile,
                UseShellExecute = true
            });
            AppendOutput("Build succeeded.", Color.LightGreen);
        }

        private void editor_KeyUp(object sender, KeyEventArgs e)
        {
            // determine key released
            switch (e.KeyCode)
            {
                case Keys.Down:
                    pos = editor.CurrentPosition;    // get starting point
                    line = editor.LineFromPosition(pos);    // get line number
                    column = editor.SelectionStart - editor.Lines[line].Position;    // get column number
                    lineColumnStatusLabel.Text = "Ln: " + (line + 1) + ", Ch: " + (column + 1);
                    break;
                case Keys.Right:
                    pos = editor.CurrentPosition; // get starting point
                    line = editor.LineFromPosition(pos); // get line number
                    column = editor.SelectionStart - editor.Lines[line].Position;    // get column number
                    lineColumnStatusLabel.Text = "Ln: " + (line + 1) + ", Ch: " + (column + 1);
                    break;
                case Keys.Up:
                    pos = editor.CurrentPosition; // get starting point
                    line = editor.LineFromPosition(pos); // get line number
                    column = editor.SelectionStart - editor.Lines[line].Position;    // get column number
                    lineColumnStatusLabel.Text = "Ln: " + (line + 1) + ", Ch: " + (column + 1);
                    break;
                case Keys.Left:
                    pos = editor.CurrentPosition; // get starting point
                    line = editor.LineFromPosition(pos); // get line number
                    column = editor.SelectionStart - editor.Lines[line].Position;    // get column number
                    lineColumnStatusLabel.Text = "Ln: " + (line + 1) + ", Ch: " + (column + 1);
                    break;
            }
        }

        private void editor_MouseDown(object sender, MouseEventArgs e)
        {
            int pos = editor.CurrentPosition;
            int line = editor.LineFromPosition(pos);
            int column = pos - editor.Lines[line].Position;
            lineColumnStatusLabel.Text = $"Ln: {line + 1}, Ch: {column + 1}";
        }

        #region File menu additions

        private void pageSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pageSetupDialog1.Document = printDocument;
            pageSetupDialog1.ShowDialog(this);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        #region Edit menu additions

        private void goToLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int totalLines = editor.Lines.Count;
            int current = editor.LineFromPosition(editor.CurrentPosition) + 1;

            using (var dlg = new Form())
            {
                dlg.Text = "Go to Line";
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                dlg.ShowInTaskbar = false;
                dlg.ClientSize = new Size(260, 110);
                dlg.BackColor = Color.FromArgb(34, 30, 34);

                var label = new Label
                {
                    Text = $"Line number (1 - {totalLines}):",
                    ForeColor = Color.Gainsboro,
                    AutoSize = true,
                    Location = new Point(12, 14)
                };

                var input = new NumericUpDown
                {
                    Minimum = 1,
                    Maximum = Math.Max(1, totalLines),
                    Location = new Point(12, 38),
                    Width = 236
                };
                input.Value = Math.Min(Math.Max(current, 1), input.Maximum);

                var ok = new Button { Text = "Go", DialogResult = DialogResult.OK, Location = new Point(92, 72) };
                var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(173, 72) };

                dlg.Controls.Add(label);
                dlg.Controls.Add(input);
                dlg.Controls.Add(ok);
                dlg.Controls.Add(cancel);
                dlg.AcceptButton = ok;
                dlg.CancelButton = cancel;

                dm = new DarkModeCS(dlg)
                {
                    ColorMode = DarkModeCS.DisplayMode.DarkMode
                };

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    int target = Math.Max(0, Math.Min((int)input.Value - 1, totalLines - 1));
                    int pos = editor.Lines[target].Position;
                    editor.GotoPosition(pos);
                    editor.ScrollCaret();
                    editor.Focus();
                }
            }
        }

        private void duplicateLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.ExecuteCmd(Command.LineDuplicate);
            editor.Focus();
        }

        private void deleteLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int lineIndex = editor.LineFromPosition(editor.CurrentPosition);
            var ln = editor.Lines[lineIndex];
            editor.BeginUndoAction();
            editor.DeleteRange(ln.Position, ln.Text.Length);
            editor.EndUndoAction();
            editor.Focus();
        }

        private void moveLineUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.ExecuteCmd(Command.MoveSelectedLinesUp);
            editor.Focus();
        }

        private void moveLineDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.ExecuteCmd(Command.MoveSelectedLinesDown);
            editor.Focus();
        }

        private void toggleCommentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int startLine = editor.LineFromPosition(editor.SelectionStart);
            int endLine = editor.LineFromPosition(editor.SelectionEnd);

            string firstTrimmed = editor.Lines[startLine].Text.TrimStart();
            bool shouldComment = !firstTrimmed.StartsWith("//");

            editor.BeginUndoAction();
            for (int i = startLine; i <= endLine; i++)
            {
                var ln = editor.Lines[i];
                string text = ln.Text;
                string trimmed = text.TrimStart();
                int indent = text.Length - trimmed.Length;

                if (shouldComment)
                {
                    if (trimmed.Length == 0) continue; // skip blank lines
                    editor.InsertText(ln.Position + indent, "// ");
                }
                else
                {
                    if (trimmed.StartsWith("// "))
                        editor.DeleteRange(ln.Position + indent, 3);
                    else if (trimmed.StartsWith("//"))
                        editor.DeleteRange(ln.Position + indent, 2);
                }
            }
            editor.EndUndoAction();
            editor.Focus();
        }

        #endregion

        #region View menu additions

        private void showLineNumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.ShowLineNumbers = showLineNumbersToolStripMenuItem.Checked;
            ApplyLineNumbersSetting();
            Config.Save();
        }

        private void showWhitespaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.ShowWhitespace = showWhitespaceToolStripMenuItem.Checked;
            ApplyWhitespaceSetting();
            Config.Save();
        }

        private void showIndentGuidesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.ShowIndentGuides = showIndentGuidesToolStripMenuItem.Checked;
            ApplyIndentGuidesSetting();
            Config.Save();
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int newZoom = Math.Min(20, editor.Zoom + 1);
            editor.Zoom = newZoom;
            zoomDropDownButton.Text = (100 + newZoom * 10) + " %";
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int newZoom = Math.Max(-10, editor.Zoom - 1);
            editor.Zoom = newZoom;
            zoomDropDownButton.Text = (100 + newZoom * 10) + " %";
        }

        private void resetZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetZoomPercent(Config.DefaultZoomPercent);
            zoomDropDownButton.Text = Config.DefaultZoomPercent + " %";
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                fontDialog1.Font = new Font(Config.FontFamily, currentFontSize);
            }
            catch
            {
                fontDialog1.Font = editor.Font;
            }

            if (fontDialog1.ShowDialog(this) == DialogResult.OK)
            {
                Config.FontFamily = fontDialog1.Font.Name;
                currentFontSize = (int)fontDialog1.Font.Size;
                ApplyFontSize();
                Config.Save();
            }
        }

        private void alwaysOnTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = alwaysOnTopToolStripMenuItem.Checked;
        }

        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_isFullScreen)
            {
                _prevBorderStyle = this.FormBorderStyle;
                _prevWindowState = this.WindowState;
                _prevBounds = this.Bounds;

                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Bounds = Screen.FromControl(this).Bounds;
                _isFullScreen = true;
                fullScreenToolStripMenuItem.Checked = true;
            }
            else
            {
                this.FormBorderStyle = _prevBorderStyle;
                this.WindowState = _prevWindowState;
                this.Bounds = _prevBounds;
                _isFullScreen = false;
                fullScreenToolStripMenuItem.Checked = false;
            }
        }

        #endregion

        #region Tools menu

        private void wordCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = editor.Text;
            int chars = text.Length;
            int charsNoSpaces = text.Count(c => !char.IsWhiteSpace(c));
            int words = text.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            int lines = editor.Lines.Count;

            string selText = editor.SelectedText ?? string.Empty;
            int selChars = selText.Length;

            string message = $"Lines: {lines}\nWords: {words}\nCharacters: {chars}\nCharacters (no spaces): {charsNoSpaces}";
            if (selChars > 0)
            {
                int selWords = selText.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
                message += $"\n\nSelection: {selWords} word(s), {selChars} character(s)";
            }

            Messenger.MessageBox(message, "Word Count", MessageBoxButtons.OK, MsgIcon.Info);
        }

        private void insertDateTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string stamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            int pos = editor.CurrentPosition;
            editor.BeginUndoAction();
            editor.InsertText(pos, stamp);
            editor.EndUndoAction();
            editor.SetSelection(pos + stamp.Length, pos + stamp.Length);
            editor.Focus();
        }

        private void trimTrailingWhitespaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.BeginUndoAction();
            for (int i = editor.Lines.Count - 1; i >= 0; i--)
            {
                var ln = editor.Lines[i];
                string text = ln.Text.TrimEnd('\r', '\n');
                string trimmed = text.TrimEnd(' ', '\t');
                if (trimmed.Length != text.Length)
                {
                    int deleteStart = ln.Position + trimmed.Length;
                    int deleteLength = text.Length - trimmed.Length;
                    editor.DeleteRange(deleteStart, deleteLength);
                }
            }
            editor.EndUndoAction();
        }

        private void convertTabsToSpacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string spaces = new string(' ', Math.Max(1, editor.TabWidth));
            string newText = editor.Text.Replace("\t", spaces);
            if (newText == editor.Text) return;
            editor.BeginUndoAction();
            editor.Text = newText;
            editor.EndUndoAction();
        }

        private void convertSpacesToTabsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string spaces = new string(' ', Math.Max(1, editor.TabWidth));
            string newText = editor.Text.Replace(spaces, "\t");
            if (newText == editor.Text) return;
            editor.BeginUndoAction();
            editor.Text = newText;
            editor.EndUndoAction();
        }

        private void sortLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var lines = editor.Text.Replace("\r\n", "\n").Split('\n').ToList();
            lines.Sort(StringComparer.OrdinalIgnoreCase);
            string joined = string.Join(Environment.NewLine, lines);
            if (joined == editor.Text) return;
            editor.BeginUndoAction();
            editor.Text = joined;
            editor.EndUndoAction();
        }

        private void clearRecentFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearRecentFiles();
        }

        #endregion

        #region Run menu additions

        private void clearOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            output.Clear();
            output.Hide();
        }

        #endregion

        #region Help menu additions

        private void keyboardShortcutsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string shortcuts =
                "File\n" +
                "  Ctrl+N            New File\n" +
                "  Ctrl+O            Open File\n" +
                "  Ctrl+S            Save\n" +
                "  Ctrl+Shift+S      Save As\n" +
                "  Ctrl+P            Print\n\n" +
                "Edit\n" +
                "  Ctrl+Z / Ctrl+Y   Undo / Redo\n" +
                "  Ctrl+X/C/V        Cut / Copy / Paste\n" +
                "  Ctrl+F            Find & Replace\n" +
                "  Ctrl+G            Go to Line\n" +
                "  Ctrl+D            Duplicate Line\n" +
                "  Ctrl+Shift+K      Delete Line\n" +
                "  Alt+Up / Alt+Down Move Line Up / Down\n" +
                "  Ctrl+/            Toggle Line Comment\n\n" +
                "View\n" +
                "  Ctrl+=  / Ctrl+-  Zoom In / Zoom Out\n" +
                "  Ctrl+0            Reset Zoom\n" +
                "  F11               Full Screen\n\n" +
                "Run\n" +
                "  F5                Run Code\n\n" +
                "Tools\n" +
                "  Ctrl+,            Preferences";

            Messenger.MessageBox(shortcuts, "Keyboard Shortcuts", MessageBoxButtons.OK, MsgIcon.Info);
        }

        private void aboutTextifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string version = Application.ProductVersion;
            string message = "Textify Editor\nVersion " + version +
                "\n\nA fast, keyboard-driven code editor";
            Messenger.MessageBox(message, "About Textify Editor", MessageBoxButtons.OK, MsgIcon.Info);
        }

        #endregion
    }
}