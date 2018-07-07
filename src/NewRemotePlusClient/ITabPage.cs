using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace NewRemotePlusClient
{
    public interface ITabPage
    {
        string Name { get; set; }
        ICommand CloseCommand { get; set; }
        event EventHandler CloseRequested;
    }
    public abstract class TabPage : ITabPage
    {
        public string Name { get; set; }
        public ICommand CloseCommand { get; set; }
        public event EventHandler CloseRequested;
        public TabPage()
        {
            CloseCommand = new RelayCommand(param => true, param => CloseRequested?.Invoke(this, EventArgs.Empty));
        }
    }
}
