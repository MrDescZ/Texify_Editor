namespace Textify_Editor.Forms
{
    partial class TemplateForm
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
            this.btnCreate = new System.Windows.Forms.Button();
            this.lstTemplates = new CustomListBox();
            this.SuspendLayout();
            // 
            // btnCreate
            // 
            this.btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCreate.Location = new System.Drawing.Point(611, 415);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(102, 23);
            this.btnCreate.TabIndex = 1;
            this.btnCreate.Text = "Create ";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // lstTemplates
            // 
            this.lstTemplates.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.lstTemplates.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstTemplates.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lstTemplates.DescTextColor = System.Drawing.Color.Gray;
            this.lstTemplates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTemplates.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstTemplates.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstTemplates.FormattingEnabled = true;
            this.lstTemplates.IntegralHeight = false;
            this.lstTemplates.ItemHeight = 56;
            this.lstTemplates.ItemPadding = 12;
            this.lstTemplates.Location = new System.Drawing.Point(0, 0);
            this.lstTemplates.Name = "lstTemplates";
            this.lstTemplates.NormalBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lstTemplates.NormalTextColor = System.Drawing.Color.Gainsboro;
            this.lstTemplates.SelectedBackColor = System.Drawing.Color.DarkGray;
            this.lstTemplates.SelectedDescColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.lstTemplates.SelectedTextColor = System.Drawing.Color.White;
            this.lstTemplates.SeparatorColor = System.Drawing.Color.Transparent;
            this.lstTemplates.Size = new System.Drawing.Size(725, 450);
            this.lstTemplates.TabIndex = 2;
            this.lstTemplates.DoubleClick += new System.EventHandler(this.lstTemplates_DoubleClick);
            // 
            // TemplateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 450);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.lstTemplates);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TemplateForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Choose your new template";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCreate;
        private CustomListBox lstTemplates;
    }
}