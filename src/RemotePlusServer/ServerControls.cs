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
        bool startFlag;
        public ServerControls(bool flag)
        {
            startFlag = flag;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
#if COGNITO
            ServerManager.RunInServerMode();
            button2.Enabled = true;
            button1.Enabled = false;
            Hide();
#else
            ServerManager.RunInServerMode();
            button2.Enabled = true;
            button1.Enabled = false;
#endif
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ServerManager.Close();
        }

        private void ServerControls_Load(object sender, EventArgs e)
        {
            if (startFlag)
            {
                button1_Click(this, new EventArgs());
            }
#if COGNITO
            this.ShowInTaskbar = false;
#endif

        }
    }
}
