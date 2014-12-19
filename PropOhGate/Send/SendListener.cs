using System.Linq;
using System;
using System.Reactive.Subjects;
using PropOhGate.Data;
using PropOhGate.Message;

namespace PropOhGate.Send
{
    public abstract class SendListener
    {
        private readonly object _lockObject = new object();
        private readonly string _repositoryHash;
        private readonly Subject<byte[]> _cellUpdated = new Subject<byte[]>();

        protected SendListener(Repository repository)
        {
            Repository = repository;
            _repositoryHash = Repository.GetRepositoryHash();
            StartListening();
        }

        protected Repository Repository
        {
            get;
            private set;
        }

        protected IDisposable Subscribe(ISender sender)
        {
            lock (_lockObject)
            {
                if (sender.RepositoryHash != _repositoryHash)
                {
                    throw new InvalidOperationException("Invalid repository hash");
                }

                ProcessFirstUpdate(sender);
                return _cellUpdated.Subscribe(sender.Update);
            }
        }

        private void StartListening()
        {
            lock (_lockObject)
            {
                Repository.SubscribeToCellUpdated(CellUpdated);
                Repository.SubscribeToReset(Reset);
            }
        }

        private void ProcessFirstUpdate(ISender sender)
        {
            var data = Repository.GetAllData().Select(c => c.Data).ToList();
            sender.Update(data);
        }

        private void CellUpdated(RepositoryCell cell)
        {
            _cellUpdated.OnNext(cell.Data);
        }

        private void Reset()
        {
            _cellUpdated.OnNext(MessageType.Reset.TypeId);
        }
    }
}
