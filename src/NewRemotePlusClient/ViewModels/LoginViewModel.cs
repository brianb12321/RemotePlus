using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using NewRemotePlusClient.IOC;
using RemotePlusLibrary.Security.AccountSystem;

namespace NewRemotePlusClient.ViewModels
{
    public class LoginViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private UserCredentials _credentials = new UserCredentials();
        public string UserName
        {
            get { return _credentials.Username; }
            set
            {
                _credentials.Username = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserName)));
            }
        }
        public string Password
        {
            get { return _credentials.Password; }
            set
            {
                _credentials.Password = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password)));
            }
        }
        public ICommand LoginCommand { get; private set; }

        public LoginViewModel()
        {
            CancelCommand = new RelayCommand(param => true, param => ((Window)param).Close());
            LoginCommand = new RelayCommand(param => true, param =>
            {
                Messenger.Default.Send(_credentials);
                ((Window)param).Close();
            });
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
