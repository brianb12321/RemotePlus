using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using RemotePlusServer;

namespace RemotePlusClientCmd.RequestHelpers
{
    /// <summary>
    /// Provides a wrapper for the rcmd_textBox URI.
    /// </summary>
    public class CmdTextBox : IURIWrapper<string>
    {
        /// <summary>
        /// The question that will be asked to the user.
        /// </summary>
        public string Prompt { get; set; }
        public CmdTextBox(string prompt)
        {
            Prompt = prompt;
        }

        public RequestBuilder Build()
        {
            return new RequestBuilder("rcmd_textBox", Prompt, null);
        }

        public string BuildAndSend()
        {
            return (string)ServerManager.DefaultService.Remote.Client.ClientCallback.RequestInformation(Build()).Data;
        }
    }
}
