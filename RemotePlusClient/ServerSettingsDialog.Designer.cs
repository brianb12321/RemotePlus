namespace RemotePlusClient
{
    partial class ServerSettingsDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(617, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "These are the settings that will be applied to the server. Once you hit save, the" +
    " client will contact the server to make the changes.";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Location = new System.Drawing.Point(15, 49);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(614, 325);
            this.propertyGrid1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(553, 381);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ServerSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 416);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "ServerSettingsDialog";
            this.ShowIcon = false;
            this.Text = "Server Settings";
            this.Load += new System.EventHandler(this.ServerSettingsDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Button button1;
    }
}