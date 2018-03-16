namespace RemotePlusClient
{
    partial class ScriptingEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptingEditor));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.run = new System.Windows.Forms.ToolStripButton();
            this.openFileToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.consoleModeToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.autocompleteMenu1 = new AutocompleteMenuNS.AutocompleteMenu();
            this.editor = new ICSharpCode.TextEditor.TextEditorControl();
            this.openScriptObjectToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.run,
            this.openFileToolStripButton,
            this.openScriptObjectToolStripButton,
            this.toolStripLabel1,
            this.consoleModeToolStripComboBox});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(443, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // run
            // 
            this.run.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.run.Image = global::RemotePlusClient.ScriptIcons.runScript;
            this.run.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.run.Name = "run";
            this.run.Size = new System.Drawing.Size(23, 22);
            this.run.Text = "Run Script";
            this.run.Click += new System.EventHandler(this.run_Click);
            // 
            // openFileToolStripButton
            // 
            this.openFileToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openFileToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openFileToolStripButton.Image")));
            this.openFileToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openFileToolStripButton.Name = "openFileToolStripButton";
            this.openFileToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openFileToolStripButton.Text = "Open File";
            this.openFileToolStripButton.Click += new System.EventHandler(this.openFileToolStripButton_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(87, 22);
            this.toolStripLabel1.Text = "Console Mode:";
            // 
            // consoleModeToolStripComboBox
            // 
            this.consoleModeToolStripComboBox.Items.AddRange(new object[] {
            "Normal",
            "Clear On Execute"});
            this.consoleModeToolStripComboBox.Name = "consoleModeToolStripComboBox";
            this.consoleModeToolStripComboBox.Size = new System.Drawing.Size(121, 25);
            // 
            // autocompleteMenu1
            // 
            this.autocompleteMenu1.Colors = ((AutocompleteMenuNS.Colors)(resources.GetObject("autocompleteMenu1.Colors")));
            this.autocompleteMenu1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.autocompleteMenu1.ImageList = null;
            this.autocompleteMenu1.Items = new string[0];
            this.autocompleteMenu1.TargetControlWrapper = null;
            // 
            // editor
            // 
            this.editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editor.IsReadOnly = false;
            this.editor.Location = new System.Drawing.Point(0, 25);
            this.editor.Name = "editor";
            this.editor.Size = new System.Drawing.Size(443, 296);
            this.editor.TabIndex = 2;
            // 
            // openScriptObjectToolStripButton
            // 
            this.openScriptObjectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openScriptObjectToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openScriptObjectToolStripButton.Image")));
            this.openScriptObjectToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openScriptObjectToolStripButton.Name = "openScriptObjectToolStripButton";
            this.openScriptObjectToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openScriptObjectToolStripButton.Text = "Open Script Object Viewer";
            this.openScriptObjectToolStripButton.Click += new System.EventHandler(this.openScriptObjectToolStripButton_Click);
            // 
            // ScriptingEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 321);
            this.Controls.Add(this.editor);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ScriptingEditor";
            this.Text = "ScriptingEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScriptingEditor_FormClosing);
            this.Load += new System.EventHandler(this.ScriptingEditor_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton run;
        private AutocompleteMenuNS.AutocompleteMenu autocompleteMenu1;
        private ICSharpCode.TextEditor.TextEditorControl editor;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox consoleModeToolStripComboBox;
        private System.Windows.Forms.ToolStripButton openFileToolStripButton;
        private System.Windows.Forms.ToolStripButton openScriptObjectToolStripButton;
    }
}