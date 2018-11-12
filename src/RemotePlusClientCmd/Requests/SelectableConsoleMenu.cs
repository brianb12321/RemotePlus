using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using ConsoLovers.ConsoleToolkit.Menu;
using System.Drawing;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.RequestSystem;
using BetterLogger;
using RemotePlusLibrary.RequestSystem.DefaultRequestOptions;

namespace RemotePlusClientCmd.Requests
{
    //Interface: rcmd_smenu
    public class SelectableConsoleMenu : IDataRequest
    {
        bool IDataRequest.ShowProperties => false;

        string IDataRequest.FriendlyName => "Selectable Command Line Menu";

        string IDataRequest.Description => "You can use arrows to select your option.";

        string IDataRequest.URI => "rcmd_csmenu";

        ConsoLovers.ConsoleToolkit.Menu.ColoredConsoleMenu cm;
        char selectedItem = '0';
        RawDataRequest IDataRequest.RequestData(RequestBuilder builder)
        {
           try
            {
                var options = builder.UnsafeResolve<SMenuRequestOptions>();
                bool useBorderaroundHeader = false;
                Color selectBackColor = Color.Brown;
                Color consoleBackColor = Color.Black;
                Color headerForeground = Color.White;
                Color menuItemForeground = Color.White;
                Color selectForeColor = Color.Black;
                Color headerFillColor = consoleBackColor;
                useBorderaroundHeader = options.DrawHeadingBorder;
                selectBackColor = Color.FromArgb(options.SelectBackColor);
                consoleBackColor = Color.FromArgb(options.ConsoleBackColor);
                headerForeground = Color.FromArgb(options.HeaderForeground);
                menuItemForeground = Color.FromArgb(options.MenuItemForeground);
                selectForeColor = Color.FromArgb(options.SelectForeground);
                headerFillColor = Color.FromArgb(options.HeaderBackgrondColor);
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
                    cm.Header = "===========================\n" + options.Message + "\n" + "===========================\n";
                }
                else
                {
                    cm.Header = options.Message + "\n";
                }
                foreach (KeyValuePair<string, string> optionsList in options.MenuItems)
                {
                    var mi = new ConsoleMenuItem($"{optionsList.Key}) {optionsList.Value}", new Action<ConsoleMenuItem>(doneExecute));
                    cm.Add(mi);
                }
                cm.Show();
                Console.ResetColor();
                Console.Clear();
                return RawDataRequest.Success(selectedItem);
            }
            catch (Exception ex)
            {
                GlobalServices.Logger.Log($"Unable to open menu: {ex.ToString()}", LogLevel.Error);
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

        void IDataRequest.Update(string message)
        {
            throw new NotImplementedException();
        }
    }
}