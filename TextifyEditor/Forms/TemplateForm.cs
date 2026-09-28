using DarkModeForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Textify_Editor.Forms
{
    public partial class TemplateForm : Form
    {
        private DarkModeCS dm = null;
        const int DWMWA_BORDER_COLOR = 34;

        public TemplateForm()
        {
            InitializeComponent();
            SetBorderColor(Color.DarkOrange);
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer, true);

            dm = new DarkModeCS(this)
            {
                ColorMode = DarkModeCS.DisplayMode.SystemDefault
            };
            DarkModeCS.ExcludeFromProcessing(lstTemplates);
            LoadTemplates();
        }

        public TemplateInfo SelectedTemplate
        {
            get { return lstTemplates.SelectedItem as TemplateInfo; }
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

        void SetBorderColor(Color color)
        {
            int colorRef = ColorTranslator.ToWin32(color);
            DwmSetWindowAttribute(this.Handle, DWMWA_BORDER_COLOR, ref colorRef, sizeof(int));
        }

        private void LoadTemplates()
        {
            lstTemplates.Items.Clear();

            lstTemplates.Items.Add(new TemplateInfo
            {
                Id = "cs_console",
                Name = "C# Console",
                Language = "C#",
                Extension = ".cs",
                Description = "Basic Program.cs starter"
            });

            lstTemplates.Items.Add(new TemplateInfo
            {
                Id = "python_console",
                Name = "Python Console",
                Language = "Python",
                Extension = ".py",
                Description = "Basic Python starter"
            });

            lstTemplates.Items.Add(new TemplateInfo
            {
                Id = "java_console",
                Name = "Java Console",
                Language = "Java",
                Extension = ".java",
                Description = "Basic Java starter"
            });

            lstTemplates.Items.Add(new TemplateInfo
            {
                Id = "cpp_console",
                Name = "C++ Console",
                Language = "C++",
                Extension = ".cpp",
                Description = "Simple main.cpp starter"
            });

            lstTemplates.Items.Add(new TemplateInfo
            {
                Id = "html_basic",
                Name = "HTML Basic",
                Language = "HTML",
                Extension = ".html",
                Description = "Starter HTML page"
            });

            if (lstTemplates.Items.Count > 0)
                lstTemplates.SelectedIndex = -1;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (SelectedTemplate == null)
            { 
                Messenger.MessageBox("Please select a template", "No Template Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void lstTemplates_DoubleClick(object sender, EventArgs e)
        {
            btnCreate.PerformClick();
        }
            
    }
}
