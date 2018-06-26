using RemotePlusClient.ExtensionSystem;
using RemotePlusLibrary.Extension.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.UIForms
{
    public partial class ExtensionView : ThemedForm
    {
        public ExtensionView()
        {
            InitializeComponent();
        }

        private void mi_open_Click(object sender, EventArgs e)
        {
            var collection = MainF.DefaultCollection.GetAllExtensions();
            var extension = collection[treeView1.SelectedNode.Name];
            switch (extension.Position)
            {
                case FormPosition.Top:
                    ClientApp.MainWindow.AddTabToMainTabControl(extension.ExtensionName, extension.ExtensionForm);
                    break;
                case FormPosition.Bottum:
                    ClientApp.MainWindow.AddTabToConsoleTabControl(extension.ExtensionName, extension.ExtensionForm);
                    break;
            }
        }

        private void ExtensionView_Load(object sender, EventArgs e)
        {

        }
    }
}
