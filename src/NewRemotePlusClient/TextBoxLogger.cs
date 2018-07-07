using BetterLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using NewRemotePlusClient.Models;
using GalaSoft.MvvmLight.Messaging;

namespace NewRemotePlusClient
{
    public class TextBoxLogger : ILogger
    {
        public void Log(string message, LogLevel level)
        {
            message += Environment.NewLine;
            LogMessage lm = new LogMessage();
            lm.Level = level;
            lm.Message = message;
            Messenger.Default.Send(lm);
        }
    }
}