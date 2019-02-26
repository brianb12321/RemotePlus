using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BetterLogger;

namespace RemotePlusClient.Utils
{
    public class WinFormLogger : ILogger
    {
        IWindowManager _winManager = null;
        public WinFormLogger(IWindowManager manager)
        {
            _winManager = manager;
        }
        public void Log(string message, LogLevel level)
        {
            _winManager.GetAllByKind<ViewModels.LogViewModel>().ToList().ForEach(view => view.ViewModel.AppendText(message + Environment.NewLine));
        }
    }
}