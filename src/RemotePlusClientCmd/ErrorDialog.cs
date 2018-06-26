using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.Faults;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClientCmd
{
    public partial class ErrorDialog : Form
    {
        public Exception Error { get; private set; }
        public ErrorDialog(Exception e)
        {
            Error = e;
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void ErrorDialog_Load(object sender, EventArgs e)
        {
            if (Error is FaultException<ServerFault>)
            {
                var fault = Error as FaultException<ServerFault>;
                textBox1.AppendText(fault.ToString() + Environment.NewLine);
                textBox1.AppendText(fault.Detail.ToString());
            }
            else
            {
                textBox1.Text = Error.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
