using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient
{
    public interface IDialogForm<out TViewModel> : IForm<Form> where TViewModel : BaseViewModel
    {
        TViewModel ViewModel { get; }
        DialogResult Open(object args);
    }
}