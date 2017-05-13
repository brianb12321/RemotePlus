namespace RemotePlusClient
{
    partial class MainF
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
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Speak");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Beep");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("File Transfer");
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.serverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.consoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extensionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getServerExtensionNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getExtensionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mi_open = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.emi_open = new System.Windows.Forms.ToolStripMenuItem();
            this.tcRight = new System.Windows.Forms.TabControl();
            this.tcr_extensions = new System.Windows.Forms.TabPage();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.tcLeft = new System.Windows.Forms.TabControl();
            this.tcl_da = new System.Windows.Forms.TabPage();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.tcRight.SuspendLayout();
            this.tcr_extensions.SuspendLayout();
            this.tcLeft.SuspendLayout();
            this.tcl_da.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serverToolStripMenuItem,
            this.extensionsToolStripMenuItem,
            this.tabsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(835, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // serverToolStripMenuItem
            // 
            this.serverToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem,
            this.consoleToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.serverToolStripMenuItem.Name = "serverToolStripMenuItem";
            this.serverToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.serverToolStripMenuItem.Text = "Server";
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.connectToolStripMenuItem.Text = "Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // consoleToolStripMenuItem
            // 
            this.consoleToolStripMenuItem.Name = "consoleToolStripMenuItem";
            this.consoleToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.consoleToolStripMenuItem.Text = "Console";
            this.consoleToolStripMenuItem.Click += new System.EventHandler(this.consoleToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Enabled = false;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // extensionsToolStripMenuItem
            // 
            this.extensionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.getServerExtensionNamesToolStripMenuItem,
            this.getExtensionsToolStripMenuItem});
            this.extensionsToolStripMenuItem.Name = "extensionsToolStripMenuItem";
            this.extensionsToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
            this.extensionsToolStripMenuItem.Text = "Extensions";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // getServerExtensionNamesToolStripMenuItem
            // 
            this.getServerExtensionNamesToolStripMenuItem.Name = "getServerExtensionNamesToolStripMenuItem";
            this.getServerExtensionNamesToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.getServerExtensionNamesToolStripMenuItem.Text = "Get server extension names";
            this.getServerExtensionNamesToolStripMenuItem.Click += new System.EventHandler(this.getServerExtensionNamesToolStripMenuItem_Click);
            // 
            // getExtensionsToolStripMenuItem
            // 
            this.getExtensionsToolStripMenuItem.Name = "getExtensionsToolStripMenuItem";
            this.getExtensionsToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.getExtensionsToolStripMenuItem.Text = "Get Extensions";
            this.getExtensionsToolStripMenuItem.Click += new System.EventHandler(this.getExtensionsToolStripMenuItem_Click);
            // 
            // tabsToolStripMenuItem
            // 
            this.tabsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
            this.tabsToolStripMenuItem.Name = "tabsToolStripMenuItem";
            this.tabsToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.tabsToolStripMenuItem.Text = "Tabs";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi_open});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(104, 26);
            this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // mi_open
            // 
            this.mi_open.Name = "mi_open";
            this.mi_open.Size = new System.Drawing.Size(103, 22);
            this.mi_open.Text = "Open";
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.emi_open});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(104, 26);
            this.contextMenuStrip2.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip2_ItemClicked);
            // 
            // emi_open
            // 
            this.emi_open.Name = "emi_open";
            this.emi_open.Size = new System.Drawing.Size(103, 22);
            this.emi_open.Text = "Open";
            this.emi_open.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // tcRight
            // 
            this.tcRight.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tcRight.Controls.Add(this.tcr_extensions);
            this.tcRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.tcRight.Location = new System.Drawing.Point(652, 24);
            this.tcRight.Name = "tcRight";
            this.tcRight.SelectedIndex = 0;
            this.tcRight.Size = new System.Drawing.Size(183, 402);
            this.tcRight.TabIndex = 6;
            // 
            // tcr_extensions
            // 
            this.tcr_extensions.Controls.Add(this.treeView2);
            this.tcr_extensions.Location = new System.Drawing.Point(4, 4);
            this.tcr_extensions.Name = "tcr_extensions";
            this.tcr_extensions.Padding = new System.Windows.Forms.Padding(3);
            this.tcr_extensions.Size = new System.Drawing.Size(175, 376);
            this.tcr_extensions.TabIndex = 1;
            this.tcr_extensions.Text = "Extensions";
            this.tcr_extensions.UseVisualStyleBackColor = true;
            // 
            // treeView2
            // 
            this.treeView2.ContextMenuStrip = this.contextMenuStrip2;
            this.treeView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView2.Location = new System.Drawing.Point(3, 3);
            this.treeView2.Name = "treeView2";
            this.treeView2.Size = new System.Drawing.Size(169, 370);
            this.treeView2.TabIndex = 5;
            // 
            // tcLeft
            // 
            this.tcLeft.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tcLeft.Controls.Add(this.tcl_da);
            this.tcLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.tcLeft.Location = new System.Drawing.Point(0, 24);
            this.tcLeft.Name = "tcLeft";
            this.tcLeft.SelectedIndex = 0;
            this.tcLeft.Size = new System.Drawing.Size(174, 402);
            this.tcLeft.TabIndex = 7;
            // 
            // tcl_da
            // 
            this.tcl_da.Controls.Add(this.treeView1);
            this.tcl_da.Location = new System.Drawing.Point(4, 4);
            this.tcl_da.Name = "tcl_da";
            this.tcl_da.Padding = new System.Windows.Forms.Padding(3);
            this.tcl_da.Size = new System.Drawing.Size(166, 376);
            this.tcl_da.TabIndex = 1;
            this.tcl_da.Text = "Default actions";
            this.tcl_da.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(3, 3);
            this.treeView1.Name = "treeView1";
            treeNode4.Name = "nd_speak";
            treeNode4.Text = "Speak";
            treeNode5.Name = "nd_beep";
            treeNode5.Text = "Beep";
            treeNode6.Name = "nd_FileTransfer";
            treeNode6.Text = "File Transfer";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5,
            treeNode6});
            this.treeView1.Size = new System.Drawing.Size(160, 370);
            this.treeView1.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl1.Location = new System.Drawing.Point(174, 271);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(478, 155);
            this.tabControl1.TabIndex = 8;
            // 
            // tabControl2
            // 
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(174, 24);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(478, 247);
            this.tabControl2.TabIndex = 9;
            // 
            // MainF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(835, 426);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.tcLeft);
            this.Controls.Add(this.tcRight);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainF";
            this.Text = "MainF";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainF_FormClosing);
            this.Load += new System.EventHandler(this.MainF_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.tcRight.ResumeLayout(false);
            this.tcr_extensions.ResumeLayout(false);
            this.tcLeft.ResumeLayout(false);
            this.tcl_da.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem serverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem consoleToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mi_open;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extensionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem emi_open;
        private System.Windows.Forms.ToolStripMenuItem tabsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getServerExtensionNamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getExtensionsToolStripMenuItem;
        private System.Windows.Forms.TabControl tcRight;
        private System.Windows.Forms.TabPage tcr_extensions;
        private System.Windows.Forms.TreeView treeView2;
        private System.Windows.Forms.TabControl tcLeft;
        private System.Windows.Forms.TabPage tcl_da;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabControl tabControl2;
    }
}