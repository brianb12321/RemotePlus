using RemotePlusClient;
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

namespace ClientTools
{
    public partial class BeepDialog : ThemedForm, IClientExtension
    {
        public ThemedForm ExtensionForm => this;

        public ClientExtensionDetails GeneralDetails => new ClientExtensionDetails("Beep", "1.0.0.0");

        public BeepDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainF.Remote.Beep((int)numericUpDown1.Value, (int)numericUpDown2.Value);
        }

        private void BeepDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
