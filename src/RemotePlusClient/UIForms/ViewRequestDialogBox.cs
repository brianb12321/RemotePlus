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
using RemotePlusLibrary.RequestSystem;

namespace RemotePlusClient.UIForms
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
            foreach(IDataRequest req in RequestStore.GetAll())
            {
                listView1.Items.Add(new ListViewItem(new string[] { req.FriendlyName, req.URI, req.Description }));
                temp.Add(req.URI, req);
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
                selectedItem.UpdateProperties();
            }
        }
    }
}
