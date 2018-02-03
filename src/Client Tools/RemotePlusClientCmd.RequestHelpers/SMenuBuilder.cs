using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using System.Drawing;
using RemotePlusServer;

namespace RemotePlusClientCmd.RequestHelpers
{
    /// <summary>
    /// Provides a wrapper for the rcmd_smenu URI.
    /// </summary>
    public class SMenuBuilder : IURIWrapper<int>
    {
        /// <summary>
        /// The message that will be displayed on the menu.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// The background color of the header. The default color is Black. [Metadata: HeaderBackColor]
        /// </summary>
        public Color HeaderBackgrondColor { get; set; } = Color.Black;
        /// <summary>
        /// The background color of the selector. The default color is Brown. [Metadata: SelectBackColor]
        /// </summary>
        public Color SelectBackColor { get; set; } = Color.Brown;
        /// <summary>
        /// The background color of the client console. The default color is Black. [Metadata: ConsoleBackColor]
        /// </summary>
        public Color ConsoleBackColor { get; set; } = Color.Black;
        /// <summary>
        /// The foreground color of the header. The default color is White. [Metadata: HeaderForeground]
        /// </summary>
        public Color HeaderForeground { get; set; } = Color.White;
        /// <summary>
        /// The foreground color of each menu item. The default color is White. [Metadata: MenuItemForeground]
        /// </summary>
        public Color MenuItemForeground { get; set; } = Color.White;
        /// <summary>
        /// The foreground color of a menu item when selected. The default color is Black. [Metadata: SelectForeColor]
        /// </summary>
        public Color SelectForeground { get; set; } = Color.Black;
        /// <summary>
        /// Determines whether to draw a border around the heading. The default is False. [Metadata: BorderHeader]
        /// </summary>
        public bool DrawHeadingBorder { get; set; } = false;
        /// <summary>
        /// The options that are in the menu.
        /// </summary>
        public List<string> MenuOptions { get; set; }
        public SMenuBuilder(string message)
        {
            Message = message;
            MenuOptions = new List<string>();
        }
        /// <summary>
        /// Creates a new <see cref="RequestBuilder"/> with all the configured options.
        /// </summary>
        /// <returns>The built request</returns>
        public RequestBuilder Build()
        {
            Dictionary<string, string> nd = new Dictionary<string, string>();
            for (int i = 0; i < MenuOptions.Count; i++)
            {
                nd.Add(i.ToString(), MenuOptions[i]);
            }
            RequestBuilder rb = new RequestBuilder("rcmd_smenu", Message, nd);
            rb.Metadata.Add("HeaderBackColor", HeaderBackgrondColor.Name);
            rb.Metadata.Add("SelectBackColor", SelectBackColor.Name);
            rb.Metadata.Add("ConsoleBackColor", ConsoleBackColor.Name);
            rb.Metadata.Add("HeaderForeground", HeaderForeground.Name);
            rb.Metadata.Add("MenuItemForeground", MenuItemForeground.Name);
            rb.Metadata.Add("SelectForeColor", SelectForeground.Name);
            rb.Metadata.Add("BorderHeader", DrawHeadingBorder.ToString());
            return rb;
        }
        public int BuildAndSend()
        {
            return int.Parse(char.ToString((char)ServerManager.DefaultService.Remote.Client.ClientCallback.RequestInformation(Build()).Data));
        }
    }
}
