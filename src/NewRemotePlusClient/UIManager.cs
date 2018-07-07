using GalaSoft.MvvmLight.Messaging;
using NewRemotePlusClient.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NewRemotePlusClient
{
    public class UIManager : IUIManager
    {
        public void Show<TWindow>() where TWindow : IWindow, new()
        {
            TWindow tw = new TWindow();
            tw.ShowDialog();
        }

        public MessageResult ShowMessageBox(string message, string caption, MessageButtons buttons, MessageType type)
        {
            MessageBoxButton mb = MessageBoxButton.OK;
            MessageBoxImage mi = MessageBoxImage.Information;
            switch(buttons)
            {
                case MessageButtons.OK:
                    mb = MessageBoxButton.OK;
                    break;
                case MessageButtons.Yes_No:
                    mb = MessageBoxButton.YesNo;
                    break;
            }
            switch(type)
            {
                case MessageType.Error:
                    mi = MessageBoxImage.Error;
                    break;
                case MessageType.Information:
                    mi = MessageBoxImage.Information;
                    break;
                case MessageType.Warning:
                    mi = MessageBoxImage.Warning;
                    break;
            }
            var result = MessageBox.Show(message, caption, mb, mi);
            switch(result)
            {
                case MessageBoxResult.OK:
                    return MessageResult.OK;
                case MessageBoxResult.Yes:
                    return MessageResult.Yes;
                case MessageBoxResult.No:
                    return MessageResult.No;
                default:
                    return MessageResult.No;
            }
        }
    }
}