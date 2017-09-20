namespace RemotePlusClient.CommonUI.EmailGui
{
    partial class SendEmailForm
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
            this.Tbox_to = new System.Windows.Forms.TextBox();
            this.to = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TBox_subject = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.RTBox_message = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Tbox_to
            // 
            this.Tbox_to.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Tbox_to.Location = new System.Drawing.Point(51, 12);
            this.Tbox_to.Name = "Tbox_to";
            this.Tbox_to.Size = new System.Drawing.Size(602, 20);
            this.Tbox_to.TabIndex = 0;
            // 
            // to
            // 
            this.to.AutoSize = true;
            this.to.Location = new System.Drawing.Point(12, 15);
            this.to.Name = "to";
            this.to.Size = new System.Drawing.Size(20, 13);
            this.to.TabIndex = 1;
            this.to.Text = "To";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "subject";
            // 
            // TBox_subject
            // 
            this.TBox_subject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TBox_subject.Location = new System.Drawing.Point(51, 50);
            this.TBox_subject.Name = "TBox_subject";
            this.TBox_subject.Size = new System.Drawing.Size(602, 20);
            this.TBox_subject.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Body";
            // 
            // RTBox_message
            // 
            this.RTBox_message.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RTBox_message.Location = new System.Drawing.Point(12, 105);
            this.RTBox_message.Name = "RTBox_message";
            this.RTBox_message.Size = new System.Drawing.Size(641, 179);
            this.RTBox_message.TabIndex = 5;
            this.RTBox_message.Text = "";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(12, 295);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Send";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SendEmailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 330);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.RTBox_message);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TBox_subject);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.to);
            this.Controls.Add(this.Tbox_to);
            this.MaximizeBox = false;
            this.Name = "SendEmailForm";
            this.ShowIcon = false;
            this.Text = "Send a email";
            this.Load += new System.EventHandler(this.SendEmailForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Tbox_to;
        private System.Windows.Forms.Label to;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TBox_subject;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox RTBox_message;
        private System.Windows.Forms.Button button1;
    }
}