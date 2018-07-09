using RemotePlusLibrary;
using System;
using System.Windows.Forms;

namespace RemotePlusClient.CommonUI.Connection
{
    public partial class NewConnectionDialog : Form
    {
        ConnectionConfiguration config = null;
        public NewConnectionDialog(ConnectionConfiguration c)
        {
            config = c;
            InitializeComponent();
        }

        private void NewConnectionDialog_Load(object sender, EventArgs e)
        {
            if(config == null)
            {
                config = new ConnectionConfiguration("");
            }
            else
            {
                config = GlobalServices.DataAccess.LoadConfig<ConnectionConfiguration>(config.ConfigurationFileName);
            }
            populateFields();
        }

        private void populateFields()
        {
            textBox1.Text = config.ServerAddress;
            propertyGrid1.SelectedObject = config.RegisterationDetails;
            textBox2.Text = config.ConfigurationFileName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            config.ConfigurationFileName = textBox2.Text + ".ccf";
            config.ServerAddress = textBox1.Text;
            config.RegisterationDetails = (RegisterationObject)propertyGrid1.SelectedObject;
            GlobalServices.DataAccess.SaveConfig(config, config.ConfigurationFileName);
            MessageBox.Show("Connection saved.");
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
