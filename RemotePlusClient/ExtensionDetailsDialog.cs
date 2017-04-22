using RemotePlusLibrary.Extension;
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
    public partial class ExtensionDetailsDialog : Form
    {
        public ExtensionDetailsDialog()
        {
            InitializeComponent();
        }

        private void ExtensionDetailsDialog_Load(object sender, EventArgs e)
        {
            List<ExtensionDetails> ed = MainF.Remote.GetExtensionNames();
            foreach(ExtensionDetails ed2 in ed)
            {
                string[] details = new string[] { ed2.Name, ed2.Version };
                ListViewItem lvc = new ListViewItem(details);
                listView1.Items.Add(lvc);
            }
        }
    }
}