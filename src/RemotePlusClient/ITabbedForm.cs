using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient
{
    public enum FormPosition { Up, Down, Left, Right }
    public interface ITabbedForm<TViewModel> : IForm<UserControl> where TViewModel : class
    {
        TViewModel ViewModel { get; }
        FormPosition Location { get; set; }
    }
}