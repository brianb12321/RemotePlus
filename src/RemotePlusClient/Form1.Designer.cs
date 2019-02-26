namespace RemotePlusClient
{
    partial class Form1
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
            this.tc_left = new System.Windows.Forms.TabControl();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.tc_top = new System.Windows.Forms.TabControl();
            this.tc_bottum = new System.Windows.Forms.TabControl();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // tc_left
            // 
            this.tc_left.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tc_left.Dock = System.Windows.Forms.DockStyle.Left;
            this.tc_left.Location = new System.Drawing.Point(0, 0);
            this.tc_left.Multiline = true;
            this.tc_left.Name = "tc_left";
            this.tc_left.SelectedIndex = 0;
            this.tc_left.Size = new System.Drawing.Size(200, 450);
            this.tc_left.TabIndex = 0;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(200, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.tc_top);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.tc_bottum);
            this.splitContainer.Size = new System.Drawing.Size(600, 450);
            this.splitContainer.SplitterDistance = 200;
            this.splitContainer.TabIndex = 1;
            // 
            // tc_top
            // 
            this.tc_top.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tc_top.Location = new System.Drawing.Point(0, 0);
            this.tc_top.Name = "tc_top";
            this.tc_top.SelectedIndex = 0;
            this.tc_top.Size = new System.Drawing.Size(600, 200);
            this.tc_top.TabIndex = 0;
            // 
            // tc_bottum
            // 
            this.tc_bottum.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tc_bottum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tc_bottum.Location = new System.Drawing.Point(0, 0);
            this.tc_bottum.Name = "tc_bottum";
            this.tc_bottum.SelectedIndex = 0;
            this.tc_bottum.Size = new System.Drawing.Size(600, 246);
            this.tc_bottum.TabIndex = 0;
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem3});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2});
            this.menuItem1.Text = "File";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 0;
            this.menuItem2.Text = "Exit";
            this.menuItem2.Click += new System.EventHandler(this.globalOpen);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem4,
            this.menuItem5});
            this.menuItem3.Text = "Server";
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 0;
            this.menuItem4.Text = "Connect";
            this.menuItem4.Click += new System.EventHandler(this.globalOpen);
            // 
            // menuItem5
            // 
            this.menuItem5.Enabled = false;
            this.menuItem5.Index = 1;
            this.menuItem5.Text = "Open Console";
            this.menuItem5.Click += new System.EventHandler(this.globalOpen);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.tc_left);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tc_left;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TabControl tc_top;
        private System.Windows.Forms.TabControl tc_bottum;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem menuItem5;
    }
}

