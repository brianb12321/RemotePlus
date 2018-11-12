using BetterLogger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.RequestSystem.DefaultRequestOptions;
using RemotePlusLibrary.Security.AccountSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    public static class GlobalServerBuilderExtensions
    {
        public static void InitializeKnownTypes()
        {
            GlobalServices.Logger.Log("Adding default known types.", LogLevel.Info);
            DefaultKnownTypeManager.LoadDefaultTypes();
            GlobalServices.Logger.Log("Adding UserAccount to known type list.", LogLevel.Debug);
            DefaultKnownTypeManager.AddType(typeof(UserAccount));
            DefaultKnownTypeManager.AddType(typeof(FileDialogRequestOptions));
            DefaultKnownTypeManager.AddType(typeof(FileRequestOptions));
            DefaultKnownTypeManager.AddType(typeof(MessageBoxRequestOptions));
            DefaultKnownTypeManager.AddType(typeof(PromptRequestOptions));
            DefaultKnownTypeManager.AddType(typeof(SimpleMenuRequestOptions));
            DefaultKnownTypeManager.AddType(typeof(SMenuRequestOptions));
        }
        public static IServerBuilder InitializeKnownTypes(this IServerBuilder builder)
        {
            return builder.AddTask(() => InitializeKnownTypes());
        }
    }
}
