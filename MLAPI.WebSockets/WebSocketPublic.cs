using System;

namespace MLAPI.WebSockets
{
    public delegate void OnClientOpenDelegate();
    public delegate void OnClientPayloadDelegate(ArraySegment<byte> payload);
    public delegate void OnClientCloseDelegate(DisconnectCode closeCode);
    public delegate void OnClientErrorDelegate(string errorMsg);

    public interface IWebSocketClient
    {
        void Connect();
        void Close(DisconnectCode code = DisconnectCode.Normal, string reason = null);
        void Send(ArraySegment<byte> payload);
        WebSocketState GetState();

        void SetOnOpen(OnClientOpenDelegate action);
        void SetOnPayload(OnClientPayloadDelegate action);
        void SetOnError(OnClientErrorDelegate action);
        void SetOnClose(OnClientCloseDelegate action);
    }

#if !JSLIB
    public struct WebSocketServerEvent
    {
        public WebSocketServerEventType Type;
        public ulong Id;
        public byte[] Payload;
        public string Error;
        public string Reason;
    }

    public enum WebSocketServerEventType
    {
        Nothing,
        Open,
        Close,
        Payload,
        Error
    }
#endif
}
