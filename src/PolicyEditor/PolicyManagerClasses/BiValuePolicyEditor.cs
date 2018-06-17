using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PolicyEditor
{
    public partial class BiValuePolicyEditor : Form, IPolicyEditor
    {
        PolicyView view { get; set; }
        public BiValuePolicyEditor()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            view.Policy.Values["Value"] = checkBox1.Checked.ToString();
            DialogResult = DialogResult.OK;
        }

        private void biValuePolicyEditor_Load(object sender, EventArgs e)
        {
            lbl_shortName.Text = view.Policy.ShortName;
            checkBox1.Text = view.Policy.ShortDescription;
            checkBox1.Checked = (view.Policy.Values["Value"] == "True") ? true : false;
            richTextBox1.Text = view.Policy.Description;
        }
        Dictionary<string, string> IPolicyEditor.ShowDialog(PolicyView policy)
        {
            view = policy;
            if (ShowDialog() == DialogResult.OK)
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
