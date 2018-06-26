using RemotePlusClient.UIForms.Connection;
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

namespace RemotePlusClient.UIForms
{
    public partial class ServerExplorer : ThemedForm
    {
        public ServerExplorer()
        {
            InitializeComponent();
        }

        private void registerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {

            }
            else
            {
                ConnectAdvancedDialogBox cadb = new ConnectAdvancedDialogBox();
                if (cadb.ShowDialog() == DialogResult.OK)
                {
                    MainF.FoundServers.SelectServer((Guid)listBox1.SelectedItem);
                    MainF.FoundServers.Register(cadb.RegObject);
                }
            }
        }

        private void ServerExplorer_Load(object sender, EventArgs e)
        {
            foreach (Guid client in MainF.FoundServers.GetServers())
            {
                listBox1.Items.Add(client);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {

            }
            else
            {
                MainF.FoundServers.SelectServer((Guid)listBox1.SelectedItem);
                ClientApp.MainWindow.OpenConsole((Guid)listBox1.SelectedItem, MainF.FoundServers, ExtensionSystem.FormPosition.Top, true);
            }
        }
    }
}
