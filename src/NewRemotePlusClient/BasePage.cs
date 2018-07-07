using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NewRemotePlusClient
{
    public class BasePage<VM> : Page where VM : BaseViewModel, new()
    {
        public BasePage()
        {
            DataContext = new VM();
        }
        public VM ViewModel { get; set; }
    }
}
