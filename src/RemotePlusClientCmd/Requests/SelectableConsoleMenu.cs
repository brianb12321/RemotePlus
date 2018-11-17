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
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;

namespace RemotePlusClientCmd.Requests
{
    public class SelectableConsoleMenu : StandordRequest<SMenuRequestBuilder>
    {
        public override bool ShowProperties => false;

        public override string FriendlyName => "Selectable Command Line Menu";

        public override string Description => "You can use arrows to select your option.";

        public override string URI => "rcmd_csmenu";

        public override NetworkSide SupportedSides => NetworkSide.Client;

        ConsoLovers.ConsoleToolkit.Menu.ColoredConsoleMenu cm;
        char selectedItem = '0';
        public override RawDataResponse RequestData(SMenuRequestBuilder builder, NetworkSide executingSide)
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
                useBorderaroundHeader = builder.DrawHeadingBorder;
                selectBackColor = Color.FromArgb(builder.SelectBackColor);
                consoleBackColor = Color.FromArgb(builder.ConsoleBackColor);
                headerForeground = Color.FromArgb(builder.HeaderForeground);
                menuItemForeground = Color.FromArgb(builder.MenuItemForeground);
                selectForeColor = Color.FromArgb(builder.SelectForeground);
                headerFillColor = Color.FromArgb(builder.HeaderBackgrondColor);
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
                foreach (KeyValuePair<string, string> optionsList in builder.MenuItems)
                {
                    var mi = new ConsoleMenuItem($"{optionsList.Key}) {optionsList.Value}", new Action<ConsoleMenuItem>(doneExecute));
                    cm.Add(mi);
                }
                cm.Show();
                Console.ResetColor();
                Console.Clear();
                return RawDataResponse.Success(selectedItem);
            }
            catch (Exception ex)
            {
                GlobalServices.Logger.Log($"Unable to open menu: {ex.ToString()}", LogLevel.Error);
                return RawDataResponse.Cancel();
            }
        }

        private void doneExecute(ConsoleMenuItem mi)
        {
            cm.Close();
            selectedItem = mi.Text.ToCharArray()[0];
        }

        public override void Update(string message)
        {
            throw new NotImplementedException();
        }
    }
}