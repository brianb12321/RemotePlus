using RemotePlusLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace NewRemotePlusClient.Models
{
    /// <summary>
    /// Provides the necessary information to connect to a RemotePlus server.
    /// </summary>
    public class Connection : INotifyPropertyChanged
    {
        private string _address = "net.tcp://localhost:9000/Remote";
        private RegisterationObject _registerObject = new RegisterationObject();
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Address)));
            }
        }
        public RegisterationObject RegisterObject
        {
            get { return _registerObject; }
            set
            {
                _registerObject = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RegisterObject)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
