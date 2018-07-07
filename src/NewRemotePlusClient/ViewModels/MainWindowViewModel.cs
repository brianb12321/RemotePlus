using BetterLogger;
using GalaSoft.MvvmLight.Messaging;
using NewRemotePlusClient.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NewRemotePlusClient.ViewModels
{
    public class MainWindowViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ILogFactory MainServerLogger { get; private set; }
        private ApplicationStatus _status = ApplicationStatus.Ready;
        public ApplicationStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
            }
        }
        public MainWindowViewModel()
        {
            MainServerLogger = new BaseLogFactory();
            MainServerLogger.AddLogger(new TextBoxLogger());
        }
        #region Commands
        public ICommand ExitCommand => new RelayCommand(param => true, param => Application.Current.Shutdown(0));
        public ICommand VersionCommand => new RelayCommand(param => true, param => MainServerLogger.Log("RemotePlus version 1.0.0.0", LogLevel.Info));
        public ICommand ConnectCommand => new RelayCommand(param =>
        {
            //Make sure that the proxy is null
            if (true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }, param =>
        {
            Messenger.Default.Register<Connection>(this, (connect) =>
            {

            });
            IOCHelper.UI.Show<Views.ConnectView>();
        });
        #endregion
    }
}