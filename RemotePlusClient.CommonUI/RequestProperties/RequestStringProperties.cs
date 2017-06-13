using RemotePlusClient.CommonUI.RequestSettings;
using RemotePlusLibrary.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.CommonUI.RequestProperties
{
    public partial class RequestStringProperties : Form
    {
        public RequestStringSettings setting { get; private set; }
        public RequestStringProperties()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                setting.Save();
                MessageBox.Show("Setting saved.", "RequestStringProperties");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unknown error: {ex.Message}", "RequestStringProperties");
            }
        }

        private void RequestStringProperties_Load(object sender, EventArgs e)
        {
            setting = new RequestStringSettings();
            try
            {
                setting.Load();
                propertyGrid1.SelectedObject = setting;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("File not found. Creating new file.", "RequestStringProperties");
                setting = new RequestStringSettings();
                setting.Save();
            }
        }
    }
}
