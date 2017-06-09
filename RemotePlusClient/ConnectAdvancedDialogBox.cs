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
    public partial class ConnectAdvancedDialogBox : Form
    {
        public RegistirationObject RegObject { get; private set; }
        public ConnectAdvancedDialogBox()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RegObject = ro;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}