using System;
using System.Windows.Forms;
using RemotePlusServer.Core;
using BetterLogger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusServer;

namespace ReleaseExtensions
{
    public sealed class Startup : ILibraryStartup
    {
        void ILibraryStartup.Init(IServiceCollection services)
        {
            GlobalServices.Logger.Log("Welcome to \"ReleaseExtension.\" This library contains some useful tools that demonstrates the powers of \"RemotePlus\"", LogLevel.Info, "ReleaseExtensions");
            IOCContainer.GetService<IScriptingEngine>().GetDefaultModule().AddVariable("showMessageBox", new Func<IClientContext, string, string, MessageBoxButtons, MessageBoxIcon, DialogResult>(showMessageBoxScriptMethod));
        }
        DialogResult showMessageBoxScriptMethod(IClientContext context, string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            var rb = new MessageBoxRequestBuilder()
            {
                Message = message,
                Caption = caption,
                Buttons = buttons,
                Icons = icon
            };
            return (DialogResult)Enum.Parse(typeof(DialogResult), context.GetClient<RemoteClient>().ClientCallback.RequestInformation(rb).Data.ToString());
        }
        [CommandHelp("Describes about the ReleaseExtensionsLibrary.")]
        CommandResponse releaseExtensionAbout(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            currentEnvironment.WriteLine("ReleaseExtension is a test of the extension system.");
            return new CommandResponse((int)CommandStatus.Success);
        }

        public void PostInit()
        {
            
        }
    }
}