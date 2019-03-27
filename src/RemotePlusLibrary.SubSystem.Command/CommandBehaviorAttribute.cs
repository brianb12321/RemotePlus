using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command
{
    /// <summary>
    /// Provides additional settings for a RemotePlus Command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [DataContract]
    public class CommandBehaviorAttribute : Attribute
    {
        /// <summary>
        /// Determines whether a script or a client should call this command.
        /// </summary>
        [DataMember]
        public CommandExecutionMode ExecutionType { get; set; } = CommandExecutionMode.Client;
        /// <summary>
        /// Determines whether for this command to show up in any help screen.
        /// </summary>
        [DataMember]
        public bool IndexCommandInHelp { get; set; } = true;
        /// <summary>
        /// Determines whether the main exception handler should catch any exceptions that occur during execution.
        /// </summary>
        [DataMember]
        public bool DoNotCatchExceptions { get; set; }
        /// <summary>
        /// Determines whether a message should be displayed to the end user.
        /// </summary>
        [DataMember]
        public StatusCodeDeliveryMethod StatusCodeDeliveryMethod { get; set; } = StatusCodeDeliveryMethod.DoNotDeliver;
        /// <summary>
        /// Determines what type of client can call this command. This property only applies for commands running on a server.
        /// </summary>
        [DataMember]
        public ClientType SupportClients { get; set; } = ClientType.CommandLine | ClientType.GUI;
        /// <summary>
        /// Specifies what should be shown to the user if their client type is not supported.
        /// </summary>
        [DataMember]
        public string ClientRejectionMessage { get; set; }
        /// <summary>
        /// Specifies the state of this command. Set this if you are developing a command or a command becomes obsolete.
        /// </summary>
        [DataMember]
        public ExtensionDevelopmentState CommandDevelepmentState { get; set; } = ExtensionDevelopmentState.Official;
        /// <summary>
        /// Requires the caller to call this command first in a command chain.
        /// </summary>
        [DataMember]
        public bool TopChainCommand { get; set; }
    }
}