using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BetterLogger;
using NDesk.Options;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.RequestSystem.DefaultUpdateRequestBuilders;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusServer.Core.ExtensionSystem;

namespace RemotePlusServer.Core.Commands
{
    [ExtensionModule]
    public class DefaultCommands : ServerCommandClass
    {
        private IRemotePlusService<ServerRemoteInterface> _service;
        ICommandSubsystem<IServerCommandModule> _commandSubsystem;
        IResourceManager _resourceManager;
        IScriptingEngine _scriptingEngine;

        #region Commands
        
        [CommandBehavior(IndexCommandInHelp = false)]
        public CommandResponse progTest(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            var client = currentEnvironment.ClientContext.GetClient<RemoteClient>();
            client.ClientCallback.RequestInformation(new ProgressRequestBuilder()
            {
                Maximum = 100,
                Message = "This is just a test. The server is performing a very long operation."
            });
            Thread.Sleep(1000);
            for(int i = 0; i < 100; i++)
            {
                if(args.CancellationToken.IsCancellationRequested)
                {
                    client.ClientCallback.DisposeCurrentRequest();
                    throw new OperationCanceledException();
                }
                client.ClientCallback.UpdateRequest(new ProgressUpdateBuilder(i)
                {
                    Text = $"{i} / 100 Ticks"
                });

                Thread.Sleep(250);
            }
            client.ClientCallback.DisposeCurrentRequest();
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandBehavior(IndexCommandInHelp = false)]
        public CommandResponse readLineTest(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            currentEnvironment.Write("Enter something to say: ");
            string result = currentEnvironment.ReadLine();
            currentEnvironment.WriteLine($"You said \"{result}\"");
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Displays a list of commands.")]
        public CommandResponse Help(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            string helpString = string.Empty;
            if(args.Arguments.Count == 2)
            {
                helpString = _commandSubsystem.ShowHelpPage(args.Arguments[1].ToString());
            }
            else
            {
                helpString = _commandSubsystem.ShowHelpScreen();
            }
            currentEnvironment.WriteLine(helpString);
            var response = new CommandResponse((int)CommandStatus.Success);
            response.Metadata.Add("helpText", helpString);
            return response;
        }
        [CommandHelp("Gets the server log.")]
        [HelpPage("logs.txt", Source = HelpSourceType.File)]
        public CommandResponse Logs(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            currentEnvironment.WriteLine(Console.In.ReadToEnd());
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Shuts down the server.")]
        public CommandResponse shutdown(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            currentEnvironment.WriteLineWithColor("Shutting down server in 10 SECONDS. PRESS CONTROL+C NOW, IF YOU WANT TO STOP!", Color.Yellow, Color.Red);
            var t = Task.Delay(1000 * 10, args.CancellationToken);
            var t2 = t.ContinueWith((blalba) =>
            {
                Process.Start("cmd.exe", "/c \"shutdown -s -t 1\"");
            }, args.CancellationToken, TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
            t2.Wait(args.CancellationToken);
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Manages resources on the server.")]
        [HelpPage("vars.txt", Source = HelpSourceType.File)]
        public CommandResponse resex(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            var client = currentEnvironment.ClientContext.GetClient<RemoteClient>();
            string mode = string.Empty;
            string name = string.Empty;
            string creationType = string.Empty;
            string value = string.Empty;
            OptionSet set = new OptionSet()
                .Add("new", "Creates a new resource in the system.", v => mode = "new")
                .Add("view", "View all the resources.", v => mode = "view")
                .Add("save", "Saves all resources to the global resource file.", v => mode = "save")
                .Add("modify", "Modifies a resource's properties.", v => mode = "modify")
                .Add("delete", "Deletes a resource from the system.", v => mode = "delete")
                .Add("resourceName|n=", "The resource name.", v => name = v)
                .Add("resourceType|t=", "The resource type to create when creating a resource.", v => creationType = v)
                .Add("value|v=", "The value of the resource to edit or create.", v => value = v)
                .Add("help|?", "Shows the help screen", v => mode = "help");
            set.Parse(args.Arguments.Select(e => e.ToString()));
            if (args.Arguments.Count >= 2)
            {
                switch(mode)
                {
                    case "new":
                        if (!string.IsNullOrEmpty(creationType))
                        {
                            switch (creationType)
                            {
                                case "string":
                                    _resourceManager.AddResource("/custom", new StringResource(name, value));
                                    currentEnvironment.WriteLine($"Variable {name} added.");
                                    return new CommandResponse((int)CommandStatus.Success);
                                case "fileByte":
                                    ReturnData result = client.ClientCallback.RequestInformation(new FileDialogRequestBuilder()
                                    {
                                        Title = "Select file to upload as resource."
                                    });
                                    if(result.AcquisitionState == RequestState.OK)
                                    {
                                        client.ClientCallback.RequestInformation(new SendLocalFileByteStreamRequestBuilder(name, result.Data.ToString()) { PathToSave = "/custom"});
                                        currentEnvironment.WriteLine("Resource created.");
                                        return new CommandResponse((int)CommandStatus.Success);
                                    }
                                    else
                                    {
                                        currentEnvironment.WriteLine("Action canceled.");
                                        return new CommandResponse((int)CommandStatus.Success);
                                    }
                                case "filePointer":
                                    ReturnData result2 = client.ClientCallback.RequestInformation(new SelectFileRequestBuilder());
                                    if (result2.AcquisitionState == RequestState.OK)
                                    {
                                        _resourceManager.AddResource<FilePointerResource>("/custom", new FilePointerResource(result2.Data.ToString(), name));
                                        currentEnvironment.WriteLine("Resource created.");
                                        return new CommandResponse((int)CommandStatus.Success);
                                    }
                                    else
                                    {
                                        currentEnvironment.WriteLine("Action canceled.");
                                        return new CommandResponse((int)CommandStatus.Success);
                                    }
                                default:
                                    currentEnvironment.WriteLine(new ConsoleText("Invalid resource type expected." ) { TextColor = Color.Red });
                                    return new CommandResponse((int)CommandStatus.Fail);
                            }
                        }
                        else
                        {
                            currentEnvironment.WriteLine(new ConsoleText("You must provide a reosurce creation type.") { TextColor = Color.Red });
                            return new CommandResponse((int)CommandStatus.Fail);
                        }
                    case "view":
                        //Will run into issues when we have lots of resources to enumerate.
                        var padWidth = _resourceManager.GetAllResources().Select(r => r.Path).Max(c => c.Length) + 5;
                        var typePadWidth = _resourceManager.GetAllResources().Select(r => r.ResourceType).Max(c => c.Length) + 5;
                        currentEnvironment.WriteLine();
                        foreach (Resource r in _resourceManager.GetAllResources())
                        {
                            string paddedString = r.Path.PadRight(padWidth);
                            if (r is IODevice)
                            {
                                currentEnvironment.WriteLine(new ConsoleText($"{r.ResourceType.PadRight(typePadWidth)}: {paddedString}{r.ToString()}") { TextColor = Color.Yellow });
                            }
                            else
                            {
                                currentEnvironment.WriteLine($"{r.ResourceType.PadRight(typePadWidth)}: {paddedString}{r.ToString()}");
                            }
                        }
                        currentEnvironment.WriteLine();
                        return new CommandResponse((int)CommandStatus.Success);
                    case "save":
                        _resourceManager.Save();
                        currentEnvironment.WriteLine("Resource saved");
                        return new CommandResponse((int)CommandStatus.Success);
                    case "delete":
                        _resourceManager.RemoveResource(args.Arguments[2].ToString());
                        currentEnvironment.WriteLine("Resource removed.");
                        return new CommandResponse((int)CommandStatus.Success);
                    case "modify":
                        if (args.Arguments.Count >= 3)
                        {
                            switch (args.Arguments[2].ToString())
                            {
                                case "name":
                                    if (args.Arguments[3].IsOfType<ResourceQuery>())
                                    {
                                        var resource = _resourceManager.GetResource<Resource>((ResourceQuery)args.Arguments[3]);
                                        resource.ResourceIdentifier = args.Arguments[4].ToString();
                                        currentEnvironment.WriteLine("Resource name modified.");
                                        return new CommandResponse((int)CommandStatus.Success);
                                    }
                                    else
                                    {
                                        currentEnvironment.WriteLine(new ConsoleText("Please pass in resource query.") { TextColor = Color.Red });
                                        return new CommandResponse((int)CommandStatus.Fail);
                                    }
                                default:
                                    currentEnvironment.WriteLine(new ConsoleText("Invalid resource value expected.") { TextColor = Color.Red });
                                    return new CommandResponse((int)CommandStatus.Fail);
                            }
                        }
                        else
                        {
                            currentEnvironment.WriteLine(new ConsoleText("You must provide the value to edit.") { TextColor = Color.Red });
                            return new CommandResponse((int)CommandStatus.Fail);
                        }
                    case "help":
                    default:
                        set.WriteOptionDescriptions(currentEnvironment.Out);
                        currentEnvironment.WriteLine();
                        return new CommandResponse((int)CommandStatus.Fail);
                }
            }
            else
            {
                currentEnvironment.WriteLine(new ConsoleText("Please provide an action for this command.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Gets the date and time set on the remote server.")]
        public CommandResponse dateTime(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            currentEnvironment.WriteLine(DateTime.Now.ToString());
            return new CommandResponse((int)CommandStatus.Success);
        }

        [CommandHelp("Returns the server version.")]
        public CommandResponse version(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            currentEnvironment.WriteLine(ServerManager.DefaultSettings.ServerVersion);
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Executes the EncryptFile service method.")]
        public CommandResponse svm_encyptFile(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            try
            {
                _service.RemoteInterface.EncryptFile(currentEnvironment.ClientContext, args.Arguments[1].ToString(), args.Arguments[2].ToString());
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (IndexOutOfRangeException)
            {
                currentEnvironment.WriteLine(new ConsoleText("You need to provide all the information.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Executes the DecryptFile service method.")]
        public CommandResponse svm_decryptFile(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            try
            {
                _service.RemoteInterface.DecryptFile(currentEnvironment.ClientContext, args.Arguments[1].ToString(), args.Arguments[2].ToString());
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (IndexOutOfRangeException)
            {
                currentEnvironment.WriteLine(new ConsoleText("You need to provide all the information.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Wraps around the beep function.")]
        [HelpPage("beep.txt", Source = HelpSourceType.File)]
        public CommandResponse svm_beep(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            _service.RemoteInterface.Beep(int.Parse(args.Arguments[1].ToString()), int.Parse(args.Arguments[2].ToString()));
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Wraps around the speak function.")]
        public CommandResponse svm_speak(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            VoiceAge age = VoiceAge.Adult;
            VoiceGender gender = VoiceGender.Male;
            string message = "";
            if (args.Arguments[1].ToString() == "vg_male")
            {
                gender = VoiceGender.Male;
            }
            else if (args.Arguments[1].ToString() == "vg_female")
            {
                gender = VoiceGender.Female;
            }
            else if (args.Arguments[1].ToString() == "vg_neutral")
            {
                gender = VoiceGender.Neutral;
            }
            else if (args.Arguments[1].ToString() == "vg_notSet")
            {
                gender = VoiceGender.NotSet;
            }
            else
            {
                currentEnvironment.WriteLine(new ConsoleText("You must provide a valid voice gender.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            if (args.Arguments[2].ToString() == "va_adult")
            {
                age = VoiceAge.Adult;
            }
            else if (args.Arguments[2].ToString() == "va_child")
            {
                age = VoiceAge.Child;
            }
            else if (args.Arguments[2].ToString() == "va_senior")
            {
                age = VoiceAge.Senior;
            }
            else if (args.Arguments[2].ToString() == "va_teen")
            {
                age = VoiceAge.Teen;
            }
            else if (args.Arguments[2].ToString() == "va_notSet")
            {
                age = VoiceAge.NotSet;
            }
            else
            {
                currentEnvironment.WriteLine(new ConsoleText("You must provide a valid voice age..") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            message = args.Arguments[3].ToString();
            _service.RemoteInterface.Speak(currentEnvironment.ClientContext, message, gender, age);
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Manages and displays all devices in RemotePlus")]
        public CommandResponse devCtl(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            string mode = string.Empty;
            string deviceString = string.Empty;
            string propString = string.Empty;
            string value = string.Empty;
            OptionSet set = new OptionSet()
                .Add("view", "View the properties of a device.", v => mode = "view")
                .Add("edit", "Edit a property of a device.", v => mode = "edit")
                .Add("device|d=", "The path to a device.", v => deviceString = v)
                .Add("property|p=", "The property to edit.", v => propString = v)
                .Add("value|v=", "The new value of a property.", v => value = v)
                .Add("help|?", "Shows the help screen.", v => mode = "help");
            set.Parse(args.Arguments.Select(e => e.ToString()));
            if(args.Arguments.Count >= 2)
            {
                switch(mode)
                {
                    case "view":
                        if (!string.IsNullOrEmpty(deviceString))
                        {
                            IODevice device = _resourceManager.GetResource<IODevice>(new ResourceQuery(deviceString, Guid.Empty));
                            var padWidth = device.DeviceProperties.Select(r => r.Value.Name).Max(c => c.Length) + 5;
                            currentEnvironment.WriteLine($"{device.ToString()}");
                            currentEnvironment.WriteLine(new string('=', device.ToString().Length));
                            foreach (DeviceProperty prop in device.DeviceProperties.Values)
                            {
                                currentEnvironment.WriteLine($"{prop.Name.PadRight(padWidth)}: {prop.Value}");
                            }
                            currentEnvironment.WriteLine();
                        }
                        else
                        {
                            currentEnvironment.WriteLine(new ConsoleText("No device has been specified.") { TextColor = Color.Red });
                        }
                        break;
                    case "edit":
                        if(!string.IsNullOrEmpty(propString))
                        {
                            if(!string.IsNullOrEmpty(value))
                            {
                                IODevice device = _resourceManager.GetResource<IODevice>(new ResourceQuery(deviceString, Guid.Empty));
                                bool success = device.DeviceProperties[propString].SetValue(value);
                                if (!success) currentEnvironment.WriteLine(new ConsoleText("Property failed to change.") { TextColor = Color.Red });
                            }
                            else
                            {
                                currentEnvironment.WriteLine(new ConsoleText("You must specify value to change.") { TextColor = Color.Red });
                            }
                        }
                        else
                        {
                            currentEnvironment.WriteLine(new ConsoleText("You must specify property to change.") { TextColor = Color.Red });
                        }
                        break;
                    case "help":
                        set.WriteOptionDescriptions(currentEnvironment.Out);
                        currentEnvironment.WriteLine();
                        break;
                }
            }
            else
            {
                currentEnvironment.WriteLine(new ConsoleText("No switch has been specified.") { TextColor = Color.Red });
            }
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Wraps around the showMessageBox function.")]
        [HelpPage("showMessageBox.txt", Source = HelpSourceType.File)]
        public CommandResponse svm_showMessageBox(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            bool showHelp = false;
            string tButtons = string.Empty;
            string tIcon = string.Empty;
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.None;
            string message = "";
            string caption = "";
            OptionSet set = new OptionSet()
                .Add("buttonType|b=", "The button to show to the message box.", v => tButtons = v)
                .Add("icon|i=", "The icon to show to the message box.", v => tIcon = v)
                .Add("caption|c=", "The caption to show to the message box.", v => caption = v)
                .Add("message|m=", "The message to show to the message box.", v => message = v)
                .Add("help|?", "Displays the help screen.", v => showHelp = true);
            set.Parse(args.Arguments.Select(a => a.ToString()));
            if (showHelp)
            {
                set.WriteOptionDescriptions(currentEnvironment.Out);
                return new CommandResponse((int)CommandStatus.Success);
            }
            if (tButtons == "b_OK")
            {
                buttons = MessageBoxButtons.OK;
            }
            else if (tButtons == "b_OK_CANCEL")
            {
                buttons = MessageBoxButtons.OKCancel;
            }
            else if (tButtons == "b_ABORT_RETRY_IGNORE")
            {
                buttons = MessageBoxButtons.AbortRetryIgnore;
            }
            else if (tButtons == "b_RETRY_CANCEL")
            {
                buttons = MessageBoxButtons.RetryCancel;
            }
            else if (tButtons == "b_YES_NO")
            {
                buttons = MessageBoxButtons.YesNo;
            }
            else if (tButtons == "b_YES_NO_CANCEL")
            {
                buttons = MessageBoxButtons.YesNoCancel;
            }
            else
            {
                currentEnvironment.WriteLine(new ConsoleText("Please provide a valid MessageBox button.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail) { ReturnData = "Invalid messageBox button." };
            }
            if (tIcon == "i_WARNING")
            {
                icon = MessageBoxIcon.Warning;
            }
            else if (tIcon == "i_STOP")
            {
                icon = MessageBoxIcon.Stop;
            }
            else if (tIcon == "i_ERROR")
            {
                icon = MessageBoxIcon.Error;
            }
            else if (tIcon == "i_HAND")
            {
                icon = MessageBoxIcon.Hand;
            }
            else if (tIcon == "i_INFORMATION")
            {
                icon = MessageBoxIcon.Information;
            }
            else if (tIcon == "i_QUESTION")
            {
                icon = MessageBoxIcon.Question;
            }
            else if (tIcon == "i_EXCLAMATION")
            {
                icon = MessageBoxIcon.Exclamation;
            }
            else if (tIcon == "i_ASTERISK")
            {
                icon = MessageBoxIcon.Asterisk;
            }
            else
            {
                currentEnvironment.WriteLine(new ConsoleText("Please provide a valid MessageBox icon type.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail) { ReturnData = "Invalid MessageBox icon" };
            }
            var dr = _service.RemoteInterface.ShowMessageBox(currentEnvironment.ClientContext, message, caption, icon, buttons);
            CommandResponse response = new CommandResponse((int)CommandStatus.Success);
            response.Metadata.Add("Buttons", buttons.ToString());
            response.Metadata.Add("Icon", icon.ToString());
            response.Metadata.Add("Caption", caption);
            response.Metadata.Add("Message", message);
            response.Metadata.Add("Response", dr.ToString());
            response.ReturnData = dr.ToString();
            return response;
        }
        [CommandHelp("Displays the path of the current server folder.")]
        public CommandResponse path(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            currentEnvironment.WriteLine($"The path to the server is {Environment.CurrentDirectory}");
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Changes the current working directory.")]
        public CommandResponse cd(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            var client = currentEnvironment.ClientContext.GetClient<RemoteClient>();
            if (args.Arguments.Count < 2)
            {
                currentEnvironment.WriteLine(new ConsoleText("You must specify a path to change to.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            else
            {
                Environment.CurrentDirectory = args.Arguments[1].ToString();
                client.ClientCallback.ChangePrompt(new RemotePlusLibrary.SubSystem.Command.PromptBuilder()
                {
                    CurrentUser = currentEnvironment.ClientContext.Username,
                    Path = Environment.CurrentDirectory,
                    AdditionalData = "Current Path"
                });
                return new CommandResponse((int)CommandStatus.Success);
            }
        }
        
        [CommandHelp("Loads an extension library dll into the system.")]
        public CommandResponse loadExtensionLibrary(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            var client = currentEnvironment.ClientContext.GetClient<RemoteClient>();
            var clientLogger = new ClientLogger(client);
            try
            {
                string path = string.Empty;
                if (Path.IsPathRooted(args.Arguments[1].ToString()))
                {
                    path = args.Arguments[1].ToString();
                }
                else
                {
                    path = Path.Combine(Environment.CurrentDirectory, args.Arguments[1].ToString());
                }
                GlobalServices.Logger.AddLogger(clientLogger);
                ServerManager.DefaultExtensionLibraryLoader.LoadFromAssembly(Assembly.LoadFrom(path));
                GlobalServices.Logger.RemoveLogger(clientLogger);
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (Exception ex)
            {
                GlobalServices.Logger.RemoveLogger(clientLogger);
                GlobalServices.Logger.Log($"Unable to load extension library: {ex.Message}", LogLevel.Error);
                currentEnvironment.WriteLine(new ConsoleText($"Unable to load extension library: {ex.Message}") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Loads an extension library from an external url.")]
        public CommandResponse loadExtensionLibraryRemote(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            var client = currentEnvironment.ClientContext.GetClient<RemoteClient>();
            if (args.Arguments.Count < 2)
            {
                currentEnvironment.WriteLine(new ConsoleText("You must provide a url.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            else
            {
                try
                {
                    WebClient wClient = new WebClient();
                    var extensionData = wClient.DownloadData(args.Arguments[1].ToString());
                    var clientLogger = new ClientLogger(client);
                    ServerManager.DefaultExtensionLibraryLoader.LoadFromAssembly(Assembly.Load(extensionData));
                    wClient.Dispose();
                    return new CommandResponse((int)CommandStatus.Success);
                }
                catch
                {
                    throw;
                }
            }
        }
        [CommandHelp("Copies a specified file to a new file.")]
        public CommandResponse cp(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            try
            {
                File.Copy(args.Arguments[1].ToString(), args.Arguments[2].ToString());
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch
            {
                return new CommandResponse((int)CommandStatus.Fail);
                throw;
            }
        }
        [CommandHelp("Deletes the specified file. This cannot be reverted.")]
        public CommandResponse deleteFile(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            var client = currentEnvironment.ClientContext.GetClient<RemoteClient>();
            bool autoAccept = args.Arguments.HasFlag("--acceptDisclamer");
            if (File.Exists(args.Arguments[1].ToString()))
            {
                if (!autoAccept)
                {
                    var rb = new MessageBoxRequestBuilder()
                    {
                        Message = "You are about to delete a file. THIS OPERATION IS PERMANENT!!",
                        Caption = "WARNING",
                        Buttons = MessageBoxButtons.YesNo,
                        Icons = MessageBoxIcon.Warning
                    };
                    DialogResult result = (DialogResult)Enum.Parse(typeof(DialogResult), (string)client.ClientCallback.RequestInformation(rb).Data);
                    if (result == DialogResult.Yes)
                    {
                        File.Delete(args.Arguments[1].ToString());
                        return new CommandResponse((int)CommandStatus.Success);
                    }
                    else
                    {
                        currentEnvironment.WriteLine("Operation canceled.");
                        return new CommandResponse((int)CommandStatus.Fail);
                    }
                }
                else
                {
                    File.Delete(args.Arguments[1].ToString());
                    return new CommandResponse((int)CommandStatus.Success);
                }
            }
            else
            {
                currentEnvironment.WriteLine(new ConsoleText("File does not exist.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Success);
            }
        }
        [CommandHelp("Reads the specified file and prints the contents to the screen.")]
        public CommandResponse echoFile(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            if (File.Exists(args.Arguments[1].ToString()))
            {
                currentEnvironment.WriteLine(File.ReadAllText(args.Arguments[1].ToString()));
                return new CommandResponse((int)CommandStatus.Success);
            }
            else
            {
                currentEnvironment.WriteLine(new ConsoleText("The file does not exist.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Tests the progress bar.")]
        public CommandResponse pg(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            var client = currentEnvironment.ClientContext.GetClient<RemoteClient>();
            client.ClientCallback.RequestInformation(new ProgressRequestBuilder()
            {
                Message = "Testing"
            });
            Thread.Sleep(3000);
            for(int i = 0; i <= 100; i++)
            {
                client.ClientCallback.UpdateRequest(new ProgressUpdateBuilder(i));
                Thread.Sleep(200);
            }
            client.ClientCallback.DisposeCurrentRequest();
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Lists all the files and directories in the current directory.")]
        public CommandResponse ls(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string file in Directory.GetFiles(Environment.CurrentDirectory))
            {
                currentEnvironment.WriteLine(new ConsoleText(Path.GetFileName(file) + "\t") { TextColor = Color.LightGray });
            }
            foreach (string directory in Directory.GetDirectories(Environment.CurrentDirectory))
            {
                currentEnvironment.WriteLine(new ConsoleText(directory + "\t") { TextColor = Color.Purple });
            }
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Clears all variables and functions from the interactive scripts.")]
        public CommandResponse resetStaticScript(CommandRequest reqest, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            _scriptingEngine.ResetSessionContext();
            return new CommandResponse((int)CommandStatus.Success);
        }

        #endregion Commands

        public override void InitializeServices(IServiceCollection services)
        {
            _commandSubsystem = services.GetService<ICommandSubsystem<IServerCommandModule>>();
            _resourceManager = services.GetService<IResourceManager>();
            _scriptingEngine = services.GetService<IScriptingEngine>();
            _service = services.GetService<IRemotePlusService<ServerRemoteInterface>>();
            Commands.Add("progTest", progTest);
            Commands.Add("readLineTest", readLineTest);
            Commands.Add("help", Help);
            Commands.Add("logs", Logs);
            Commands.Add("resex", resex);
            Commands.Add("dateTime", dateTime);
            Commands.Add("version", version);
            Commands.Add("encrypt", svm_encyptFile);
            Commands.Add("decrypt", svm_decryptFile);
            Commands.Add("beep", svm_beep);
            Commands.Add("speak", svm_speak);
            Commands.Add("devCtl", devCtl);
            Commands.Add("showMessageBox", svm_showMessageBox);
            Commands.Add("path", path);
            Commands.Add("cd", cd);
            Commands.Add("load-extensionLibrary", loadExtensionLibrary);
            Commands.Add("cp", cp);
            Commands.Add("deleteFile", deleteFile);
            Commands.Add("echoFile", echoFile);
            Commands.Add("ls", ls);
            Commands.Add("resetStaticScript", resetStaticScript);
            Commands.Add("load-extensionLibrary-remote", loadExtensionLibraryRemote);
            Commands.Add("pg", pg);
            Commands.Add("shutdown", shutdown);
        }
    }
}