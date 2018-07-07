using BetterLogger;
using GalaSoft.MvvmLight.Messaging;
using NewRemotePlusClient.Models;
using RemotePlusClient.CommonUI.ConnectionClients;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.IOC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NewRemotePlusClient.ViewModels
{
    public class MainWindowViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public ObservableCollection<ITabPage> TabPagesLoaded { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ILogFactory MainServerLogger { get; private set; }
        private ApplicationStatus _status = ApplicationStatus.Ready;
        private CommunicationState _connectionState = CommunicationState.Created;
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
        public CommunicationState ConnectionState
        {
            get { return _connectionState; }
            set
            {
                _connectionState = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConnectionState)));
            }
        }
        public MainWindowViewModel()
        {
            MainServerLogger = new BaseLogFactory();
            MainServerLogger.AddLogger(new TextBoxLogger());
            TabPagesLoaded = new ObservableCollection<ITabPage>();
            TabPagesLoaded.CollectionChanged += TabPagesLoaded_CollectionChanged;
        }

        private void TabPagesLoaded_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    ((ITabPage)e.NewItems[0]).CloseRequested += MainWindowViewModel_CloseRequested;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    ((ITabPage)e.OldItems[0]).CloseRequested -= MainWindowViewModel_CloseRequested;
                    break;
            }
        }

        private void MainWindowViewModel_CloseRequested(object sender, EventArgs e)
        {
            TabPagesLoaded.Remove((ITabPage)sender);
        }
        #region Commands
        public ICommand ExitCommand => new RelayCommand(param => true, param => Application.Current.Shutdown(0));
        public ICommand VersionCommand => new RelayCommand(param => true, param => MainServerLogger.Log("RemotePlus version 1.0.0.0", LogLevel.Info));
        public ICommand OpenConsoleCommand => new RelayCommand(param =>
        {
            //TODO: Check if connection is opened;
            return true;
        }, param =>
        {
            TabPagesLoaded.Add(new Pages.ConsoleViewModel());
        });
        public ICommand ConnectCommand => new RelayCommand(param =>
        {
            //Make sure that the proxy is null
            if (ConnectionState != CommunicationState.Opened)
            {
                return true;
            }
            else
            {
                return false;
            }
        }, param =>
        {
            Messenger.Default.Register<Connection>(this, async (connect) =>
            {
                try
                {
                    Status = ApplicationStatus.Connecting;
                    ServiceClient sc = new ServiceClient(new ClientCallback(), _ConnectionFactory.BuildBinding(), new EndpointAddress(connect.Address));
                    await Task.Run(() =>
                    {
                        sc.ChannelFactory.Opening += (sender, e) => ConnectionState = CommunicationState.Opening;
                        sc.ChannelFactory.Opened += (sender, e) => ConnectionState = CommunicationState.Opened;
                        sc.ChannelFactory.Closing += (sender, e) => ConnectionState = CommunicationState.Closing;
                        sc.ChannelFactory.Closed += (sender, e) => ConnectionState = CommunicationState.Closed;
                        sc.ChannelFactory.Faulted += (sender, e) => ConnectionState = CommunicationState.Faulted;
                        sc.Open();
                        IOCContainer.Provider.Bind<ServiceClient>().ToConstant(sc);
                    });
                    sc.Register(connect.RegisterObject);
                    Status = ApplicationStatus.Ready;
                }
                catch (Exception ex)
                {
                    IOCHelper.UI.ShowMessageBox($"An error occurred when establishing a connection to the server: {ex.Message}", "Failed to connect to server.", MessageButtons.OK, MessageType.Error);
                    Status = ApplicationStatus.Ready;
                }
                finally
                {
                    Messenger.Default.Unregister<Connection>(this);
                }
            });
            IOCHelper.UI.Show<Views.ConnectView>();
        });
        #endregion
    }
}