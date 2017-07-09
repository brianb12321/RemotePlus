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
    public partial class ConnectDialog : Form
    {
        ConnectAdvancedDialogBox cadb = new ConnectAdvancedDialogBox();
        public string Address { get; private set; }
        public RegistirationObject RegObject { get; private set; }
        public ConnectDialog()
        {
            InitializeComponent();
            RegObject = new RegistirationObject();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Address = textBox1.Text;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (cadb.ShowDialog() == DialogResult.OK)
            {
                RegObject = cadb.RegObject;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (ConnectionBuilderDialogBox cb = new ConnectionBuilderDialogBox())
            {
                if(cb.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = cb.NewUrl;
                }
            }
        }
    }
}