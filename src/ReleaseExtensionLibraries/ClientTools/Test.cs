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
    public partial class Test : ThemedForm, IClientExtension
    {
        public Test()
        {
            InitializeComponent();
        }

        public ThemedForm ExtensionForm => this;

        public ClientExtensionDetails GeneralDetails => new ClientExtensionDetails("Test", "1.0.0.0")
        {
            Description = "A client extension that "
        };

        public bool StaticPositioned => true;

        public FormPosition Position => FormPosition.Top;

        private void button1_Click(object sender, EventArgs e)
        {
            var response = MainF.Remote.RunServerCommand("help", RemotePlusLibrary.Extension.CommandSystem.CommandExecutionMode.Client);
            textBox1.Text = response.GetLatest().Output.Metadata["helpText"];
        }

        private void Test_Load(object sender, EventArgs e)
        {

        }
    }
}
