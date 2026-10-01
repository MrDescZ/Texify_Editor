namespace Textify_Editor.Forms
{
    partial class StartEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartEditor));
            this.continueWithoutFile = new System.Windows.Forms.Button();
            this.pinMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.recentListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.chooseTemplate = new System.Windows.Forms.Button();
            this.createNewFile = new System.Windows.Forms.Button();
            this.openFile = new System.Windows.Forms.Button();
            this.genxLabel2 = new System.Windows.Forms.Label();
            this.searchHost = new Textify_Editor.Forms.RoundedPanel();
            this.searchClear = new System.Windows.Forms.Label();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.searchIcon = new System.Windows.Forms.Label();
            this.RecentList = new RecentFilesListBox();
            this.recentListContextMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.searchHost.SuspendLayout();
            this.SuspendLayout();
            // 
            // continueWithoutFile
            // 
            this.continueWithoutFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.continueWithoutFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.continueWithoutFile.ForeColor = System.Drawing.Color.White;
            this.continueWithoutFile.Location = new System.Drawing.Point(988, 12);
            this.continueWithoutFile.Margin = new System.Windows.Forms.Padding(4);
            this.continueWithoutFile.Name = "continueWithoutFile";
            this.continueWithoutFile.Size = new System.Drawing.Size(165, 32);
            this.continueWithoutFile.TabIndex = 0;
            this.continueWithoutFile.Text = "Continue without code";
            this.continueWithoutFile.UseVisualStyleBackColor = false;
            this.continueWithoutFile.Click += new System.EventHandler(this.continueWithoutFile_Click);
            // 
            // pinMenuItem
            // 
            this.pinMenuItem.Name = "pinMenuItem";
            this.pinMenuItem.Size = new System.Drawing.Size(132, 24);
            this.pinMenuItem.Text = "Pin";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(132, 24);
            this.toolStripMenuItem1.Text = "Remove";
            // 
            // recentListContextMenu
            // 
            this.recentListContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.recentListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pinMenuItem,
            this.toolStripMenuItem1});
            this.recentListContextMenu.Name = "recentListContextMenu";
            this.recentListContextMenu.Size = new System.Drawing.Size(133, 52);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.continueWithoutFile);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 742);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1171, 57);
            this.panel1.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(33, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 18);
            this.label1.TabIndex = 24;
            this.label1.Text = "Last synced";
            // 
            // chooseTemplate
            // 
            this.chooseTemplate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chooseTemplate.Location = new System.Drawing.Point(988, 116);
            this.chooseTemplate.Margin = new System.Windows.Forms.Padding(4);
            this.chooseTemplate.Name = "chooseTemplate";
            this.chooseTemplate.Size = new System.Drawing.Size(157, 44);
            this.chooseTemplate.TabIndex = 15;
            this.chooseTemplate.Text = "Choose Template";
            this.chooseTemplate.UseVisualStyleBackColor = true;
            this.chooseTemplate.Click += new System.EventHandler(this.chooseTemplate_Click);
            // 
            // createNewFile
            // 
            this.createNewFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.createNewFile.Location = new System.Drawing.Point(988, 178);
            this.createNewFile.Margin = new System.Windows.Forms.Padding(4);
            this.createNewFile.Name = "createNewFile";
            this.createNewFile.Size = new System.Drawing.Size(157, 44);
            this.createNewFile.TabIndex = 16;
            this.createNewFile.Text = "Create a new file";
            this.createNewFile.UseVisualStyleBackColor = true;
            this.createNewFile.Click += new System.EventHandler(this.createNewFile_Click);
            // 
            // openFile
            // 
            this.openFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.openFile.Location = new System.Drawing.Point(988, 242);
            this.openFile.Margin = new System.Windows.Forms.Padding(4);
            this.openFile.Name = "openFile";
            this.openFile.Size = new System.Drawing.Size(157, 44);
            this.openFile.TabIndex = 17;
            this.openFile.Text = "Open a file";
            this.openFile.UseVisualStyleBackColor = false;
            this.openFile.Click += new System.EventHandler(this.openFile_Click);
            // 
            // genxLabel2
            // 
            this.genxLabel2.AutoSize = true;
            this.genxLabel2.Font = new System.Drawing.Font("Microsoft YaHei UI", 25.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.genxLabel2.Location = new System.Drawing.Point(37, 9);
            this.genxLabel2.Name = "genxLabel2";
            this.genxLabel2.Size = new System.Drawing.Size(322, 56);
            this.genxLabel2.TabIndex = 22;
            this.genxLabel2.Text = "Textify Editor ";
            // 
            // searchHost
            // 
            this.searchHost.BackColor = System.Drawing.Color.Transparent;
            this.searchHost.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(74)))));
            this.searchHost.Controls.Add(this.searchClear);
            this.searchHost.Controls.Add(this.searchBox);
            this.searchHost.Controls.Add(this.searchIcon);
            this.searchHost.CornerRadius = 10;
            this.searchHost.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.searchHost.Location = new System.Drawing.Point(32, 121);
            this.searchHost.Margin = new System.Windows.Forms.Padding(4);
            this.searchHost.Name = "searchHost";
            this.searchHost.Size = new System.Drawing.Size(915, 34);
            this.searchHost.TabIndex = 18;
            // 
            // searchClear
            // 
            this.searchClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.searchClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.searchClear.Font = new System.Drawing.Font("Segoe MDL2 Assets", 9F);
            this.searchClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.searchClear.Location = new System.Drawing.Point(877, 6);
            this.searchClear.Name = "searchClear";
            this.searchClear.Size = new System.Drawing.Size(27, 22);
            this.searchClear.TabIndex = 2;
            this.searchClear.Text = "";
            this.searchClear.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.searchClear.Visible = false;
            // 
            // searchBox
            // 
            this.searchBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchBox.ForeColor = System.Drawing.Color.White;
            this.searchBox.Location = new System.Drawing.Point(40, 9);
            this.searchBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(829, 15);
            this.searchBox.TabIndex = 0;
            this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
            this.searchBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.searchBox_KeyDown);
            // 
            // searchIcon
            // 
            this.searchIcon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.searchIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 11F);
            this.searchIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.searchIcon.Location = new System.Drawing.Point(11, 5);
            this.searchIcon.Name = "searchIcon";
            this.searchIcon.Size = new System.Drawing.Size(27, 25);
            this.searchIcon.TabIndex = 1;
            this.searchIcon.Text = "";
            this.searchIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RecentList
            // 
            this.RecentList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.RecentList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RecentList.ContextMenuStrip = this.recentListContextMenu;
            this.RecentList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.RecentList.FormattingEnabled = true;
            this.RecentList.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.RecentList.IntegralHeight = false;
            this.RecentList.ItemHeight = 56;
            this.RecentList.ItemPadding = 12;
            this.RecentList.Location = new System.Drawing.Point(32, 167);
            this.RecentList.Margin = new System.Windows.Forms.Padding(4);
            this.RecentList.Name = "RecentList";
            this.RecentList.NormalBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.RecentList.NormalPathColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.RecentList.NormalTitleColor = System.Drawing.Color.Gainsboro;
            this.RecentList.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.RecentList.SelectedPathColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.RecentList.SelectedTitleColor = System.Drawing.Color.White;
            this.RecentList.SeparatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.RecentList.Size = new System.Drawing.Size(915, 572);
            this.RecentList.TabIndex = 13;
            this.RecentList.DoubleClick += new System.EventHandler(this.RecentList_DoubleClick);
            this.RecentList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RecentList_KeyDown);
            // 
            // StartEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1171, 799);
            this.Controls.Add(this.genxLabel2);
            this.Controls.Add(this.searchHost);
            this.Controls.Add(this.openFile);
            this.Controls.Add(this.createNewFile);
            this.Controls.Add(this.chooseTemplate);
            this.Controls.Add(this.RecentList);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StartEditor";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.StartEditor_Load);
            this.recentListContextMenu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.searchHost.ResumeLayout(false);
            this.searchHost.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button continueWithoutFile;
        private System.Windows.Forms.ToolStripMenuItem pinMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip recentListContextMenu;
        private RecentFilesListBox RecentList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button chooseTemplate;
        private System.Windows.Forms.Button createNewFile;
        private System.Windows.Forms.Button openFile;
        private RoundedPanel searchHost;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Label searchIcon;
        private System.Windows.Forms.Label searchClear;
        private System.Windows.Forms.Label genxLabel2;
        private System.Windows.Forms.Label label1;
    }
}