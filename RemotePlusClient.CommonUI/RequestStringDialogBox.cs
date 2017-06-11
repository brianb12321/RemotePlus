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

        bool IDataRequest.ShowProperties => true;

        string IDataRequest.FriendlyName => "Request String";

        string IDataRequest.Description => "Requests the user for a string.";

        public RequestStringDialogBox(string _message)
        {
            message = _message;
            InitializeComponent();
        }
        public RequestStringDialogBox()
        {
            InitializeComponent();
        }

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

        void IDataRequest.UpdateProperties()
        {
            new RequestStringDialogBox("This is a test.").ShowDialog();
        }
    }
}
