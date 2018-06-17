using RemotePlusLibrary;
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

namespace RemotePlusClient
{
    public partial class ServerExplorer : ThemedForm
    {
        public ServerExplorer()
        {
            InitializeComponent();
        }

        private void registerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectAdvancedDialogBox cadb = new ConnectAdvancedDialogBox();
            if (cadb.ShowDialog() == DialogResult.OK)
            {
                ((ServiceClient)listBox1.SelectedItem).Register(cadb.RegObject);
            }
        }

        private void ServerExplorer_Load(object sender, EventArgs e)
        {
            foreach (ServiceClient client in MainF.FoundServers)
            {
                listBox1.Items.Add(client);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ClientApp.MainWindow.OpenConsole((ServiceClient)listBox1.SelectedItem, ExtensionSystem.FormPosition.Top, true);
        }
    }
}
