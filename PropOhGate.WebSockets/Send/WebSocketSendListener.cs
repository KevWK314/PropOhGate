using System;
using System.Collections.Generic;
using PropOhGate.Data;
using PropOhGate.Message;
using PropOhGate.Send;
using Fleck;

namespace PropOhGate.WebSockets.Send
{
    public class WebSocketSendListener : SendListener
    {
        private WebSocketServer _server;
        private readonly Dictionary<Guid, WebSocketSender> _clients = new Dictionary<Guid, WebSocketSender>();

        public WebSocketSendListener(Repository repository)
            : base(repository)
        {
        }

        public void Start(string uri)
        {
            _server = new WebSocketServer(uri);
            _server.Start(socket =>
            {
                socket.OnOpen = () => OnOpen(socket);
                socket.OnClose = () => OnClose(socket);
                socket.OnBinary = data => OnBinary(socket, data);
                socket.OnError = ex => OnError(socket, ex);
            });
        }

        public void Stop()
        {
            if (_server != null)
            {
                _server.Dispose();
            }
        }

        private void OnOpen(IWebSocketConnection connection)
        {
            lock (_clients)
            {
                var sender = new WebSocketSender(connection, Repository.GetRepositoryHash());
                _clients[connection.ConnectionInfo.Id] = sender;
            }
        }

        private void OnBinary(IWebSocketConnection connection, byte[] data)
        {
            lock (_clients)
            {
                WebSocketSender sender;
                if (_clients.TryGetValue(connection.ConnectionInfo.Id, out sender))
                {
                    if (data.IsSubscribe())
                    {
                        DoSubscribe(sender);
                    }
                    else if (data.IsUnsubscribe())
                    {
                        DoUnsubscribe(connection);
                    }
                }
            }
        }

        private void OnClose(IWebSocketConnection connection)
        {
            lock (_clients)
            {
                DoUnsubscribe(connection);  
            }
        }

        private void OnError(IWebSocketConnection connection, Exception ex)
        {
            Console.WriteLine("There was an error: {0}", ex.Message);
            OnClose(connection);
        }

        private void DoSubscribe(WebSocketSender sender)
        {
            if (sender.Subscription == null)
            {
                sender.Subscription = Subscribe(sender);
            }
        }

        private void DoUnsubscribe(IWebSocketConnection connection)
        {
            lock (_clients)
            {
                WebSocketSender sender;
                if (_clients.TryGetValue(connection.ConnectionInfo.Id, out sender))
                {
                    if (sender.Subscription != null)
                    {
                        sender.Subscription.Dispose();
                    }
                    _clients.Remove(connection.ConnectionInfo.Id);
                    connection.Close();
                }
            }
        }
    }
}
