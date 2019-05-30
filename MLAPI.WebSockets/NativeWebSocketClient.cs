#if !JSLIB
using System;

namespace MLAPI.WebSockets
{
    internal class NativeWebSocketClient : IWebSocketClient
    {
        internal event OnClientOpenDelegate OnOpenEvent;
        internal event OnClientPayloadDelegate OnPayloadEvent;
        internal event OnClientErrorDelegate OnErrorEvent;
        internal event OnClientCloseDelegate OnCloseEvent;

        private WebSocketSharp.WebSocket websocket = null;

        private readonly byte[] _buffer = new byte[4096];

        internal NativeWebSocketClient(string url)
        {
            try
            {
                websocket = new WebSocketSharp.WebSocket(url);

                websocket.OnOpen += (sender, ev) =>
                {
                    if (OnOpenEvent != null)
                    {
                        OnOpenEvent();
                    }
                };

                websocket.OnMessage += (sender, ev) =>
                {
                    if (ev.RawData != null && OnPayloadEvent != null)
                    {
                        OnPayloadEvent(new ArraySegment<byte>(ev.RawData, 0, ev.RawData.Length));
                    }
                };

                // Bind OnError event
                websocket.OnError += (sender, ev) =>
                {
                    if (OnErrorEvent != null)
                    {
                        OnErrorEvent(ev.Message);
                    }
                };

                // Bind OnClose event
                this.websocket.OnClose += (sender, ev) =>
                {
                    if (OnCloseEvent != null)
                    {
                        DisconnectCode code = (DisconnectCode)(int)ev.Code;

                        if (!Enum.IsDefined(typeof(DisconnectCode), code))
                        {
                            code = DisconnectCode.Unknown;
                        }

                        OnCloseEvent(code);
                    }
                };

            }

            catch (Exception e)
            {
                throw new WebSocketException("Failed to create socket", e);
            }
        }

        public void Connect()
        {
            if (websocket.ReadyState == WebSocketSharp.WebSocketState.Open)
            {
                throw new InvalidOperationException("Socket is already open");
            }

            if (websocket.ReadyState == WebSocketSharp.WebSocketState.Closing)
            {
                throw new InvalidOperationException("Socket is closing");
            }

            try
            {
                websocket.ConnectAsync();
            }
            catch (Exception e)
            {
                throw new WebSocketException("Connection failed", e);
            }
        }

        public void Close(DisconnectCode code = DisconnectCode.Normal, string reason = null)
        {
            if (websocket.ReadyState == WebSocketSharp.WebSocketState.Closing)
            {
                throw new InvalidOperationException("Socket is already closing");
            }

            if (this.websocket.ReadyState == WebSocketSharp.WebSocketState.Closed)
            {
                throw new InvalidOperationException("Socket is already closed");
            }

            try
            {
                websocket.CloseAsync((ushort)code, reason);
            }
            catch (Exception e)
            {
                throw new WebSocketException("Could not close socket", e);
            }
        }

        public void Send(ArraySegment<byte> payload)
        {
            if (websocket.ReadyState != WebSocketSharp.WebSocketState.Open)
            {
                throw new WebSocketException("Socket is not open");
            }

            try
            {
                if (payload.Offset > 0 || payload.Count < payload.Array.Length)
                {
                    // STA Websockets cant take offsets nor buffer lenghts.
                    byte[] buf = new byte[payload.Count];
                    Buffer.BlockCopy(payload.Array, payload.Offset, buf, 0, payload.Count);

                    websocket.Send(buf);
                }
                else
                {
                    websocket.Send(payload.Array);
                }
            }
            catch (Exception e)
            {
                throw new WebSocketException("Unknown error while sending the message", e);
            }
        }

        public WebSocketState GetState()
        {
            switch (websocket.ReadyState)
            {
                case WebSocketSharp.WebSocketState.Connecting:
                    {
                        return WebSocketState.Connecting;
                    }

                case WebSocketSharp.WebSocketState.Open:
                    {
                        return WebSocketState.Open;
                    }

                case WebSocketSharp.WebSocketState.Closing:
                    {
                        return WebSocketState.Closing;
                    }

                case WebSocketSharp.WebSocketState.Closed:
                    {
                        return WebSocketState.Closed;
                    }
                default:
                    {
                        return WebSocketState.Closed;
                    }
            }
        }

        public void SetOnOpen(OnClientOpenDelegate action)
        {
            OnOpenEvent += action;
        }

        public void SetOnPayload(OnClientPayloadDelegate action)
        {
            OnPayloadEvent += action;
        }

        public void SetOnError(OnClientErrorDelegate action)
        {
            OnErrorEvent += action;
        }

        public void SetOnClose(OnClientCloseDelegate action)
        {
            OnCloseEvent += action;
        }
    }
}
#endif
