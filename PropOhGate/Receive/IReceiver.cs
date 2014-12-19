using System;

namespace PropOhGate.Receive
{
    public interface IReceiver
    {
        void Start();
        void Stop();
        IDisposable SubscribeToUpdate(Action<byte[]> cellUpdate);
    }
}
