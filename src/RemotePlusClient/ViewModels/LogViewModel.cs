using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.ViewModels
{
    public class LogViewModel : BaseViewModel
    {
        private RichTextBox _box = null;
        public LogViewModel(RichTextBox b, string name) : base(name)
        {
            _box = b;
        }
        public void AppendText(string text)
        {
            _box.AppendText(text);
        }
    }
}