using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.CommandDialogs
{
    public partial class BeepDialog : Form
    {
        public BeepDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainF.Remote.Beep((int)numericUpDown1.Value, (int)numericUpDown2.Value);
        }
    }
}
