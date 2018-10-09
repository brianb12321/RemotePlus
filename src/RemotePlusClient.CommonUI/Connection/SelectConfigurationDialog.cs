using RemotePlusLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace RemotePlusClient.CommonUI.Connection
{
    public partial class SelectConfigurationDialog : Form
    {
        List<ConnectionConfiguration> _connections;
        public ConnectionConfiguration SelectedConnection { get; private set; }
        public SelectConfigurationDialog()
        {
            InitializeComponent();
        }

        private void SelectConfigurationDialog_Load(object sender, EventArgs e)
        {
            _connections = new List<ConnectionConfiguration>();
            searchConfigFiles();
            populateListView();
        }

        private void populateListView()
        {
            foreach(ConnectionConfiguration connection in _connections)
            {
                ListViewItem item = new ListViewItem(new string[] { connection.ConfigurationFileName, connection.ServerAddress });
                item.Tag = connection;
                item.Name = connection.ConfigurationFileName;
                listView1.Items.Add(item);
            }
        }

        void searchConfigFiles()
        {
            if(Directory.Exists(ConnectionConfiguration.CONFIGURATION_NAME))
            {
                foreach(string file in Directory.GetFiles(ConnectionConfiguration.CONFIGURATION_NAME, "*.ccf", SearchOption.AllDirectories))
                {
                    ConnectionConfiguration cc = new ConnectionConfiguration("");
                    cc = new RemotePlusLibrary.Configuration.StandordDataAccess.ConfigurationHelper().LoadConfig<ConnectionConfiguration>(file);
                    _connections.Add(cc);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (NewConnectionDialog ncd = new NewConnectionDialog(null))
            {
                if(ncd.ShowDialog() == DialogResult.OK)
                {
                    searchConfigFiles();
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                using (NewConnectionDialog ncd = new NewConnectionDialog((ConnectionConfiguration)listView1.SelectedItems[0].Tag))
                {
                    ncd.ShowDialog();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                SelectedConnection = (ConnectionConfiguration)listView1.SelectedItems[0].Tag;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}