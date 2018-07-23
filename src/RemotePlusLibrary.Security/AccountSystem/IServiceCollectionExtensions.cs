using RemotePlusLibrary.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Security.AccountSystem
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection UseAuthentication<TAuthenticationImpl>(this IServiceCollection services)
        {
            return services.AddSingleton<IAccountManager, TAuthenticationImpl>();
        }
    }
}