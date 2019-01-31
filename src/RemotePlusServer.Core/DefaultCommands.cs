using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Diagnostics;
using RemotePlusServer.Core.ExtensionSystem;
using System.IO;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.Scripting.ScriptPackageEngine;
using System.Drawing;
using BetterLogger;
using RemotePlusLibrary.Core;
using System.Media;
using System.Net;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using System.Linq;
using Ninject;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using RemotePlusLibrary.RequestSystem.DefaultUpdateRequestBuilders;
using System.Threading;

namespace RemotePlusServer.Core
{
    public class DefaultCommands : StandordCommandClass
    {
        IRemotePlusService<ServerRemoteInterface> _service;
        ICommandClassStore _store;
        IResourceManager _resourceManager;
        public DefaultCommands(IResourceManager resourceManager, IRemotePlusService<ServerRemoteInterface> service, ICommandClassStore store)
        {
            _service = service;
            _store = store;
            _resourceManager = resourceManager;
        }

        #region Commands
        [CommandHelp("Displays a list of commands.")]
        public CommandResponse Help(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            string helpString = string.Empty;
            if(args.Arguments.Count == 2)
            {
                helpString = RemotePlusConsole.ShowHelpPage(_store.GetAllCommands(), args.Arguments[1].ToString());
            }
            else
            {
                helpString = RemotePlusConsole.ShowHelp(_store.GetAllCommands());
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
            currentEnvironment.WriteLine("Shutting down server.");
            Process.Start("cmd.exe", "/c \"shutdown -s -t 1\"");
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Manages resources on the server.")]
        [HelpPage("vars.txt", Source = HelpSourceType.File)]
        public CommandResponse resex(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            if (args.Arguments.Count >= 2)
            {
                switch(args.Arguments[1].ToString())
                {
                    case "new":
                        if (args.Arguments.Count >= 3)
                        {
                            switch (args.Arguments[2].ToString())
                            {
                                case "string":
                                    _resourceManager.AddResource("/custom", new StringResource(args.Arguments[3].ToString(), args.Arguments[4].ToString()));
                                    currentEnvironment.WriteLine($"Variable {args.Arguments[3]} added.");
                                    return new CommandResponse((int)CommandStatus.Success);
                                case "fileByte":
                                    ReturnData result = _service.RemoteInterface.Client.ClientCallback.RequestInformation(new FileDialogRequestBuilder()
                                    {
                                        Title = "Select file to upload as resource."
                                    });
                                    if(result.AcquisitionState == RequestState.OK)
                                    {
                                        _service.RemoteInterface.Client.ClientCallback.RequestInformation(new SendLocalFileByteStreamRequestBuilder(args.Arguments[3].ToString(), result.Data.ToString()));
                                        currentEnvironment.WriteLine("Resource created.");
                                        return new CommandResponse((int)CommandStatus.Success);
                                    }
                                    else
                                    {
                                        currentEnvironment.WriteLine("Action canceled.");
                                        return new CommandResponse((int)CommandStatus.Success);
                                    }
                                case "filePointer":
                                    ReturnData result2 = _service.RemoteInterface.Client.ClientCallback.RequestInformation(new SelectFileRequestBuilder());
                                    if (result2.AcquisitionState == RequestState.OK)
                                    {
                                        _resourceManager.AddResource<FilePointerResource>("/custom", new FilePointerResource(result2.Data.ToString(), args.Arguments[3].ToString()));
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
                            currentEnvironment.WriteLine(new ConsoleText("You must provide a reosurce name") { TextColor = Color.Red });
                            return new CommandResponse((int)CommandStatus.Fail);
                        }
                    case "view":
                        var padWidth = _resourceManager.GetAllResources().Select(r => r.Path).Max(c => c.Length) + 5;
                        var typePadWidth = _resourceManager.GetAllResources().Select(r => r.ResourceType).Max(c => c.Length) + 5;
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine();
                        foreach (Resource r in _resourceManager.GetAllResources())
                        {
                            string paddedString = r.Path.PadRight(padWidth);
                            sb.AppendLine($"{r.ResourceType.PadRight(typePadWidth)}: {paddedString}{r.ToString()}");
                        }
                        currentEnvironment.WriteLine(sb.ToString());
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
                                    if(args.Arguments[3].IsOfType<ResourceQuery>())
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
                    default:
                        currentEnvironment.WriteLine(new ConsoleText("Invalid action.") { TextColor = Color.Red });
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
        [CommandHelp("Gets the list of processes running on the remote server.")]
        public CommandResponse processes(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            foreach (var p in Process.GetProcesses())
            {
                try
                {
                    sb.AppendLine($"Name: {p.ProcessName}, ID: {p.Id}, Start Time: {p.StartTime.ToString()}");
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"This process can be accessed: {ex.Message}");
                }
            }
            currentEnvironment.WriteLine(sb.ToString());
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
                _service.RemoteInterface.EncryptFile(args.Arguments[1].ToString(), args.Arguments[2].ToString());
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
                _service.RemoteInterface.DecryptFile(args.Arguments[1].ToString(), args.Arguments[2].ToString());
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
            _service.RemoteInterface.Speak(message, gender, age);
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Wraps around the showMessageBox function.")]
        [HelpPage("showMessageBox.txt", Source = HelpSourceType.File)]
        public CommandResponse svm_showMessageBox(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.None;
            string message = "";
            string caption = "";
            if (args.Arguments[1].ToString() == "b_OK")
            {
                buttons = MessageBoxButtons.OK;
            }
            else if (args.Arguments[1].ToString() == "b_OK_CANCEL")
            {
                buttons = MessageBoxButtons.OKCancel;
            }
            else if (args.Arguments[1].ToString() == "b_ABORT_RETRY_IGNORE")
            {
                buttons = MessageBoxButtons.AbortRetryIgnore;
            }
            else if (args.Arguments[1].ToString() == "b_RETRY_CANCEL")
            {
                buttons = MessageBoxButtons.RetryCancel;
            }
            else if (args.Arguments[1].ToString() == "b_YES_NO")
            {
                buttons = MessageBoxButtons.YesNo;
            }
            else if (args.Arguments[1].ToString() == "b_YES_NO_CANCEL")
            {
                buttons = MessageBoxButtons.YesNoCancel;
            }
            else
            {
                currentEnvironment.WriteLine(new ConsoleText("Please provide a valid MessageBox button.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail) { ReturnData = "Invalid messageBox button." };
            }
            if (args.Arguments[2].ToString() == "i_WARNING")
            {
                icon = MessageBoxIcon.Warning;
            }
            else if (args.Arguments[2].ToString() == "i_STOP")
            {
                icon = MessageBoxIcon.Stop;
            }
            else if (args.Arguments[2].ToString() == "i_ERROR")
            {
                icon = MessageBoxIcon.Error;
            }
            else if (args.Arguments[2].ToString() == "i_HAND")
            {
                icon = MessageBoxIcon.Hand;
            }
            else if (args.Arguments[2].ToString() == "i_INFORMATION")
            {
                icon = MessageBoxIcon.Information;
            }
            else if (args.Arguments[2].ToString() == "i_QUESTION")
            {
                icon = MessageBoxIcon.Question;
            }
            else if (args.Arguments[2].ToString() == "i_EXCLAMATION")
            {
                icon = MessageBoxIcon.Exclamation;
            }
            else if (args.Arguments[2].ToString() == "i_ASTERISK")
            {
                icon = MessageBoxIcon.Asterisk;
            }
            else
            {
                currentEnvironment.WriteLine(new ConsoleText("Please provide a valid MessageBox icon type.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail) { ReturnData = "Invalid MessageBox icon" };
            }
            caption = args.Arguments[3].ToString();
            message = args.Arguments[4].ToString();
            var dr = _service.RemoteInterface.ShowMessageBox(message, caption, icon, buttons);
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
            if (args.Arguments.Count < 2)
            {
                currentEnvironment.WriteLine(new ConsoleText("You must specify a path to change to.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            else
            {
                Environment.CurrentDirectory = args.Arguments[1].ToString();
                _service.RemoteInterface.Client.ClientCallback.ChangePrompt(new RemotePlusLibrary.Extension.CommandSystem.PromptBuilder()
                {
                    Path = _service.RemoteInterface.CurrentPath,
                    AdditionalData = "Current Path"
                });
                return new CommandResponse((int)CommandStatus.Success);
            }
        }
        [CommandHelp("Prints the message to the screen.")]
        public CommandResponse echo(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            string stringToPrint = string.Empty;
            if (args.Arguments.Count == 1 && args.HasLastCommand)
            {
                stringToPrint = args.LastCommand.ToString() + Environment.NewLine;
                currentEnvironment.WriteLine(stringToPrint);
            }
            else
            {
                stringToPrint = args.Arguments[1].ToString() + Environment.NewLine;
                currentEnvironment.WriteLine(stringToPrint);
            }
            return new CommandResponse((int)CommandStatus.Success)
            {
                ReturnData = stringToPrint
            };
        }
        [CommandHelp("Loads an extension library dll into the system.")]
        public CommandResponse loadExtensionLibrary(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            var clientLogger = new ClientLogger(_service.RemoteInterface.Client);
            try
            {
                string path = string.Empty;
                if (Path.IsPathRooted(args.Arguments[1].ToString()))
                {
                    path = args.Arguments[1].ToString();
                }
                else
                {
                    path = Path.Combine(_service.RemoteInterface.CurrentPath, args.Arguments[1].ToString());
                }
                GlobalServices.Logger.AddLogger(clientLogger);
                ServerManager.DefaultCollection.LoadExtension(path, new ServerInitEnvironment(false));
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
            if (args.Arguments.Count < 2)
            {
                currentEnvironment.WriteLine(new ConsoleText("You must provide a url.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            else
            {
                try
                {
                    WebClient client = new WebClient();
                    var extensionData = client.DownloadData(args.Arguments[1].ToString());
                    var clientLogger = new ClientLogger(_service.RemoteInterface.Client);
                    GlobalServices.Logger.AddLogger(clientLogger);
                    ServerManager.DefaultCollection.LoadExtension(extensionData, new ServerInitEnvironment(false));
                    GlobalServices.Logger.RemoveLogger(clientLogger);
                    client.Dispose();
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
                    DialogResult result = (DialogResult)Enum.Parse(typeof(DialogResult), (string)_service.RemoteInterface.Client.ClientCallback.RequestInformation(rb).Data);
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
            _service.RemoteInterface.Client.ClientCallback.RequestInformation(new ProgressRequestBuilder()
            {
                Message = "Testing"
            });
            Thread.Sleep(3000);
            for(int i = 0; i <= 100; i++)
            {
                _service.RemoteInterface.Client.ClientCallback.UpdateRequest(new ProgressUpdateBuilder(i));
                Thread.Sleep(200);
            }
            _service.RemoteInterface.Client.ClientCallback.DisposeCurrentRequest();
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Lists all the files and directories in the current directory.")]
        public CommandResponse ls(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string file in Directory.GetFiles(_service.RemoteInterface.CurrentPath))
            {
                currentEnvironment.WriteLine(new ConsoleText(Path.GetFileName(file) + "\t") { TextColor = Color.LightGray });
            }
            foreach (string directory in Directory.GetDirectories(_service.RemoteInterface.CurrentPath))
            {
                currentEnvironment.WriteLine(new ConsoleText(directory + "\t") { TextColor = Color.Purple });
            }
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Generates a sample package manifest file")]
        public CommandResponse genMan(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            ScriptPackageManifest m = new ScriptPackageManifest();
            m.PackageName = "TestPackage";
            m.ScriptEntryPoint = "main.py";
            m.GenerateManifestToFile(args.Arguments[1].ToString());
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Opens a script package and executes the entry-point script.")]
        public CommandResponse scp(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            ScriptPackage package = ScriptPackage.Open(args.Arguments[1].ToString());
            try
            {
                package.ExecuteScript();
                package.PackageContents.Dispose();
                package = null;
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (Exception ex)
            {
                currentEnvironment.WriteLine(new ConsoleText($"Unable to execute script package: {ex.Message}") { TextColor = Color.Red });
                package = null;
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Clears all variables and functions from the interactive scripts.")]
        public CommandResponse resetStaticScript(CommandRequest reqest, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            ServerManager.ScriptBuilder.ClearStaticScope();
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Plays an audio file sent by the client.")]
        public CommandResponse playAudio(CommandRequest req, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            bool removeResource = true;
            ResourceQuery query = null;
            if(req.Arguments.Count >= 2 && req.Arguments[1].IsOfType<ResourceQuery>())
            {
                query = (ResourceQuery)req.Arguments[1].Value;
                removeResource = false;
            }
            else
            {
                var requestPathBuilder = new FileDialogRequestBuilder()
                {
                    Title = "Select audio file.",
                    Filter = "Wav File (*.wav)|*.wav"
                };
                var path = _service.RemoteInterface.Client.ClientCallback.RequestInformation(requestPathBuilder);
                if (path.AcquisitionState == RequestState.OK)
                {
                    _service.RemoteInterface.Client.ClientCallback.RequestInformation(new SendLocalFileByteStreamRequestBuilder(Path.GetFileName(path.Data.ToString()), path.Data.ToString()));
                    query = new ResourceQuery(Path.GetFileName(path.Data.ToString()), Guid.Empty);
                }
                else
                {
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
            currentEnvironment.WriteLine($"Going to play audio file.");
            var audio = _resourceManager.GetResource<StreamResource>(query);
            currentEnvironment.WriteLine($"Now playing audio file. Name {audio.FileName}");
            audio.Open();
            SoundPlayer sp = new SoundPlayer(audio.Data);
            sp.PlaySync();
            if(removeResource)
            {
                _resourceManager.RemoveResource(audio.ResourceIdentifier);
            }
            audio.Close();
            return new CommandResponse((int)CommandStatus.Success);
        }
#endregion Commands

        public override void AddCommands()
        {
            Commands.Add("help", Help);
            Commands.Add("logs", Logs);
            Commands.Add("resex", resex);
            Commands.Add("dateTime", dateTime);
            Commands.Add("processes", processes);
            Commands.Add("version", version);
            Commands.Add("encrypt", svm_encyptFile);
            Commands.Add("decrypt", svm_decryptFile);
            Commands.Add("beep", svm_beep);
            Commands.Add("speak", svm_speak);
            Commands.Add("showMessageBox", svm_showMessageBox);
            Commands.Add("path", path);
            Commands.Add("cd", cd);
            Commands.Add("echo", echo);
            Commands.Add("load-extensionLibrary", loadExtensionLibrary);
            Commands.Add("cp", cp);
            Commands.Add("deleteFile", deleteFile);
            Commands.Add("echoFile", echoFile);
            Commands.Add("ls", ls);
            Commands.Add("genMan", genMan);
            Commands.Add("scp", scp);
            Commands.Add("resetStaticScript", resetStaticScript);
            Commands.Add("playAudio", playAudio);
            Commands.Add("load-extensionLibrary-remote", loadExtensionLibraryRemote);
            Commands.Add("pg", pg);
            Commands.Add("shutdown", shutdown);
        }
    }
}