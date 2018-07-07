using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewRemotePlusClient.IOC
{
    public interface IUIManager
    {
        void Show<TWindow>() where TWindow : IWindow, new();
        MessageResult ShowMessageBox(string message, string caption, MessageButtons buttons, MessageType type);
    }
}