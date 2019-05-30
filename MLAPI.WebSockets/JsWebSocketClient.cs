#if JSLIB
using System;
using System.Runtime.InteropServices;

namespace MLAPI.WebSockets
{
    internal class JsWebSocketClient : IWebSocketClient
    {
        [DllImport("__Internal")]
        internal static extern int Connect(int instanceId);
        [DllImport("__Internal")]
        internal static extern int Close(int instanceId, int code, string reason);
        [DllImport("__Internal")]
        internal static extern int Send(int instanceId, byte[] data, int offset, int size);
        [DllImport("__Internal")]
        internal static extern int GetState(int instanceId);

        private readonly int instanceId;

        public OnClientOpenDelegate OnOpen => OnOpenEvent;
        public OnClientPayloadDelegate OnPayload => OnPayloadEvent;
        public OnClientErrorDelegate OnError => OnErrorEvent;
        public OnClientCloseDelegate OnClose => OnCloseEvent;

        internal event OnClientOpenDelegate OnOpenEvent;
        internal event OnClientPayloadDelegate OnPayloadEvent;
        internal event OnClientErrorDelegate OnErrorEvent;
        internal event OnClientCloseDelegate OnCloseEvent;

        internal JsWebSocketClient(int instanceId)
        {
            this.instanceId = instanceId;
        }

        ~JsWebSocketClient()
        {
            WebSocketClientFactory.HandleInstanceDestroy(instanceId);
        }

        public void Connect()
        {
            int response = Connect(this.instanceId);

            if (response < 0)
            {
                throw ErrorParser.GetWebSocketExceptionFromErrorCode(response, null);
            }
        }

        public void Close(DisconnectCode code = DisconnectCode.Normal, string reason = null)
        {
            int response = Close(instanceId, (int)code, reason);

            if (response < 0)
            {
                throw ErrorParser.GetWebSocketExceptionFromErrorCode(response, null);
            }
        }

        public WebSocketState GetState()
        {
            int state = GetState(instanceId);

            if (state < 0)
            {
                throw ErrorParser.GetWebSocketExceptionFromErrorCode(state, null);
            }

            switch (state)
            {
                case 0:
                    {
                        return WebSocketState.Connecting;
                    }

                case 1:
                    {
                        return WebSocketState.Open;
                    }

                case 2:
                    {
                        return WebSocketState.Closing;
                    }
                case 3:
                    {
                        return WebSocketState.Closed;
                    }

                default:
                    {
                        return WebSocketState.Closed;
                    }
            }
        }

        public void Send(ArraySegment<byte> payload)
        {
            int response = Send(instanceId, payload.Array, payload.Offset, payload.Count);

            if (response < 0)
            {
                throw ErrorParser.GetWebSocketExceptionFromErrorCode(response, null);
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