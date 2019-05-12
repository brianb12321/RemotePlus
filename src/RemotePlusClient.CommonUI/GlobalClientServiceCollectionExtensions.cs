using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusLibrary.Core.IOC;

namespace RemotePlusClient.CommonUI
{
    public static class GlobalClientServiceCollectionExtensions
    {
        public static IServiceCollection UseNotifyIcon(this IServiceCollection services)
        {
            return services.AddSingleton<NotifyIcon>(new NotifyIcon());
        }
    }
}