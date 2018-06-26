using RemotePlusClient.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.UIForms.SettingDialogs
{
    public partial class ConsoleSettingsDialogBox : Form
    {
        ConsoleSettings settings = null;
        public ConsoleSettingsDialogBox(ConsoleSettings s)
        {
            settings = s;
            InitializeComponent();
        }

        private void ConsoleSettingsDialogBox_Load(object sender, EventArgs e)
        {
            panel1.BackColor = settings.DefaultInfoColor;
            panel2.BackColor = settings.DefaultWarningColor;
            panel3.BackColor = settings.DefaultErrorColor;
            panel4.BackColor = settings.DefaultDebugColor;
            panel5.BackColor = settings.DefaultBackColor;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if(cd.ShowDialog() == DialogResult.OK)
            {
                panel1.BackColor = cd.Color;
                settings.DefaultInfoColor = cd.Color;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                panel2.BackColor = cd.Color;
                settings.DefaultWarningColor = cd.Color;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                panel3.BackColor = cd.Color;
                settings.DefaultErrorColor = cd.Color;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                panel4.BackColor = cd.Color;
                settings.DefaultDebugColor = cd.Color;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                panel5.BackColor = cd.Color;
                settings.DefaultBackColor = cd.Color;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            settings.Save();
            MessageBox.Show("Saved", "RemotePlusClient");
            MainF.ConsoleObj.Logger.AddOutput("Saved console config file.", Logging.OutputLevel.Info);
        }

        private void btn_selectFont_Click(object sender, EventArgs e)
        {
            using (FontDialog fd = new FontDialog())
            {
                if(fd.ShowDialog() == DialogResult.OK)
                {
                    settings.DefaultFont = TypeDescriptor.GetConverter(typeof(Font)).ConvertToString(fd.Font);
                }
            }
        }
    }
}
