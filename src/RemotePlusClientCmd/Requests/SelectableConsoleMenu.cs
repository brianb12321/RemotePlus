using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using ConsoLovers.ConsoleToolkit.Menu;
using System.Drawing;
using ConsoLovers.ConsoleToolkit.Console;
using RemotePlusLibrary.RequestSystem;

namespace RemotePlusClientCmd.Requests
{
    //Interface: rcmd_smenu
    public class SelectableConsoleMenu : IDataRequest
    {
        bool IDataRequest.ShowProperties => false;

        string IDataRequest.FriendlyName => "Selectable Command Line Menu";

        string IDataRequest.Description => "You can use arrows to select your option.";
        ConsoLovers.ConsoleToolkit.Menu.ColoredConsoleMenu cm;
        char selectedItem = '0';
        RawDataRequest IDataRequest.RequestData(RequestBuilder builder)
        {
           try
            {
                bool useBorderaroundHeader = false;
                Color selectBackColor = Color.Brown;
                Color consoleBackColor = Color.Black;
                Color headerForeground = Color.White;
                Color menuItemForeground = Color.White;
                Color selectForeColor = Color.Black;
                Color headerFillColor = consoleBackColor;
                if (builder.Metadata.ContainsKey("BorderHeader"))
                {
                    useBorderaroundHeader = (builder.Metadata["BorderHeader"].ToLower() == "true") ? true : false;
                }
                if (builder.Metadata.ContainsKey("HeaderBackColor"))
                {
                    headerFillColor = Color.FromName(builder.Metadata["HeaderBackColor"]);
                }
                if(builder.Metadata.ContainsKey("SelectBackColor"))
                {
                    selectBackColor = Color.FromName(builder.Metadata["SelectBackColor"]);
                }
                if (builder.Metadata.ContainsKey("ConsoleBackColor"))
                {
                    consoleBackColor = Color.FromName(builder.Metadata["ConsoleBackColor"]);
                    if (!builder.Metadata.ContainsKey("HeaderBackColor"))
                    {
                        headerFillColor = consoleBackColor;
                    }
                }
                if(builder.Metadata.ContainsKey("HeaderForeground"))
                {
                    headerForeground = Color.FromName(builder.Metadata["HeaderForeground"]);
                }
                if(builder.Metadata.ContainsKey("MenuItemForeground"))
                {
                    menuItemForeground = Color.FromName(builder.Metadata["MenuItemForeground"]);
                }
                if(builder.Metadata.ContainsKey("SelectForeColor"))
                {
                    selectForeColor = Color.FromName(builder.Metadata["SelectForeColor"]);
                }
                selectedItem = '0';
                cm = new ColoredConsoleMenu();
                cm.Theme = new MenuColorTheme()
                {
                    HeaderBackground = headerFillColor,
                    MenuItem = new ColorSet()
                    {
                        SelectedBackground = selectBackColor,
                        SelectedForeground = selectForeColor,
                        Background = consoleBackColor,
                        Foreground = menuItemForeground
                    },
                    Selector = new ColorSet()
                    {
                        SelectedBackground = selectBackColor,
                        SelectedForeground = selectForeColor,
                        Background = consoleBackColor
                    },
                    ConsoleBackground = consoleBackColor,
                    HeaderForeground = headerForeground
                };
                if (useBorderaroundHeader)
                {
                    cm.Header = "===========================\n" + builder.Message + "\n" + "===========================\n";
                }
                else
                {
                    cm.Header = builder.Message + "\n";
                }
                foreach (KeyValuePair<string, string> options in builder.Arguments)
                {
                    var mi = new ConsoleMenuItem($"{options.Key}) {options.Value}", new Action<ConsoleMenuItem>(doneExecute));
                    cm.Add(mi);
                }
                cm.Show();
                Console.ResetColor();
                Console.Clear();
                return RawDataRequest.Success(selectedItem);
            }
            catch (Exception ex)
            {
                ClientCmdManager.Logger.AddOutput($"Unable to open menu: {ex.ToString()}", Logging.OutputLevel.Error);
                return RawDataRequest.Cancel();
            }
        }

        private void doneExecute(ConsoleMenuItem mi)
        {
            cm.Close();
            selectedItem = mi.Text.ToCharArray()[0];
        }

        void IDataRequest.UpdateProperties()
        {
            throw new NotImplementedException();
        }

        public void Update(string message)
        {
            throw new NotImplementedException();
        }
    }
}