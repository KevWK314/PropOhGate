using System;
using System.Reactive.Subjects;
using PropOhGate.Message;
using PropOhGate.Receive;
using WebSocket4Net;

namespace PropOhGate.WebSockets.Receive
{
    public class WebSocketReceiver : IReceiver
    {
        private readonly string _uri;
        private readonly Subject<byte[]> _onData = new Subject<byte[]>();
        private WebSocket _webSocket;

        public WebSocketReceiver(string uri, string repositoryHash)
        {
            _uri = uri;
        }

        public void Start()
        {
            _webSocket = new WebSocket(_uri);
            _webSocket.Opened += (s, a) => StartSubscribing();
            _webSocket.Error += (s, a) => LogMessage("Error - " + a.Exception.Message);
            _webSocket.Closed += (s, a) => LogMessage("Closed");
            _webSocket.DataReceived += (s, a) => DataReceived(a.Data);
            _webSocket.Open();
        }

        public void Stop()
        {
            _webSocket.Send(MessageType.Unsubscribe.TypeId, 0, MessageType.Unsubscribe.TypeId.Length);
            if (_webSocket.State != WebSocketState.Closing && _webSocket.State != WebSocketState.Closed)
            {
                _webSocket.Close();
            }
        }

        public IDisposable SubscribeToUpdate(Action<byte[]> cellUpdate)
        {
            return _onData.Subscribe(cellUpdate);
        }

        private void StartSubscribing()
        {
            LogMessage("Connected, start subscribing");
            _webSocket.Send(MessageType.Subscribe.TypeId, 0, MessageType.Subscribe.TypeId.Length);
        }

        private void LogMessage(string message)
        {
            Console.WriteLine("Client: {0}", message);
        }

        private void DataReceived(byte[] data)
        {
            _onData.OnNext(data);
        }
    }
}
