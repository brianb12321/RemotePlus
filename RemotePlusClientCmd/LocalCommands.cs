using Logging;
using RemotePlusLibrary.Extension.CommandSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClientCmd
{
    partial class Program
    {
        [CommandHelp("Shows help for local commands.")]
        static int Help(string[] args)
        {
            Logger.AddOutput(RemotePlusConsole.ShowHelp(LocalCommands, args), OutputLevel.Info);
            return (int)CommandStatus.Success;
        }
        [CommandHelp("Clears the console screen.")]
        static int clearScreen(string[] args)
        {
            Console.Clear();
            return (int)CommandStatus.Success;
        }
        [CommandHelp("Closes the connection to the server.")]
        static int close(string[] args)
        {
            Remote.Disconnect();
            channel.Close();
            Environment.Exit(0);
            return (int)CommandStatus.Success;
        }
    }
}
