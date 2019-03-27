using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BetterLogger;
using GalaSoft.MvvmLight.Messaging;
using NewRemotePlusClient;
using NewRemotePlusClient.Models;
using RemotePlusLibrary.SubSystem.Command;

namespace NewRemotePlusClient.Views.Pages
{
    /// <summary>
    /// Interaction logic for ConsolePage.xaml
    /// </summary>
    public partial class ConsolePage : UserControl
    {
        ILogFactory _internalLogFactory { get; set; } = new BaseLogFactory();
        public ConsolePage()
        {
            InitializeComponent();
            Messenger.Default.Register<ConsoleText>(this, async (message) =>
            {
                await Application.Current.Dispatcher.InvokeAsync(() => console.AppendText(message.Text));
                await Application.Current.Dispatcher.InvokeAsync(() => console.ScrollToEnd());
            });
            Messenger.Default.Register<ConsoleLogMessage>(this, async message =>
            {
                _internalLogFactory.Log(message.Message, message.Level, message.From, message.Extra);
                await Application.Current.Dispatcher.InvokeAsync(() => console.ScrollToEnd());
            });
            _internalLogFactory.AddLogger(new ServerConsoleLogger(console));
        }

        private void input_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                IOCHelper.Client.RunServerCommand(input.Text, CommandExecutionMode.Client);
                input.Clear();
            }
        }
    }
}
