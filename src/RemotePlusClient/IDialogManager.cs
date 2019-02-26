using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient
{
    public interface IDialogManager
    {
        (TViewModel ViewModel, DialogResult Result) Show<TViewModel>() where TViewModel : BaseViewModel;
        (TViewModel ViewModel, DialogResult Result) Show<TViewModel>(object args) where TViewModel : BaseViewModel;
        void AddDialog<TViewModel>(IDialogForm<TViewModel> form) where TViewModel : BaseViewModel;
    }
}