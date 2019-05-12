using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Discovery;
using RemotePlusLibrary;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Security.AccountSystem;
using System.Speech.Synthesis;
using System.Windows.Forms;
using RemotePlusLibrary.Core;
using System.Reflection;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.FileTransfer.BrowserClasses;
using RemotePlusLibrary.Core.Faults;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.Security.Authentication;
using BetterLogger;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.Extension.ResourceSystem;
using TinyMessenger;
using RemotePlusLibrary.Discovery.Events;
using System.Drawing;
using ProxyServer.Internal;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.SubSystem.Command;
using ProxyServer.ExtensionSystem;

namespace ProxyServer
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true,
        InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        MaxItemsInObjectGraph = int.MaxValue,
        UseSynchronizationContext = false)]
    public class ProxyServerRemoteImpl : IProxyServerRemote, IProxyRemote
    {
        private IServerListManager _list;
        public SessionClient<IRemoteWithProxy> SelectedClient = null;
        public Client<IRemoteClient> ProxyClient = null;

        IClientContext BuildContext()
        {
            return new ProxyClientContext(ProxyClient, SelectedClient, OperationContext.Current.RequestContext, OperationContext.Current.InstanceContext, string.Empty);
        }
        public ProxyServerRemoteImpl()
        {
            _list = IOCContainer.GetService<IServerListManager>();
        }
        
        public void Beep(int Hertz, int Duration)
        {
            SelectedClient.ClientCallback.Beep(Hertz, Duration);
        }

        public void DecryptFile(string fileName, string password)
        {
            SelectedClient.ClientCallback.DecryptFile(fileName, password);
        }

        public void Disconnect()
        {
            SelectedClient.ClientCallback.Disconnect();
            _list.RemoveServer(SelectedClient);
        }

        public void EncryptFile(string fileName, string password)
        {
            SelectedClient.ClientCallback.EncryptFile(fileName, password);
        }

        public object ExecuteScript(string script)
        {
            return SelectedClient.ClientCallback.ExecuteScript(script);
        }

        public string GetCommandHelpDescription(string command)
        {
            return SelectedClient.ClientCallback.GetCommandHelpDescription(command);
        }

        public string GetCommandHelpPage(string command)
        {
            return SelectedClient.ClientCallback.GetCommandHelpPage(command);
        }

        public IEnumerable<CommandDescription> GetCommands()
        {
            return SelectedClient.ClientCallback.GetCommands();
        }

        public IEnumerable<string> GetCommandsAsStrings()
        {
            return SelectedClient.ClientCallback.GetCommandsAsStrings();
        }

        public UserAccount GetLoggedInUser()
        {
            return SelectedClient.ClientCallback.GetLoggedInUser();
        }

        public IDirectory GetRemoteFiles(string path, bool useRequest)
        {
            try
            {
                return SelectedClient.ClientCallback.GetRemoteFiles(path, useRequest);
            }
            catch (FaultException<ServerFault> ex)
            {
                throw new FaultException<ProxyFault>(new ProxyFault(SelectedClient.UniqueID), ex.Message);
            }
        }

        public Guid[] GetServers()
        {
            return _list.GetAllServers();
        }

        public ServerSettings GetServerSettings()
        {
            return SelectedClient.ClientCallback.GetServerSettings();
        }

        public void PlaySound(string FileName)
        {
            SelectedClient.ClientCallback.PlaySound(FileName);
        }

        public void PlaySoundLoop(string FileName)
        {
            SelectedClient.ClientCallback.PlaySoundLoop(FileName);
        }

        public void PlaySoundSync(string FileName)
        {
            SelectedClient.ClientCallback.PlaySoundSync(FileName);
        }

        public void ProxyDisconnect()
        {
            GlobalServices.Logger.Log($"Client [{ProxyClient.UniqueID}] disconnected from proxy server. Proxy server notifying connected servers that the client has disconnected.", LogLevel.Info);
            foreach(Client<IRemoteWithProxy> client in _list)
            {
                client.ClientCallback.Disconnect();
                GlobalServices.Logger.Log($"Server [{client.UniqueID}] notified of client disconnection.", LogLevel.Info);
            }
        }

        public void ProxyRegister()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IRemoteClient>();
            ProxyClient = Client<IRemoteClient>.Build(callback.RegisterClient(), callback, OperationContext.Current.Channel);
            IOCContainer.GetService<IScriptingEngine>().AddContext(ScriptingEngineExtensions.SESSION_NAME);
            if(ProxyClient.ClientType == ClientType.CommandLine)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=================================================================================");
                sb.AppendLine($"                        Welcome to RemotePlus Proxy                             ");
                sb.AppendLine();
                sb.AppendLine($"Version: {Assembly.GetExecutingAssembly().GetName().Version}                    ");
                sb.AppendLine("YOU ARE LIABLE FOR ANY DAMAGES OR ILLEGAL ACTIVITIES USING THIS PROGRAM. MAKE    ");
                sb.AppendLine("SURE YOU ARE AUTHORIZED TO LOAD REMOTEPLUS ONTO ANY SYSTEM THAT ISN'T YOURS.     ");
                sb.AppendLine("WE AREN'T RESPONSIBLE FOR ANY DATA LOSS DUE TO A CUSTOM EXTENSION OR SCRIPT.     ");
                sb.AppendLine("If you found a bug in the RemotePlus code, please open an issue at               ");
                sb.AppendLine("http://github.com/brianb12321/RemotePlus");
                sb.AppendLine("=================================================================================");
                sb.AppendLine();
                sb.AppendLine($"There are {_list.Count} server(s) connected to the proxy server.");
                sb.AppendLine("To view all the servers connected, enter {proxyViewServers} into the console.");
                sb.AppendLine("To switch the selected server, enter {proxySwitchServer} into the console.");
                sb.AppendLine($"Proxy server GUID: {ProxyManager.ProxyGuid}");
                sb.AppendLine();
                sb.AppendLine();
                ProxyClient.ClientCallback.WriteToClientConsole(ProxyManager.ProxyGuid, sb.ToString());
                ProxyClient.ClientCallback.ChangePrompt(ProxyManager.ProxyGuid, new RemotePlusLibrary.SubSystem.Command.PromptBuilder()
                {
                    Path = "",
                    CurrentUser = "",
                    AdditionalData = "Proxy"
                });
                ProxyClient.Channel.Faulted += (sender, e) =>
                {
                    GlobalServices.Logger.Log($"Client [{ProxyClient.UniqueID}] is now in the faulted state.", LogLevel.Error);
                    ProxyClient = null;
                };
                GlobalServices.Logger.Log($"Client [{ProxyClient.UniqueID}] is now registered.", LogLevel.Info);
                ProxyClient.ClientCallback.RegistirationComplete(ProxyManager.ProxyGuid);
            }
        }

        public string ReadFileAsString(string fileName)
        {
            return SelectedClient.ClientCallback.ReadFileAsString(fileName);
        }

        public void Register()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IRemoteWithProxy>();
            SessionClient<IRemoteWithProxy> tempClient = SessionClient<IRemoteWithProxy>.BuildSessionClient(new ClientBuilder(ClientType.Server), callback);
            tempClient.SessionId = OperationContext.Current.Channel.SessionId;
            _list.AddServer(tempClient);
            tempClient.Channel = OperationContext.Current.Channel;
            if(SelectedClient == null)
            {
                SelectedClient = tempClient;
            }
            tempClient.Channel.Faulted += (sender, e) =>
            {
                var closedClient = _list.GetServerByChannel(tempClient.Channel);
                GlobalServices.Logger.Log($"Server [{closedClient.UniqueID}] closed without proper shutdown.", LogLevel.Info);
                _list.RemoveServer(closedClient);
                PublishEvent(new ServerDisconnectedEvent(closedClient.UniqueID, true, this));
                if (SelectedClient == closedClient)
                {
                    Task.Run(() =>
                    {
                        var rb = new MessageBoxRequestBuilder()
                        {
                            Message = $"Server [{SelectedClient.UniqueID}] has disconnected without proper shutdown. Please select another server to be the active server.",
                            Caption = "Proxy Server",
                            Buttons = MessageBoxButtons.OK,
                            Icons = MessageBoxIcon.Information
                        };
                        ProxyClient.ClientCallback.RequestInformation(ProxyManager.ProxyGuid, rb);
                        SelectedClient = null;
                    });
                }
            };
            tempClient.ClientCallback.ServerRegistered(tempClient.UniqueID);
            PublishEvent(new ServerAddedEvent(tempClient.UniqueID, this));
            GlobalServices.Logger.Log($"Server [{tempClient.UniqueID}] joined the proxy cluster.", LogLevel.Info);
        }

        public void Register(RegisterationObject Settings)
        {
            SelectedClient.ClientCallback.Register(Settings);
        }

        public void Restart()
        {
            SelectedClient.ClientCallback.Restart();
        }

        public void RunProgram(string Program, string Argument, bool shell, bool ignore)
        {
            SelectedClient.ClientCallback.RunProgram(Program, Argument, shell, ignore);
        }

        public CommandPipeline RunServerCommand(string Command, CommandExecutionMode commandMode)
        {
            return SelectedClient.ClientCallback.RunServerCommand(Command, commandMode);
        }

        public void SelectServer(int serverPosition)
        {
            try
            {
                SelectedClient = _list[serverPosition];
                if (ProxyClient.ClientType == ClientType.CommandLine)
                {
                    ProxyClient.ClientCallback.WriteToClientConsole(ProxyManager.ProxyGuid, $"Server {serverPosition} is now active.", LogLevel.Info, "Proxy Server");
                    ProxyClient.ClientCallback.ChangePrompt(ProxyManager.ProxyGuid, new RemotePlusLibrary.SubSystem.Command.PromptBuilder()
                    {
                        AdditionalData = $"Server {serverPosition}"
                    });
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ProxyClient.ClientCallback.WriteToClientConsole(ProxyManager.ProxyGuid, "The requested server is not connected.");
            }
        }

        public void SelectServer(Guid guid)
        {
            SelectedClient = _list.GetByGuid(guid);
            if (SelectedClient == null)
            {
                ProxyClient.ClientCallback.WriteToClientConsole(ProxyManager.ProxyGuid, "The requested server is not connected.");
            }
            else
            {
                if (ProxyClient.ClientType == ClientType.CommandLine)
                {
                    SelectedClient.ClientCallback.Register(new RegisterationObject());
                    ProxyClient.ClientCallback.WriteToClientConsole(ProxyManager.ProxyGuid, $"Server {guid} is now active.", LogLevel.Info, "Proxy Server");
                    ProxyClient.ClientCallback.ChangePrompt(ProxyManager.ProxyGuid, new RemotePlusLibrary.SubSystem.Command.PromptBuilder()
                    {
                        AdditionalData = $"Server {guid}"
                    });
                }
            }
        }

        public DialogResult ShowMessageBox(string Message, string Caption, MessageBoxIcon Icon, MessageBoxButtons Buttons)
        {
            return SelectedClient.ClientCallback.ShowMessageBox(Message, Caption, Icon, Buttons);
        }

        public void Speak(string Message, VoiceGender Gender, VoiceAge Age)
        {
            SelectedClient.ClientCallback.Speak(Message, Gender, Age);
        }

        public void SwitchUser()
        {
            SelectedClient.ClientCallback.SwitchUser();
        }

        public void Disconnect(Guid guid, string Reason)
        {
            ProxyClient.ClientCallback.Disconnect(SelectedClient.UniqueID, Reason);
        }
        public RemotePlusLibrary.SubSystem.Command.PromptBuilder GetCurrentPrompt()
        {
            return ProxyClient.ClientCallback.GetCurrentPrompt();
        }
        public ClientBuilder RegisterClient()
        {
            return ProxyClient.ClientCallback.RegisterClient();
        }

        public void RegistirationComplete(Guid guid)
        {
            ProxyClient.ClientCallback.RegistirationComplete(SelectedClient.UniqueID);
        }
        public UserCredentials RequestAuthentication(Guid guid, AuthenticationRequest Request)
        {
            return ProxyClient.ClientCallback.RequestAuthentication(SelectedClient.UniqueID, Request);
        }

        public ReturnData RequestInformation(Guid guid, RequestBuilder builder)
        {
            return ProxyClient.ClientCallback.RequestInformation(SelectedClient.UniqueID, builder);
        }
        public void TellMessage(Guid guid, string Message, LogLevel o)
        {
            ProxyClient.ClientCallback.TellMessage(SelectedClient.UniqueID, Message, o);
        }

        public void WriteToClientConsole(Guid guid, string Message)
        {
            ProxyClient.ClientCallback.WriteToClientConsole(SelectedClient.UniqueID, Message);
        }

        public void WriteToClientConsole(Guid guid, ConsoleText text)
        {
            ProxyClient.ClientCallback.WriteToClientConsole(SelectedClient.UniqueID, text);
        }

        public void SendSignal(SignalMessage signal)
        {
            ProxyClient.ClientCallback.SendSignal(signal);
        }

        public void ChangePrompt(Guid guid, RemotePlusLibrary.SubSystem.Command.PromptBuilder newPrompt)
        {
            ProxyClient.ClientCallback.ChangePrompt(SelectedClient.UniqueID, newPrompt);
        }

        public Guid GetSelectedServerGuid()
        {
            return SelectedClient.UniqueID;
        }
        #region Command Methods
        public CommandPipeline ExecuteProxyCommand(string command, CommandExecutionMode mode)
        {
            return IOCContainer.GetService<ICommandSubsystem<IProxyCommandModule>>().RunServerCommand(command, mode, BuildContext());
        }
        #endregion
        public void Leave(Guid serverGuid)
        {
            var foundServer = _list.GetByGuid(serverGuid);
            if(foundServer != null)
            {
                GlobalServices.Logger.Log($"Server [{foundServer.UniqueID}] disconnected gracefully.", LogLevel.Info);
                _list.RemoveServer(foundServer);
                //Notify client that the active server has disconnected gracefully.
                if(SelectedClient == foundServer)
                {
                    Task.Run(() =>
                    {
                        var rb = new MessageBoxRequestBuilder()
                        {
                            Message = $"Server [{SelectedClient.UniqueID}] has disconnected gracefully. Please select another server to be the active server.",
                            Caption = "Proxy Server",
                            Buttons = MessageBoxButtons.OK,
                            Icons = MessageBoxIcon.Information
                        };
                        ProxyClient.ClientCallback.RequestInformation(ProxyManager.ProxyGuid, rb);
                        SelectedClient = null;
                    });
                }
            }
            PublishEvent(new ServerDisconnectedEvent(foundServer.UniqueID, false, this));
        }

        public void WriteToClientConsole(Guid serverGuid, string Message, LogLevel level)
        {
            ProxyClient.ClientCallback.WriteToClientConsole(serverGuid, Message, level);
        }

        public void WriteToClientConsole(Guid serverGuid, string Message, LogLevel level, string from)
        {
            ProxyClient.ClientCallback.WriteToClientConsole(serverGuid, Message, level, from);
        }

        public void WriteToClientConsoleNoNewLine(Guid serverGuid, string Message)
        {
            ProxyClient.ClientCallback.WriteToClientConsoleNoNewLine(serverGuid, Message);
        }

        public void UploadBytesToResource(byte[] data, int length, string friendlyName, string name, string path)
        {
            SelectedClient.ClientCallback.UploadBytesToResource(data, length, friendlyName, name, path);
        }

        public Resource GetResource(string resourceIdentifier)
        {
            return ProxyManager.ResourceStore[resourceIdentifier];
        }

        public object ExecuteProxyScript(string script)
        {
            var engine = IOCContainer.GetService<IScriptingEngine>();
            engine.SetIn(new _ClientTextReader(ProxyManager.ProxyGuid));
            engine.SetOut(new _ClientTextWriter(ProxyManager.ProxyGuid));
            engine.SetError(new _ClientTextWriter(ProxyManager.ProxyGuid));
            return engine.ExecuteString<object>(script);
        }

        public void UpdateRequest(Guid serverGuid, UpdateRequestBuilder message)
        {
            ProxyClient.ClientCallback.UpdateRequest(serverGuid, message);
        }
        public void DisposeCurrentRequest(Guid serverGuid)
        {
            ProxyClient.ClientCallback.DisposeCurrentRequest(serverGuid);
        }

        public void PublishEvent(ITinyMessage message)
        {
            foreach(var clients in _list)
            {
                if (clients.ClientCallback.HasKnownType(message.GetType().Name))
                {
                    clients.ClientCallback.PublishEvent(message);
                }
            }
            try
            {
                bool? hasKnownType = ProxyClient?.ClientCallback?.HasKnownType(message.GetType().Name);
                if (hasKnownType.HasValue && hasKnownType.Value)
                {
                    ProxyClient?.ClientCallback?.PublishEvent(message);
                }
            }
            catch (ObjectDisposedException)
            {

            }
        }

        public bool HasKnownType(string name)
        {
            return SelectedClient.ClientCallback.HasKnownType(name);
        }

        public Task<CommandPipeline> ExecuteProxyCommandAsync(string command, CommandExecutionMode mode)
        {
            return IOCContainer.GetService<ICommandSubsystem<IProxyCommandModule>>().RunServerCommandAsync(command, mode, BuildContext());
        }

        public Task<CommandPipeline> RunServerCommandAsync(string command, CommandExecutionMode commandMode)
        {
            return SelectedClient.ClientCallback.RunServerCommandAsync(command, commandMode);
        }

        public void CancelProxyCommand()
        {
            IOCContainer.GetService<ICommandSubsystem<IProxyCommandModule>>().Cancel();
        }

        public void CancelServerCommand()
        {
            SelectedClient.ClientCallback.CancelServerCommand();
        }

        public void SetClientConsoleBackgroundColor(Guid serverGuid, Color bgColor)
        {
            ProxyClient.ClientCallback.SetClientConsoleBackgroundColor(serverGuid, bgColor);
        }

        public void SetClientConsoleForegroundColor(Guid serverGuid, Color fgColor)
        {
            ProxyClient.ClientCallback.SetClientConsoleForegroundColor(serverGuid, fgColor);
        }

        public void ResetClientConsoleColor(Guid serverGuid)
        {
            ProxyClient.ClientCallback.ResetClientConsoleColor(serverGuid);
        }

        public void ClearServerConsole(Guid serverGuid)
        {
            ProxyClient.ClientCallback.ClearClientConsole(serverGuid);
        }
    }
}