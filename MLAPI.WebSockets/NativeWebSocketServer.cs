using System;

namespace MLAPI.WebSockets
{
    public class NativeWebSocketServer : IWebSocketServer
    {
        public static NativeWebSocketServer Server = new NativeWebSocketServer();

        public OnServerOpenDelegate OnOpen => OnOpenEvent;
        public OnServerCloseDelegate OnClose => OnCloseEvent;
        public OnServerPayloadDelegate OnPayload => OnPayloadEvent;
        public OnServerErrorDelegate OnError => OnErrorEvent;

        internal event OnServerOpenDelegate OnOpenEvent;
        internal event OnServerCloseDelegate OnCloseEvent;
        internal event OnServerPayloadDelegate OnPayloadEvent;
        internal event OnServerErrorDelegate OnErrorEvent;

        private NativeWebSocketServer()
        {

        }

        public void Close(ulong id, DisconnectCode code = DisconnectCode.Normal, string reason = null)
        {
            throw new NotImplementedException();
        }

        public WebSocketState GetState(ulong id)
        {
            throw new NotImplementedException();
        }

        public void Listen(ushort port)
        {
            throw new NotImplementedException();
        }

        public void Send(ulong id, ArraySegment<byte> payload)
        {
            throw new NotImplementedException();
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }
    }
}
