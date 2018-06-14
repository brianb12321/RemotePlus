using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusLibrary.AccountSystem;

namespace RemotePlusLibrary.Editors.PolicyManagerClasses
{
    public partial class EditPolicyDialogBox : Form, IPolicyEditor
    {
        PolicyView view { get; set; }
        public EditPolicyDialogBox()
        {
            InitializeComponent();
        }

        private void EditPolicyDialogBox_Load(object sender, EventArgs e)
        {
            lbl_shortName.Text = view.Policy.ShortName;
            tb_value.Text = view.Policy.Values["Value"];
            richTextBox1.Text = view.Policy.Description;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            view.Policy.Values["Value"] = tb_value.Text;
            DialogResult = DialogResult.OK;
        }

        Dictionary<string, string> IPolicyEditor.ShowDialog(PolicyView policy)
        {
            view = policy;
            if(ShowDialog() == DialogResult.OK)
            {
                return view.Policy.Values;
            }
            else
            {
                return null;
            }
        }
    }
}
