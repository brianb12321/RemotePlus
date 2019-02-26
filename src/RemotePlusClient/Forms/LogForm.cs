using RemotePlusClient.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.Forms
{
    public partial class LogForm : UserControl, ITabbedForm<LogViewModel>
    {
        public LogForm()
        {
            InitializeComponent();
        }
        public Guid FormID => Guid.NewGuid();

        public LogViewModel ViewModel { get; private set; }
        public UserControl Form => this;

        public string FormName => this.Name;

        public object FormTag { get; set; }
        FormPosition ITabbedForm<LogViewModel>.Location { get; set; } = FormPosition.Down;

        public void CloseForm()
        {
            Dispose();
        }

        private void LogForm_Load(object sender, EventArgs e)
        {
            ViewModel = new LogViewModel(logBox, FormName);
        }
    }
}