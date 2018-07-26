using System;
using RemotePlusLibrary.Extension.Gui;

namespace RemotePlusClient.UIForms
{
    partial class RemoteFileBrowser
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
            this.BackColor = t.BackgroundColor;
            this.ForeColor = t.TextForgroundColor;
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.progressWorker = new System.ComponentModel.BackgroundWorker();
            this.fileBrowser1 = new RemotePlusClient.CommonUI.Controls.FileBrowser(MainF.CurrentConnectionData.BaseAddress, MainF.CurrentConnectionData.Port);
            this.SuspendLayout();
            // 
            // fileBrowser1
            // 
            this.fileBrowser1.CountLabel = 0;
            this.fileBrowser1.CurrentPath = "Path";
            this.fileBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileBrowser1.Filter = RemotePlusClient.CommonUI.Controls.FileBrowserHelpers.FilterMode.Both;
            this.fileBrowser1.Location = new System.Drawing.Point(0, 0);
            this.fileBrowser1.Name = "fileBrowser1";
            this.fileBrowser1.Size = new System.Drawing.Size(651, 287);
            this.fileBrowser1.StatusMessage = "Idle";
            this.fileBrowser1.TabIndex = 0;
            this.fileBrowser1.FileSelected += new System.EventHandler<RemotePlusClient.CommonUI.Controls.FileBrowserHelpers.FileSelectedEventArgs>(this.fileBrowser1_FileSelected);
            this.fileBrowser1.NodeAboutToBeExpanded += new System.EventHandler<System.Windows.Forms.TreeViewCancelEventArgs>(this.fileBrowser1_NodeAboutToBeExpanded);
            this.fileBrowser1.TreeVewAfterSelect += new System.EventHandler<System.Windows.Forms.TreeViewEventArgs>(this.fileBrowser1_TreeVewAfterSelect);
            // 
            // RemoteFileBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 287);
            this.Controls.Add(this.fileBrowser1);
            this.Name = "RemoteFileBrowser";
            this.Text = "RemoteFileBrowser";
            this.Load += new System.EventHandler(this.RemoteFileBrowser_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.ComponentModel.BackgroundWorker progressWorker;
        private CommonUI.Controls.FileBrowser fileBrowser1;
    }
}