using GalaSoft.MvvmLight.Messaging;
using NewRemotePlusClient.Models;
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

namespace NewRemotePlusClient.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = IOCHelper.MainWindow;
            Messenger.Default.Register<LogMessage>(this, (message) =>
            {
                consoleTextBox.AppendText(message.Message);
            });
        }
    }
}