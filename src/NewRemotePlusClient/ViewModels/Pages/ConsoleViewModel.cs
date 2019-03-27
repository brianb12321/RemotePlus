using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterLogger;
using GalaSoft.MvvmLight.Messaging;
using NewRemotePlusClient;
using NewRemotePlusClient.Models;

namespace NewRemotePlusClient.ViewModels.Pages
{
    public class ConsoleViewModel : TabPage
    {
        public ConsoleViewModel()
        {
            Name = "Console";
        }
    }
}