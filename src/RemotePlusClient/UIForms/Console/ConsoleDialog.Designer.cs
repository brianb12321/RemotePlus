using RemotePlusLibrary.Extension.Gui;

namespace RemotePlusClient.UIForms.Consoles
{
    partial class ConsoleDialog
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
        protected override void InitializeTheme(Theme t)
        {
            richTextBox1.BackColor = t.BackgroundColor;
            richTextBox1.ForeColor = t.TextBoxForegroundColor;
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(284, 261);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // ConsoleDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.richTextBox1);
            this.Name = "ConsoleDialog";
            this.Text = "ConsoleDialog";
            this.Load += new System.EventHandler(this.ConsoleDialog_Load);
            this.TextChanged += new System.EventHandler(this.ConsoleDialog_TextChanged);
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.RichTextBox richTextBox1;
    }
}