using System;
using System.Collections.Generic;
using Fleck;
using PropOhGate.Send;

namespace PropOhGate.WebSockets.Send
{
    public class WebSocketSender : ISender
    {
        private readonly object _lockObject = new object();

        public WebSocketSender(IWebSocketConnection connection, string repositoryHash)
        {
            RepositoryHash = repositoryHash;
            Connection = connection;
        }

        public string RepositoryHash
        {
            get;
            private set;
        }

        public IWebSocketConnection Connection
        {
            get;
            private set;
        }

        public IDisposable Subscription
        {
            get;
            set;
        }

        public void Update(IList<byte[]> data)
        {
            lock (_lockObject)
            {
                foreach (var d in data)
                {
                    if (Connection.IsAvailable)
                    {
                        Connection.Send(d);
                    }
                }
            }
        }

        public void Update(byte[] data)
        {
            lock (_lockObject)
            {
                if (Connection.IsAvailable)
                {
                    Connection.Send(data);
                }
            }
        }
    }
}
