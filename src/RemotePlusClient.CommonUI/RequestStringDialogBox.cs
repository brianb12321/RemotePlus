using System;
using System.Windows.Forms;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;

namespace RemotePlusClient.CommonUI
{
    public sealed partial class RequestStringDialogBox : Form
    {
        public string Data => data;
        private string data;
        private string message;

        public RequestStringDialogBox(string _message)
        {
            message = _message;
            InitializeComponent();
        }

        private void RequestStringDialogBox_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = message;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            data = textBox1.Text;
        }

        private void RequestStringDialogBox_Shown(object sender, EventArgs e)
        {

        }
    }
}
