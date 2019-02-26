using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClient
{
    /// <summary>
    /// Provides methods for adding and retreiving tabbed windows.
    /// </summary>
    public interface IWindowManager
    {
        void Open<TViewModel>(ITabbedForm<TViewModel> form, object args) where TViewModel : BaseViewModel;
        void Close<TViewModel>(ITabbedForm<TViewModel> form) where TViewModel : BaseViewModel;
        void Close<TViewModel>(FormPosition pos, Guid id) where TViewModel : BaseViewModel;
        ITabbedForm<TViewModel> Get<TViewModel>(FormPosition pos, Guid id) where TViewModel : BaseViewModel;
        IEnumerable<ITabbedForm<TViewModel>> GetAllByKind<TViewModel>() where TViewModel : BaseViewModel;
        IEnumerable<ITabbedForm<TViewModel>> GetAllByID<TViewModel>(Guid id) where TViewModel : BaseViewModel;
    }
}