using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusServer
{
    public partial class ServerControls : Form
    {
        public ServerControls()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
#if INCOGNITO
            ServerStartup.RunInServerMode();
            button2.Enabled = true;
            button1.Enabled = false;
            Hide();
#else
            ServerStartup.RunInServerMode();
            button2.Enabled = true;
            button1.Enabled = false;
#endif
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ServerStartup.Close();
        }

        private void ServerControls_Load(object sender, EventArgs e)
        {
#if INCOGNITO
            this.ShowInTaskbar = false;
#endif
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                if (args[1] == "autoStart")
                {
                    button1_Click(this, EventArgs.Empty);
                }
            }
        }
    }
}