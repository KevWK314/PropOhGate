using System.Configuration;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using HousingData.Client.Model;
using HousingData.Core;
using PropOhGate.Collection;
using PropOhGate.Receive;
using PropOhGate.WebSockets.Receive;

namespace HousingData.Client.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly HousingRepository _repository;
        private WebSocketReceiver _receiver;
        private readonly string _uri;
        private bool _started;

        public MainViewModel()
        {
            _repository = new HousingRepository();
            _uri = ConfigurationManager.AppSettings["Uri"];

            Collection = new RepositoryObservableCollection<HousingItem>(_repository);
            StartCommand = new RelayCommand(Start, () => !_started);
        }

        public RepositoryObservableCollection<HousingItem> Collection
        {
            get;
            private set;
        }

        public RelayCommand StartCommand
        {
            get;
            private set;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            _receiver.Stop();
        }

        private void Start()
        {
            _started = true;
            StartCommand.RaiseCanExecuteChanged();

            _receiver = new WebSocketReceiver(_uri, _repository.GetRepositoryHash());
            var receiveListener = new ReceiveListener(_repository, _receiver);
            receiveListener.Start();
        }
    }
}