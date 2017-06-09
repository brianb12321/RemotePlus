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
using RemotePlusClient.CommonUI;

namespace RemotePlusClient
{
    public partial class ViewRequestDialogBox : Form
    {
        Dictionary<string, IDataRequest> temp;
        public ViewRequestDialogBox()
        {
            InitializeComponent();
        }

        private void ViewRequestDialogBox_Load(object sender, EventArgs e)
        {
            temp = new Dictionary<string, IDataRequest>();
            foreach(KeyValuePair<string, IDataRequest> req in RequestStore.GetAll())
            {
                listView1.Items.Add(new ListViewItem(new string[] { req.Value.FriendlyName, req.Key, req.Value.Description }));
                temp.Add(req.Key, req.Value);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var selectedItem = temp[listView1.SelectedItems[0].SubItems[1].Text];
            if (selectedItem.ShowProperties != true)
            {
                MessageBox.Show("This item does not support custom properties.", "RemotePlusClient", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var form = selectedItem.GetProperties();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    selectedItem.SaveProperties(form);
                }
            }
        }
    }
}
