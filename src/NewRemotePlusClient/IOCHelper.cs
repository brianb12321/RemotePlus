 using NewRemotePlusClient.ViewModels;
using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewRemotePlusClient.IOC;
using RemotePlusClient.CommonUI.ConnectionClients;

namespace NewRemotePlusClient
{
    public static class IOCHelper
    {
        public static MainWindowViewModel MainWindow => IOCContainer.GetService<MainWindowViewModel>();
        public static IUIManager UI => IOCContainer.GetService<IUIManager>();
        public static ServiceClient Client => IOCContainer.GetService<ServiceClient>();
        public static ILoginManager LoginManager => IOCContainer.GetService<ILoginManager>();
    }
}
