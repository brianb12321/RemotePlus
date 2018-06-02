using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core
{
    public class STAOperationInvoker : IOperationInvoker
    {
        IOperationInvoker _innerInvoker;
        public STAOperationInvoker(IOperationInvoker invoker)
        {
            _innerInvoker = invoker;

        }
        public object[] AllocateInputs()
        {
            return _innerInvoker.AllocateInputs();
        }
        
        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            // Create a new, STA thread
            object[] staOutputs = null;
            object retval = null;
            Thread thread = new Thread(
                delegate () {
                    retval = _innerInvoker.Invoke(instance, inputs, out staOutputs);
                });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            outputs = staOutputs;
            return retval;
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            // We don’t handle async…
            throw new NotImplementedException();
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            // We don’t handle async…
            throw new NotImplementedException();
        }

        public bool IsSynchronous => true;
    }
}
