using RemotePlusClient.CommonUI;
using RemotePlusLibrary.Extension.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient
{
    public partial class ScriptingEditor : ThemedForm
    {
        public const string NAME = "ScriptingEditor";
        public ScriptingEditor()
        {
            InitializeComponent();
        }

        private void ScriptingEditor_Load(object sender, EventArgs e)
        {
            
            //editor.ShowInvalidLines = true;
            if (MainF.ServerConsoleObj == null)
            {
                ClientApp.MainWindow.OpenConsole(ExtensionSystem.FormPosition.Bottum, true);
            }
            autocompleteMenu1.AutoPopup = true;
            autocompleteMenu1.ImageList = new ImageList();
            autocompleteMenu1.ImageList.Images.Add(ScriptIcons.function_kCl_icon);
            autocompleteMenu1.ImageList.Images.Add(ScriptIcons.table_JoW_icon);
            var items = MainF.Remote.GetScriptGlobals();
            foreach (var item in items)
            {
                autocompleteMenu1.AddItem(new ScriptAutoCompleteItem(item));
            }
        }

        private void ScriptingEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClientApp.MainWindow.CloseBottumConsole();
        }

        private async void run_Click(object sender, EventArgs e)
        {
            run.Enabled = false;
            switch (consoleModeToolStripComboBox.SelectedIndex)
            {
                case 1:
                    MainF.ServerConsoleObj.ClearConsole();
                    break;
            }
            await Task.Run(() => MainF.Remote.ExecuteScript(editor.Text)).ContinueWith((b) => run.Enabled = true);
            
        }

        private void openFileToolStripButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Server Script File (*.lua)|*.lua";
                ofd.Title = "Open a server-side script file.";
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    editor.Text = File.ReadAllText(ofd.FileName);
                }
            }
        }

        private void openScriptObjectToolStripButton_Click(object sender, EventArgs e)
        {
            ClientApp.MainWindow.AddTabToSideControl("Script Object Viewer", new ScriptObjectViewer(MainF.Remote.GetScriptGlobals()));
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            FindRemoteFileDialog dialog = new FindRemoteFileDialog(FilterMode.File, MainF.Remote, MainF.BaseAddress, MainF.Port);
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                editor.Text = MainF.Remote.ReadFileAsString(dialog.FilePath);
            }
        }
    }
}
