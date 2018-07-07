using NewRemotePlusClient.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewRemotePlusClient
{
    public class UIManager : IUIManager
    {
        public void Show<TWindow>() where TWindow : IWindow, new()
        {
            TWindow tw = new TWindow();
            tw.Show();
        }
    }
}