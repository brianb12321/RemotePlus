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
    public partial class ConnectionBuilderDialogBox : Form
    {
        public string NewUrl { get; private set; }
        public ConnectionBuilderDialogBox()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            string url = $"{comboBox1.SelectedItem}://{textBox1.Text}:{numericUpDown1.Value}";
            NewUrl = url;
            Close();
        }
    }
}
