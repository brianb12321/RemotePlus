using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.FileTransfer.Service.PackageSystem
{
    public class StandordPackageInventory : IPackageInventory<FilePackage>
    {
        List<PackageListener<FilePackage>> _listeners = new List<PackageListener<FilePackage>>();
        private void notifyListeners(FilePackage p)
        {
            for(int i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].ListenerDelegate?.Invoke(p);
                if (!_listeners[i].KeepAlive)
                {
                    UnRegisterListener(_listeners[i]);
                }
            }
            if (!p.KeepAlive)
            {
                p.Dispose();
            }
        }
        public PackageListener<FilePackage> Receive(Action<FilePackage> listener, bool keepActive = false)
        {
            var newListener = new PackageListener<FilePackage>(listener);
            newListener.KeepAlive = keepActive;
            _listeners.Add(newListener);
            return newListener;
        }

        public void UnRegisterListener(PackageListener<FilePackage> listener)
        {
            _listeners.Remove(listener);
        }

        public void Dispatch(FilePackage p)
        {
            notifyListeners(p);
        }
    }
}