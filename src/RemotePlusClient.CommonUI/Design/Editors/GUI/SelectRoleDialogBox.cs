using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.CommonUI.Design.Editors.GUI
{
    public partial class SelectRoleDialogBox : Form
    {
        public string SelectedRoleName { get; set; }
        public SelectRoleDialogBox()
        {
            InitializeComponent();
        }

        private void SelectRoleDialogBoxcs_Load(object sender, EventArgs e)
        {
            foreach (RemotePlusLibrary.Security.AccountSystem.Role roles in RemotePlusLibrary.Security.AccountSystem.Role.GlobalPool.Roles)
            {
                listBox1.Items.Add(roles.RoleName);
            }
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("You must select a role.");
            }
            else
            {
                SelectedRoleName = listBox1.SelectedItem.ToString();
                DialogResult = DialogResult.OK;
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
