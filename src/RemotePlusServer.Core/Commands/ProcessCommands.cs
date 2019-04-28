using System;
using System.Diagnostics;
using System.Linq;
using NDesk.Options;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusServer.Core.ExtensionSystem;

namespace RemotePlusServer.Core.Commands
{
    [ExtensionModule]
    public class ProcessCommands : ServerCommandClass
    {
        [CommandHelp("Starts a process on the server.")]
        public CommandResponse ps(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            currentEnvironment.WriteLine();
            bool waitForExit = false;
            bool showHelp = false;
            bool enterDebug = false;
            bool exitDebug = false;
            Process p = new Process();
            OptionSet set = new OptionSet()
                .Add("program|p=", "The process name to start. If shell execution has been disabled, you must specify a full path.", v => p.StartInfo.FileName = v)
                .Add("arguments|a=", "Arguments to pass into the process.", v => p.StartInfo.Arguments = v)
                .Add("DisableShellExecute|s", "Disables the OS to execute the program directly.", v => p.StartInfo.UseShellExecute = false)
                .Add("redirectStdOut|o", "Redirects StdOut to RemotePlus StdOut. NOTE: Shell execution will be disabled.", v =>
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.OutputDataReceived += (sender, e) =>
                    {
                        currentEnvironment.WriteLine(e.Data);
                    };
                })
                .Add("redirectStdError|r", "Redirects StdError to RemotePlus StdError. NOTE: Shell execution will be disabled.", v =>
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardError = true;
                    p.ErrorDataReceived += (sender, e) =>
                    {
                        currentEnvironment.WriteLineError(e.Data);
                    };
                })
                .Add("admin|m", "Starts the process as an administrator.", v => p.StartInfo.Verb = "runas")
                .Add("redirectStdIn|i", "Redirects StdIn to RemotePlus StdIn. NOTE: Shell execution will be disabled.", v =>
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardInput = true;
                })
                .Add("disableWindow|w", "Disables a window from appearing.", v => p.StartInfo.CreateNoWindow = true)
                .Add("waitForExit|e", "Waits for the process to exit before returning control back to the client.", v => waitForExit = true)
                .Add("enterDebug", "Puts the server process into debug mode. NOTE: Must have appropriate privileges.", v => enterDebug = true)
                .Add("exitDebug", "Puts the server process out of debug mode.", v => exitDebug = true)
                .Add("help|?", "Shows the help screen.", v => showHelp = true);
            set.Parse(args.Arguments.Select(a => a.ToString()));
            if (showHelp)
            {
                set.WriteOptionDescriptions(currentEnvironment.Out);
                return new CommandResponse((int)CommandStatus.Success);
            }
            if (enterDebug)
            {
                Process.EnterDebugMode();
                return new CommandResponse((int)CommandStatus.Success);
            }
            if (exitDebug)
            {
                Process.LeaveDebugMode();
                return new CommandResponse((int)CommandStatus.Success);
            }
            p.Start();
            if (p.StartInfo.RedirectStandardInput) p.StandardInput.Write(currentEnvironment.ReadToEnd());
            if (p.StartInfo.RedirectStandardOutput) p.BeginOutputReadLine();
            if (p.StartInfo.RedirectStandardError) p.BeginErrorReadLine();
            if (waitForExit)
            {
                var reg = args.CancellationToken.Register(() =>
                {
                    try
                    {
                        p.Kill();
                    }
                    catch (InvalidOperationException) { }
                });
                p.WaitForExit();
                reg.Dispose();
                return new CommandResponse(p.ExitCode);
            }
            else
            {
                return new CommandResponse((int)CommandStatus.Success);
            }
        }

        [CommandHelp("Enumerates and manipulates active processes.")]
        public CommandResponse process(CommandRequest args, CommandPipeline pipe,
            ICommandEnvironment currentEnvironment)
        {
            bool showHelp = false;
            string action = string.Empty;
            OptionSet options = new OptionSet()
                .Add("help|?", "Shows the help screen.", v => showHelp = true);
            options.Parse(args.Arguments.Select(a => a.ToString()));
            if (showHelp)
            {
                options.WriteOptionDescriptions(currentEnvironment.Out);
                return new CommandResponse((int)CommandStatus.Success);
            }

            switch (action)
            {
                default:
                    return new CommandResponse((int)CommandStatus.Success);
            }
        }
        public override void InitializeServices(IServiceCollection services)
        {
            Commands.Add("ps", ps);
            Commands.Add("process", process);
        }
    }
}