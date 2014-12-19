using System;
using PropOhGate.Data;
using PropOhGate.Message;

namespace PropOhGate.Receive
{
    public class ReceiveListener
    {
        private readonly Repository _repository;
        private readonly IReceiver _receiver;
        private IDisposable _receiverSubscription;

        public ReceiveListener(Repository repository, IReceiver receiver)
        {
            _repository = repository;
            _receiver = receiver;
        }

        public void Start()
        {
            _repository.Clear();
            _receiverSubscription = _receiver.SubscribeToUpdate(Update);
            _receiver.Start();
        }

        public void Stop()
        {
            _receiver.Stop();
            _receiverSubscription.Dispose();
        }

        private void Update(byte[] data)
        {
            if (data.IsUpdate())
            {
                _repository.Update(data);
            }
            else if (data.IsReset())
            {
                _repository.Clear();
            }
        }
    }
}
