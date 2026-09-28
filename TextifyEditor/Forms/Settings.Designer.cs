namespace Textify_Editor.Forms
{
    partial class Settings
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

        private System.Windows.Forms.Panel pnlSidebar;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.Button btnNavAppearance;
        private System.Windows.Forms.Button btnNavEditor;
        private System.Windows.Forms.Button btnNavFiles;
        private System.Windows.Forms.Button btnNavAbout;
        private System.Windows.Forms.Panel pnlNavIndicator;

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblHeaderTitle;

        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel pnlAppearance;
        private System.Windows.Forms.Panel pnlEditor;
        private System.Windows.Forms.Panel pnlFiles;
        private System.Windows.Forms.Panel pnlAbout;

        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Button btnRestoreDefaults;
        private System.Windows.Forms.Button btnClose;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.pnlNavIndicator = new System.Windows.Forms.Panel();
            this.btnNavAbout = new System.Windows.Forms.Button();
            this.btnNavFiles = new System.Windows.Forms.Button();
            this.btnNavEditor = new System.Windows.Forms.Button();
            this.btnNavAppearance = new System.Windows.Forms.Button();
            this.lblBrand = new System.Windows.Forms.Label();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblHeaderTitle = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlAbout = new System.Windows.Forms.Panel();
            this.pnlFiles = new System.Windows.Forms.Panel();
            this.pnlEditor = new System.Windows.Forms.Panel();
            this.pnlAppearance = new System.Windows.Forms.Panel();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRestoreDefaults = new System.Windows.Forms.Button();
            this.pnlSidebar.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSidebar.Controls.Add(this.pnlNavIndicator);
            this.pnlSidebar.Controls.Add(this.btnNavAbout);
            this.pnlSidebar.Controls.Add(this.btnNavFiles);
            this.pnlSidebar.Controls.Add(this.btnNavEditor);
            this.pnlSidebar.Controls.Add(this.btnNavAppearance);
            this.pnlSidebar.Controls.Add(this.lblBrand);
            this.pnlSidebar.Location = new System.Drawing.Point(0, 0);
            this.pnlSidebar.Margin = new System.Windows.Forms.Padding(4);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(307, 763);
            this.pnlSidebar.TabIndex = 0;
            // 
            // pnlNavIndicator
            // 
            this.pnlNavIndicator.Location = new System.Drawing.Point(0, 86);
            this.pnlNavIndicator.Margin = new System.Windows.Forms.Padding(4);
            this.pnlNavIndicator.Name = "pnlNavIndicator";
            this.pnlNavIndicator.Size = new System.Drawing.Size(5, 57);
            this.pnlNavIndicator.TabIndex = 5;
            // 
            // btnNavAbout
            // 
            this.btnNavAbout.FlatAppearance.BorderSize = 0;
            this.btnNavAbout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavAbout.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnNavAbout.Location = new System.Drawing.Point(0, 256);
            this.btnNavAbout.Margin = new System.Windows.Forms.Padding(4);
            this.btnNavAbout.Name = "btnNavAbout";
            this.btnNavAbout.Padding = new System.Windows.Forms.Padding(32, 0, 0, 0);
            this.btnNavAbout.Size = new System.Drawing.Size(307, 57);
            this.btnNavAbout.TabIndex = 4;
            this.btnNavAbout.Text = "About";
            this.btnNavAbout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavAbout.UseVisualStyleBackColor = true;
            this.btnNavAbout.Click += new System.EventHandler(this.NavButton_Click);
            // 
            // btnNavFiles
            // 
            this.btnNavFiles.FlatAppearance.BorderSize = 0;
            this.btnNavFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavFiles.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnNavFiles.Location = new System.Drawing.Point(0, 199);
            this.btnNavFiles.Margin = new System.Windows.Forms.Padding(4);
            this.btnNavFiles.Name = "btnNavFiles";
            this.btnNavFiles.Padding = new System.Windows.Forms.Padding(32, 0, 0, 0);
            this.btnNavFiles.Size = new System.Drawing.Size(307, 57);
            this.btnNavFiles.TabIndex = 3;
            this.btnNavFiles.Text = "Files && Integrations";
            this.btnNavFiles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavFiles.UseVisualStyleBackColor = true;
            this.btnNavFiles.Click += new System.EventHandler(this.NavButton_Click);
            // 
            // btnNavEditor
            // 
            this.btnNavEditor.FlatAppearance.BorderSize = 0;
            this.btnNavEditor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavEditor.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnNavEditor.Location = new System.Drawing.Point(0, 143);
            this.btnNavEditor.Margin = new System.Windows.Forms.Padding(4);
            this.btnNavEditor.Name = "btnNavEditor";
            this.btnNavEditor.Padding = new System.Windows.Forms.Padding(32, 0, 0, 0);
            this.btnNavEditor.Size = new System.Drawing.Size(307, 57);
            this.btnNavEditor.TabIndex = 2;
            this.btnNavEditor.Text = "Editor";
            this.btnNavEditor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavEditor.UseVisualStyleBackColor = true;
            this.btnNavEditor.Click += new System.EventHandler(this.NavButton_Click);
            // 
            // btnNavAppearance
            // 
            this.btnNavAppearance.FlatAppearance.BorderSize = 0;
            this.btnNavAppearance.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavAppearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnNavAppearance.Location = new System.Drawing.Point(0, 86);
            this.btnNavAppearance.Margin = new System.Windows.Forms.Padding(4);
            this.btnNavAppearance.Name = "btnNavAppearance";
            this.btnNavAppearance.Padding = new System.Windows.Forms.Padding(32, 0, 0, 0);
            this.btnNavAppearance.Size = new System.Drawing.Size(307, 57);
            this.btnNavAppearance.TabIndex = 1;
            this.btnNavAppearance.Text = "Appearance";
            this.btnNavAppearance.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavAppearance.UseVisualStyleBackColor = true;
            this.btnNavAppearance.Click += new System.EventHandler(this.NavButton_Click);
            // 
            // lblBrand
            // 
            this.lblBrand.AutoSize = true;
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI Semibold", 12.5F);
            this.lblBrand.Location = new System.Drawing.Point(32, 30);
            this.lblBrand.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(140, 30);
            this.lblBrand.TabIndex = 0;
            this.lblBrand.Text = "Textify Editor";
            // 
            // pnlHeader
            // 
            this.pnlHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlHeader.Controls.Add(this.lblHeaderTitle);
            this.pnlHeader.Location = new System.Drawing.Point(307, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(4);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(947, 79);
            this.pnlHeader.TabIndex = 1;
            // 
            // lblHeaderTitle
            // 
            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold);
            this.lblHeaderTitle.Location = new System.Drawing.Point(-7, 23);
            this.lblHeaderTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHeaderTitle.Name = "lblHeaderTitle";
            this.lblHeaderTitle.Size = new System.Drawing.Size(170, 37);
            this.lblHeaderTitle.TabIndex = 0;
            this.lblHeaderTitle.Text = "Appearance";
            // 
            // pnlContent
            // 
            this.pnlContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlContent.Controls.Add(this.pnlAbout);
            this.pnlContent.Controls.Add(this.pnlFiles);
            this.pnlContent.Controls.Add(this.pnlEditor);
            this.pnlContent.Controls.Add(this.pnlAppearance);
            this.pnlContent.Location = new System.Drawing.Point(307, 79);
            this.pnlContent.Margin = new System.Windows.Forms.Padding(4);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(947, 606);
            this.pnlContent.TabIndex = 2;
            // 
            // pnlAbout
            // 
            this.pnlAbout.AutoScroll = true;
            this.pnlAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAbout.Location = new System.Drawing.Point(0, 0);
            this.pnlAbout.Margin = new System.Windows.Forms.Padding(4);
            this.pnlAbout.Name = "pnlAbout";
            this.pnlAbout.Padding = new System.Windows.Forms.Padding(37, 25, 37, 25);
            this.pnlAbout.Size = new System.Drawing.Size(947, 606);
            this.pnlAbout.TabIndex = 3;
            this.pnlAbout.Visible = false;
            // 
            // pnlFiles
            // 
            this.pnlFiles.AutoScroll = true;
            this.pnlFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFiles.Location = new System.Drawing.Point(0, 0);
            this.pnlFiles.Margin = new System.Windows.Forms.Padding(4);
            this.pnlFiles.Name = "pnlFiles";
            this.pnlFiles.Padding = new System.Windows.Forms.Padding(37, 25, 37, 25);
            this.pnlFiles.Size = new System.Drawing.Size(947, 606);
            this.pnlFiles.TabIndex = 2;
            this.pnlFiles.Visible = false;
            // 
            // pnlEditor
            // 
            this.pnlEditor.AutoScroll = true;
            this.pnlEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEditor.Location = new System.Drawing.Point(0, 0);
            this.pnlEditor.Margin = new System.Windows.Forms.Padding(4);
            this.pnlEditor.Name = "pnlEditor";
            this.pnlEditor.Padding = new System.Windows.Forms.Padding(37, 25, 37, 25);
            this.pnlEditor.Size = new System.Drawing.Size(947, 606);
            this.pnlEditor.TabIndex = 1;
            this.pnlEditor.Visible = false;
            // 
            // pnlAppearance
            // 
            this.pnlAppearance.AutoScroll = true;
            this.pnlAppearance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAppearance.Location = new System.Drawing.Point(0, 0);
            this.pnlAppearance.Margin = new System.Windows.Forms.Padding(4);
            this.pnlAppearance.Name = "pnlAppearance";
            this.pnlAppearance.Padding = new System.Windows.Forms.Padding(37, 25, 37, 25);
            this.pnlAppearance.Size = new System.Drawing.Size(947, 606);
            this.pnlAppearance.TabIndex = 0;
            // 
            // pnlFooter
            // 
            this.pnlFooter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFooter.Controls.Add(this.btnClose);
            this.pnlFooter.Controls.Add(this.btnRestoreDefaults);
            this.pnlFooter.Location = new System.Drawing.Point(307, 684);
            this.pnlFooter.Margin = new System.Windows.Forms.Padding(4);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(947, 79);
            this.pnlFooter.TabIndex = 3;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(768, 20);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(147, 39);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnRestoreDefaults
            // 
            this.btnRestoreDefaults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestoreDefaults.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnRestoreDefaults.Location = new System.Drawing.Point(32, 20);
            this.btnRestoreDefaults.Margin = new System.Windows.Forms.Padding(4);
            this.btnRestoreDefaults.Name = "btnRestoreDefaults";
            this.btnRestoreDefaults.Size = new System.Drawing.Size(187, 39);
            this.btnRestoreDefaults.TabIndex = 0;
            this.btnRestoreDefaults.Text = "Restore Defaults";
            this.btnRestoreDefaults.UseVisualStyleBackColor = true;
            this.btnRestoreDefaults.Click += new System.EventHandler(this.btnRestoreDefaults_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1253, 763);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlSidebar);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.pnlSidebar.ResumeLayout(false);
            this.pnlSidebar.PerformLayout();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlContent.ResumeLayout(false);
            this.pnlFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}