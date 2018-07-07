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
using RemotePlusLibrary.Extension.CommandSystem;

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
            Messenger.Default.Register<ConsoleText>(this, (message) =>
            {
                console.AppendText(message.Text);
            });
            Messenger.Default.Register<ConsoleLogMessage>(this, message =>
            {
                _internalLogFactory.Log(message.Message, message.Level, message.From, message.Extra);
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
