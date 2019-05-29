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
        OnClientOpenDelegate OnOpen { get; }
        OnClientPayloadDelegate OnPayload { get; }
        OnClientErrorDelegate OnError { get; }
        OnClientCloseDelegate OnClose { get; }
    }

    public interface IWebSocketServer
    {
        void Close(ulong id, DisconnectCode code = DisconnectCode.Normal, string reason = null);
        void Send(ulong id, ArraySegment<byte> payload);
        void Shutdown();
        void Listen(ushort port);
        WebSocketState GetState(ulong id);
        WebSocketServerEvent Poll();
    }

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
        Open,
        Close,
        Payload,
        Error
    }
}
