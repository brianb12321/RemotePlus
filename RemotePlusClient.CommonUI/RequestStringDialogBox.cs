using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusLibrary;

namespace RemotePlusClient.CommonUI
{
    public partial class RequestStringDialogBox : Form, IDataRequest
    {
        private string data;
        private string message;
        public RequestStringDialogBox(string _message)
        {
            message = _message;
            InitializeComponent();
        }
        public RequestStringDialogBox()
        {
            InitializeComponent();
        }
        Form IDataRequest.RequestForm => this;

        RawDataRequest IDataRequest.RequestData(RequestBuilder builder)
        {
            this.message = builder.Message;
            if(this.ShowDialog() == DialogResult.OK)
            {
                return RawDataRequest.Success(data);
            }
            else
            {
                return RawDataRequest.Cancel();
            }
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
    }
}
