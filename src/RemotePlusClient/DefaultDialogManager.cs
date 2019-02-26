using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient
{
    public class DefaultDialogManager : IDialogManager
    {
        private Dictionary<Type, IDialogForm<BaseViewModel>> _dialogs = new Dictionary<Type, IDialogForm<BaseViewModel>>();

        public void AddDialog<TViewModel>(IDialogForm<TViewModel> form) where TViewModel : BaseViewModel
        {
            _dialogs.Add(typeof(TViewModel), form);
        }

        public (TViewModel ViewModel, DialogResult Result) Show<TViewModel>() where TViewModel : BaseViewModel
        {
            return Show<TViewModel>(null);
        }

        public (TViewModel ViewModel, DialogResult Result) Show<TViewModel>(object args) where TViewModel : BaseViewModel
        {
            if (_dialogs.ContainsKey(typeof(TViewModel)))
            {
                var form = _dialogs[typeof(TViewModel)];

                var result = form.Open(args);
                form.CloseForm();
                return ((TViewModel)form.ViewModel, result);
            }
            else
            {
                return (null, DialogResult.None);
            }
        }
    }
}