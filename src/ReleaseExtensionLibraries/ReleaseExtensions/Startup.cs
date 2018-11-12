using System;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using System.Windows.Forms;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using RemotePlusServer.Core;
using BetterLogger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestOptions;

namespace ReleaseExtensions
{
    public sealed class Startup : ILibraryStartup
    {
        void ILibraryStartup.Init(ILibraryBuilder builder, IInitEnvironment env)
        {
            GlobalServices.Logger.Log($"Init position {env.InitPosition}", LogLevel.Debug, "ReleaseExtensions");
            GlobalServices.Logger.Log("Welcome to \"ReleaseExtension.\" This library contains some useful tools that demonstrates the powers of \"RemotePlus\"", LogLevel.Info, "ReleaseExtensions");
            ServerManager.ServerRemoteService.Commands.Add("releaseExtensionAbout", releaseExtensionAbout);
            ServerManager.ScriptBuilder.AddScriptObject("showMessageBox", new Func<string, string, MessageBoxButtons, MessageBoxIcon, DialogResult>(showMessageBoxScriptMethod), "Displays a message box on the client's screen.", RemotePlusLibrary.Scripting.ScriptGlobalType.Function);
        }
        [RemotePlusLibrary.Scripting.IndexScriptObject]
        DialogResult showMessageBoxScriptMethod(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return (DialogResult)Enum.Parse(typeof(DialogResult), ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(RemotePlusLibrary.RequestSystem.RequestBuilder.RequestMessageBox(message, caption, buttons, icon)).Data.ToString());
        }
        [CommandHelp("Describes about the ReleaseExtensionsLibrary.")]
        CommandResponse releaseExtensionAbout(CommandRequest args, CommandPipeline pipe)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("ReleaseExtension is a test of the extension system.", LogLevel.Info);
            return new CommandResponse((int)CommandStatus.Success);
        }
    }
}