using System;
using System.Windows.Forms;
using RemotePlusLibrary.RequestSystem;

namespace RemotePlusClient.CommonUI
{
    public sealed partial class RequestStringDialogBox : Form, IDataRequest
    {
        private string data;
        private string message;

        bool IDataRequest.ShowProperties => false;

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
            RequestStringDialogBox rd = new RequestStringDialogBox(builder.Message);
            if(rd.ShowDialog() == DialogResult.OK)
            {
                return RawDataRequest.Success(rd.data);
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
            throw new NotImplementedException();
        }

        private void RequestStringDialogBox_Shown(object sender, EventArgs e)
        {

        }

        public void Update(string message)
        {
            throw new NotImplementedException();
        }
    }
}
