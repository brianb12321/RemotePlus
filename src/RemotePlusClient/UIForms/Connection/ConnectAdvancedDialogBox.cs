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

namespace RemotePlusClient.UIForms.Connection
{
    public partial class ConnectAdvancedDialogBox : Form
    {
        public ConnectAdvancedDialogBox()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ConnectAdvancedDialogBox_Load(object sender, EventArgs e)
        {

        }
    }
}