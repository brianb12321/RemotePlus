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

namespace ReleaseExtensions
{
    public sealed class Startup : ILibraryStartup
    {
        void ILibraryStartup.Init(ILibraryBuilder builder, IInitEnvironment env)
        {
            ServerManager.Logger.AddOutput($"Init position {env.InitPosition}", Logging.OutputLevel.Debug, "ReleaseExtensions");
            ServerManager.Logger.AddOutput(new Logging.LogItem(Logging.OutputLevel.Info, "Welcome to \"ReleaseExtension.\" This library contains some useful tools that demonstrates the powers of \"RemotePlus\"", "ReleaseExtensions") { Color = Console.ForegroundColor });
            ServerManager.DefaultService.Commands.Add("releaseExtensionAbout", releaseExtensionAbout);
            //Test Code
            ServerManager.DefaultService.Commands.Add("menuTest", menuTest);
            //Test Code
            ServerManager.DefaultService.Commands.Add("textBoxTest", cmdTextBox);
        }

        [CommandHelp("Describes about the ReleaseExtensionsLibrary.")]
        CommandResponse releaseExtensionAbout(CommandRequest args, CommandPipeline pipe)
        {
            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "ReleaseExtension is a test of the extension system."));
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Tests the client cmd menu.")]
        [CommandBehavior(IndexCommandInHelp = false)]
        CommandResponse menuTest(CommandRequest args, CommandPipeline pipe)
        {
            if (ServerManager.DefaultService.Remote.Client.ClientType == RemotePlusLibrary.ClientType.CommandLine)
            {
                Dictionary<string, string> sampleMovies = new Dictionary<string, string>();
                sampleMovies.Add("0", "The Lord of the Rings Trilagy");
                sampleMovies.Add("1", "Star Wars: The Force Awakens");
                sampleMovies.Add("2", "Lilo and Stitch");
                sampleMovies.Add("3", "My Little Pony The Movie");
                sampleMovies.Add("4", "Dancing with Wolves");
                sampleMovies.Add("5", "The Hobbit");
                sampleMovies.Add("6", "Cars 2");
                sampleMovies.Add("7", "Jaws");

                var builder = new RemotePlusLibrary.RequestBuilder("rcmd_smenu", "What is your favorite movie?\nEXPLANATION: Please select one of your favorite moves from the list.\nPLEASE NOTE: if you select Jaws, the server may kick you off.", sampleMovies);
                builder.AcqMode = RemotePlusLibrary.AcquisitionMode.ThrowIfException;
                builder.Metadata.Add("BorderHeader", "true");
                builder.Metadata.Add("HeaderBackColor", Color.Violet.Name);
                builder.Metadata.Add("SelectBackColor", Color.White.Name);
                builder.Metadata.Add("ConsoleBackColor", Color.DarkSlateGray.Name);
                var result = ServerManager.DefaultService.Remote.Client.ClientCallback.RequestInformation(builder);
                var resultNum = (char)result.Data;
                switch (resultNum)
                {
                    case '0':
                        ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "Ah, Good Movie! Great Power can bring great evil.", "ReleaseExtensions"));
                        break;
                    case '1':
                        ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "May the force be with you.", "ReleaseExtensions"));
                        break;
                    case '2':
                        ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "What kinds of adventures await in Hawaii?", "ReleaseExtensions"));
                        break;
                    case '3':
                        ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "This program can bring such dangers to Aquestria.", "ReleaseExtensions"));
                        break;
                    case '4':
                        ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "When feeling lonely, you could be friends with a wolf.", "ReleaseExtensions"));
                        break;
                    case '5':
                        ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "I have never seen this move", "ReleaseExtensions"));
                        break;
                    case '6':
                        ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "The camera is gone!, Phin", "ReleaseExtensions"));
                        break;
                    case '7':
                        ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "Processing outrageous choice...", "ReleaseExtensions"));
                        Thread.Sleep(10000);
                        ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "thinking of a response...", "ReleaseExtensions"));
                        Thread.Sleep(2000);
                        ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "Thinking...", "ReleaseExtensions"));
                        for(int i = 0; i < 60; i++)
                        {
                            ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(".");
                            Thread.Sleep(1000);
                        }
                        Dictionary<string, string> retractOptions = new Dictionary<string, string>();
                        retractOptions.Add("0", "Yes, I am sorry");
                        retractOptions.Add("1", "No, I don't care");
                        var retractBuilder = new RemotePlusLibrary.RequestBuilder("rcmd_smenu", "I HATE THAT MOVIE! DO YOU WANT TO RETRACT YOUR STATEMENT?", retractOptions);
                        retractBuilder.Metadata.Add("ConsoleBackColor", Color.Red.Name);
                        retractBuilder.Metadata.Add("HeaderForeground", Color.Yellow.Name);
                        retractBuilder.Metadata.Add("SelectBackColor", Color.Yellow.Name);
                        retractBuilder.Metadata.Add("MenuItemForeground", Color.Yellow.Name);
                        retractBuilder.Metadata.Add("SelectForeColor", Color.Black.Name);
                        var retractResult = ServerManager.DefaultService.Remote.Client.ClientCallback.RequestInformation(retractBuilder);
                        switch((char)retractResult.Data)
                        {
                            case '0':
                                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "OK", "ReleaseExtensions"));
                                break;
                            case '1':
                                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, "Fine! So be it!", "ReleaseExtensions"));
                                ServerManager.DefaultService.Remote.Client.ClientCallback.Disconnect("You have been kicked off the server by the creator of RemotePlus");
                                break;
                        }
                        break;
                    default:
                        ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Error, "Houston, We have a problem.", "ReleaseExtensions"));
                        return new CommandResponse((int)CommandStatus.Fail);
                }
                return new CommandResponse((int)CommandStatus.Success);
            }
            else
            {
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Error, "You must be on a command line based client.", "ReleaseExtensions"));
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Tests the client cmd text box.")]
        [CommandBehavior(IndexCommandInHelp = false)]
        CommandResponse cmdTextBox(CommandRequest args, CommandPipeline pipe)
        {
            if(ServerManager.DefaultService.Remote.Client.ClientType == RemotePlusLibrary.ClientType.CommandLine)
            {
                var result = ServerManager.DefaultService.Remote.Client.ClientCallback.RequestInformation(new RemotePlusLibrary.RequestBuilder("rcmd_textBox", "What's your favorite beach?", null));
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, $"Your favorite beach is {result.Data}", "ReleaseExtensions"));
                return new CommandResponse((int)CommandStatus.Success);
            }
            else
            {
                ServerManager.DefaultService.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Error, "You must be on a command line based client.", "ReleaseExtensions"));
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
    }
}