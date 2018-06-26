using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.Security.AccountSystem.Policies
{
    /// <summary>
    /// Restricts operation functions.
    /// </summary>
    [Serializable]
    public class OperationPolicies : SecurityPolicyFolder
    {
        [IgnoreDataMember]
        public bool EnableConsole { get => bool.Parse(Policies.First(p => p.ShortName == "EnableConsole").Values["Value"]); set => Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] = value.ToString(); }
        [IgnoreDataMember]
        public bool GetServerSettings { get => bool.Parse(Policies.First(p => p.ShortName == "GetServerSettings").Values["Value"]); set => Policies.First(p => p.ShortName == "GetServerSettings").Values["Value"] = value.ToString(); }
        public OperationPolicies()
        {
            base.Name = "Operations";
            Path = "Operations";
            var consolePolicy = new DefaultPolicy("EnableConsole");
            consolePolicy.PolicyEditorType = "BiValue";
            consolePolicy.Values.Add("Value", "True");
            consolePolicy.Path = $"/{Path}/{consolePolicy.ShortName}";
            consolePolicy.Description = "Determines whether to permit the user to use the console.\nSet this policy to false to prohibit users from running commands, but be careful: scripts relying on the console will fail.";
            consolePolicy.ShortDescription = "Determines whether to permit the user to use the console.";
            var serverSettingsPolicy = new DefaultPolicy("GetServerSettings");
            serverSettingsPolicy.PolicyEditorType = "BiValue";
            serverSettingsPolicy.ShortName = "GetServerSettings";
            serverSettingsPolicy.Path = $"/{Path}/{serverSettingsPolicy.ShortName}";
            serverSettingsPolicy.Description = "Determines whether to permit the user to retrieve the loaded settings on the server. You will not be able to view the loaded settings using the client, but you can change the settings on the server-side.";
            serverSettingsPolicy.ShortDescription = "Determines whether to permit the user to retrieve the loaded settings on the server."; ;
            serverSettingsPolicy.Values.Add("Value", "True");
            Policies.Add(consolePolicy);
            Policies.Add(serverSettingsPolicy);
        }
    }
}