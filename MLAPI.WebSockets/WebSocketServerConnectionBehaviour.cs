using System.Collections.Generic;
using System.Net;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace MLAPI.WebSockets
{
    public class WebSocketServerConnectionBehaviour : WebSocketBehavior
    {
        private static ulong connectionIdCounter;
        private static Queue<ulong> releasedConnectionIds = new Queue<ulong>();
        private static readonly object connectionLock = new object();
        private static Dictionary<ulong, WebSocket> connectedClients = new Dictionary<ulong, WebSocket>();
        private static Queue<WebSocketServerEvent> eventQueue = new Queue<WebSocketServerEvent>();

        private static ulong NextConnectionId()
        {
            if (releasedConnectionIds.Count == 0)
            {
                return connectionIdCounter++;
            }
            else
            {
                return releasedConnectionIds.Dequeue();
            }
        }

        private static void ReleaseConnectionId(ulong id)
        {
            releasedConnectionIds.Enqueue(id);
        }

        public IPEndPoint Endpoint { get; private set; }
        public WebSocket Socket { get; private set; }
        public ulong ConnectionId { get; private set; }

        protected override void OnOpen()
        {
            Endpoint = Context.UserEndPoint;
            Socket = Context.WebSocket;

            lock (connectionLock)
            {
                ConnectionId = NextConnectionId();

                eventQueue.Enqueue(new WebSocketServerEvent()
                {
                    Id = ConnectionId,
                    Payload = null,
                    Type = WebSocketServerEventType.Open,
                    Error = null,
                    Reason = null
                });
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            lock (connectionLock)
            {
                string reason = e.Reason;
                ushort code = e.Code;

                if (connectedClients.ContainsKey(ConnectionId))
                {
                    connectedClients.Remove(ConnectionId);
                    ReleaseConnectionId(ConnectionId);

                    eventQueue.Enqueue(new WebSocketServerEvent()
                    {
                        Id = ConnectionId,
                        Payload = null,
                        Type = WebSocketServerEventType.Close,
                        Error = null,
                        Reason = e.Reason
                    });
                }
            }
        }

        protected override void OnError(ErrorEventArgs e)
        {
            lock (connectionLock)
            {
                string error = e.Message;

                if (connectedClients.ContainsKey(ConnectionId))
                {
                    eventQueue.Enqueue(new WebSocketServerEvent()
                    {
                        Id = ConnectionId,
                        Payload = null,
                        Type = WebSocketServerEventType.Error,
                        Error = error,
                        Reason = null
                    });
                }
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            lock (connectionLock)
            {
                byte[] data = e.RawData;

                if (connectedClients.ContainsKey(ConnectionId))
                {
                    eventQueue.Enqueue(new WebSocketServerEvent()
                    {
                        Id = ConnectionId,
                        Payload = null,
                        Type = WebSocketServerEventType.Payload,
                        Error = null,
                        Reason = null
                    });
                }
            }
        }
    }
}
