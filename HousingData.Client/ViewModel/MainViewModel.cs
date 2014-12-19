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
        private bool _started;
        private string _uri;

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

        private void Start()
        {
            _started = true;
            StartCommand.RaiseCanExecuteChanged();

            var receiver = new WebSocketReceiver(_uri, _repository.GetRepositoryHash());
            var receiveListener = new ReceiveListener(_repository, receiver);
            receiveListener.Start();
        }
    }
}