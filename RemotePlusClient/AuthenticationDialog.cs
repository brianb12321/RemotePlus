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

namespace RemotePlusClient
{
    public partial class AuthenticationDialog : Form
    {
        public UserCredentials UserInfo { get; private set; }
        public string Reason { get; private set; }
        public AuthenticationDialog(string reason)
        {
            Reason = reason;
            InitializeComponent();
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
            richTextBox1.AppendText(Reason);
        }
    }
}
