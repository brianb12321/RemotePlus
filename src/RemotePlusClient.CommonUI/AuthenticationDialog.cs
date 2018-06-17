using RemotePlusLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusLibrary.Security.AccountSystem;

namespace RemotePlusClient.CommonUI
{
    public partial class AuthenticationDialog : Form
    {
        public UserCredentials UserInfo { get; private set; }
        public AuthenticationRequest Request{ get; private set; }
        public AuthenticationDialog(AuthenticationRequest req)
        {
            Request = req;
            InitializeComponent();
        }
        void CheckSeverity()
        {
            switch(Request.Severity)
            {
                case AutehnticationSeverity.Danger:
                    panel1.BackColor = Color.Red;
                    label4.Text = "The server is requesting authorization for a dangerous operation.";
                    break;
                case AutehnticationSeverity.Normal:
                    panel1.BackColor = Color.Blue;
                    label4.Text = "The server is requesting authorization.";
                    break;
                case AutehnticationSeverity.Risk:
                    panel1.BackColor = Color.Yellow;
                    label4.Text = "The server is requesting authorization for a risky operation.";
                    break;
            }
        }
        private void btn_login_Click(object sender, EventArgs e)
        {
            UserInfo = new UserCredentials(textBox1.Text, textBox2.Text);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void AuthenticationDialog_Load(object sender, EventArgs e)
        {
            CheckSeverity();
            richTextBox1.AppendText(Request.Reason);
        }
    }
}
