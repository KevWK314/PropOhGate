using System.Configuration;
using HousingData.Core;
using PropOhGate.WebSockets.Send;

namespace HousingData.Server
{
    public class HousingService
    {
        private readonly HousingRepository _repository;
        private WebSocketSendListener _listener;

        private readonly string _uri;

        public HousingService(HousingRepository repository)
        {
            _repository = repository;
            _uri = ConfigurationManager.AppSettings["Uri"];
        }

        public void Start()
        {
            _listener = new WebSocketSendListener(_repository);
            _listener.Start(_uri);
        }

        public void Stop()
        {
            _listener.Stop();
        }
    }
}
