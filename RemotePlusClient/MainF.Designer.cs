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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Speak");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Beep");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("File Transfer");
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.serverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.consoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extensionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getServerExtensionNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mi_open = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.emi_open = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.getExtensionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.connectToolStripMenuItem.Text = "Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // consoleToolStripMenuItem
            // 
            this.consoleToolStripMenuItem.Name = "consoleToolStripMenuItem";
            this.consoleToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.consoleToolStripMenuItem.Text = "Console";
            this.consoleToolStripMenuItem.Click += new System.EventHandler(this.consoleToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Enabled = false;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
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
            // treeView1
            // 
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView1.Location = new System.Drawing.Point(0, 24);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "nd_speak";
            treeNode1.Text = "Speak";
            treeNode2.Name = "nd_beep";
            treeNode2.Text = "Beep";
            treeNode3.Name = "nd_FileTransfer";
            treeNode3.Text = "File Transfer";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            this.treeView1.Size = new System.Drawing.Size(121, 402);
            this.treeView1.TabIndex = 1;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
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
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl1.Location = new System.Drawing.Point(0, 247);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(552, 155);
            this.tabControl1.TabIndex = 2;
            // 
            // tabControl2
            // 
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(552, 247);
            this.tabControl2.TabIndex = 3;
            // 
            // treeView2
            // 
            this.treeView2.ContextMenuStrip = this.contextMenuStrip2;
            this.treeView2.Dock = System.Windows.Forms.DockStyle.Right;
            this.treeView2.Location = new System.Drawing.Point(673, 24);
            this.treeView2.Name = "treeView2";
            this.treeView2.Size = new System.Drawing.Size(162, 402);
            this.treeView2.TabIndex = 4;
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
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(121, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(552, 402);
            this.panel1.TabIndex = 5;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(552, 247);
            this.panel2.TabIndex = 4;
            // 
            // getExtensionsToolStripMenuItem
            // 
            this.getExtensionsToolStripMenuItem.Name = "getExtensionsToolStripMenuItem";
            this.getExtensionsToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.getExtensionsToolStripMenuItem.Text = "Get Extensions";
            this.getExtensionsToolStripMenuItem.Click += new System.EventHandler(this.getExtensionsToolStripMenuItem_Click);
            // 
            // MainF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(835, 426);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.treeView2);
            this.Controls.Add(this.treeView1);
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
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem serverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.ToolStripMenuItem consoleToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mi_open;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extensionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem emi_open;
        private System.Windows.Forms.ToolStripMenuItem tabsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getServerExtensionNamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getExtensionsToolStripMenuItem;
    }
}