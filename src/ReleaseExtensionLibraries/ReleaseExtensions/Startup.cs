using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension;
using RemotePlusServer;
using RemotePlusLibrary.Extension.CommandSystem;
using System.IO;
using System.Reflection;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using RemotePlusLibrary.Extension.ExtensionLoader.Initialization;
using RemotePlusServer.Core;
using BetterLogger;

namespace ReleaseExtensions
{
    public sealed class Startup : ILibraryStartup
    {
        void ILibraryStartup.Init(ILibraryBuilder builder, IInitEnvironment env)
        {
            ServerManager.Logger.Log($"Init position {env.InitPosition}", LogLevel.Debug, "ReleaseExtensions");
            ServerManager.Logger.Log("Welcome to \"ReleaseExtension.\" This library contains some useful tools that demonstrates the powers of \"RemotePlus\"", LogLevel.Info, "ReleaseExtensions");
            ServerManager.ServerRemoteService.Commands.Add("releaseExtensionAbout", releaseExtensionAbout);
            //Test Code
            ServerManager.ServerRemoteService.Commands.Add("textBoxTest", cmdTextBox);
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
        [CommandHelp("Tests the client cmd text box.")]
        [CommandBehavior(IndexCommandInHelp = false)]
        CommandResponse cmdTextBox(CommandRequest args, CommandPipeline pipe)
        {
            if(ServerManager.ServerRemoteService.RemoteInterface.Client.ClientType == RemotePlusLibrary.Client.ClientType.CommandLine)
            {
                var result = ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(new RemotePlusLibrary.RequestSystem.RequestBuilder("rcmd_textBox", "What's your favorite beach?", null));
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Your favorite beach is {result.Data}", LogLevel.Info, "ReleaseExtensions");
                return new CommandResponse((int)CommandStatus.Success);
            }
            else
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("You must be on a command line based client.", LogLevel.Error, "ReleaseExtensions");
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
    }
}