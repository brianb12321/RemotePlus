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
    public partial class ConnectDialog : Form
    {
        public string Address { get; private set; }
        public ConnectDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Address = textBox1.Text;
            Close();
        }
    }
}
