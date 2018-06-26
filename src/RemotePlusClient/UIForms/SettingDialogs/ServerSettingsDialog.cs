using RemotePlusLibrary;
using RemotePlusLibrary.Configuration.ServerSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.UIForms.SettingDialogs
{
    public partial class ServerSettingsDialog : Form
    {
        public ServerSettingsDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainF.Remote.UpdateServerSettings((ServerSettings)propertyGrid1.SelectedObject);
            MessageBox.Show("Settings saved.");
        }

        private void ServerSettingsDialog_Load(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = MainF.Remote.GetServerSettings();
        }
    }
}
