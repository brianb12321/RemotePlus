using GalaSoft.MvvmLight.Messaging;
using NewRemotePlusClient.Models;
using RemotePlusLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NewRemotePlusClient.ViewModels
{
    public class ConnectViewModel : BaseViewModel
    {
        public Connection Address { get; set; } = new Connection();
        public ICommand ConnectNowCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand EditRegObjectCommand { get; private set; }
        public ConnectViewModel()
        {
            ConnectNowCommand = new RelayCommand(param =>
            {
                if (string.IsNullOrEmpty(Address.Address))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }, param =>
            {
                Messenger.Default.Send(Address);
                ((Window)param).Close();
            });
            CancelCommand = new RelayCommand(param => true, param => ((Window)param).Close());
            EditRegObjectCommand = new RelayCommand(param => true, param =>
            {
                Messenger.Default.Register<RegisterationObject>(this, regObj => Address.RegisterObject = regObj);
                IOCHelper.UI.Show<Views.EditRegistirationObject>();
            });
        }
    }
}