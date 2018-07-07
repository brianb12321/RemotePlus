using BetterLogger;
using GalaSoft.MvvmLight.Messaging;
using NewRemotePlusClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NewRemotePlusClient
{
    public class ServerConsoleLogger : ILogger
    {
        private RichTextBox _textBox = null;
        public ServerConsoleLogger(RichTextBox textBox)
        {
            _textBox = textBox;
        }
        public async void Log(string message, LogLevel level)
        {
            await Application.Current.Dispatcher.InvokeAsync(() => _textBox.AppendText(message + Environment.NewLine));
        }
    }
}