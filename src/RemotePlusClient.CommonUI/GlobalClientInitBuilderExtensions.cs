using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusClient.CommonUI.Requests;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Core.NodeStartup;
using RemotePlusLibrary.RequestSystem;

namespace RemotePlusClient.CommonUI
{
    public static class GlobalClientInitBuilderExtensions
    {
        public static INodeBuilder<IClientBuilder> SetupApplicationPrerequisites(this INodeBuilder<IClientBuilder> builder)
        {
            return builder.AddTask(() =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            });
        }

        public static INodeBuilder<IClientBuilder> SetupNotifyIcon(this INodeBuilder<IClientBuilder> builder)
        {
            return builder.AddTask(() =>
            {
                NotifyIcon applicationIcon = IOCContainer.GetService<NotifyIcon>();
                applicationIcon.Icon = SystemIcons.Application;
                applicationIcon.Visible = true;
            });
        }

        public static INodeBuilder<IClientBuilder> ConfigureRequests(this INodeBuilder<IClientBuilder> builder)
        {
            return builder.AddTask(() =>
            {
                RequestStore.Add(new RequestStringRequest());
                RequestStore.Add(new ColorRequest());
                RequestStore.Add(new MessageBoxRequest());
                RequestStore.Add(new SelectLocalFileRequest());
                RequestStore.Add(new SelectFileRequest());
                RequestStore.Add(new SendFilePackageRequest());
            });
        }

        public static INodeBuilder<IClientBuilder> ConfigureEndpointDescription(
            this INodeBuilder<IClientBuilder> builder, Action<ServiceEndpoint> configure)
        {
            return builder.AddTask(() => configure?.Invoke(IOCContainer.GetService<ServiceEndpoint>()));
        }
    }
}