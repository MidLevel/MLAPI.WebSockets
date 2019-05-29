#if JSLIB
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
#endif

namespace MLAPI.WebSockets
{
    public static class WebSocketClientFactory
    {
#if JSLIB
        private static readonly byte[] _buffer = new byte[4096];
        private static readonly Dictionary<int, JsWebSocketClient> sockets = new Dictionary<int, JsWebSocketClient>();

        internal delegate void OnOpenCallback(int instanceId);
        internal delegate void OnMessageCallback(int instanceId, IntPtr messagePointer, int messageSize);
        internal delegate void OnErrorCallback(int instanceId, IntPtr errorPointer);
        internal delegate void OnCloseCallback(int instanceId, int closeCode);

        [DllImport("__Internal")]
        internal static extern int CreateWebSocketInstance(string url);
        [DllImport("__Internal")]
        internal static extern void DestroyWebSocketInstance(int instanceId);
        [DllImport("__Internal")]
        internal static extern void SetOnOpenDelegate(OnOpenCallback callback);
        [DllImport("__Internal")]
        internal static extern void SetOnMessageDelegate(OnMessageCallback callback);
        [DllImport("__Internal")]
        internal static extern void SetOnErrorDelegate(OnErrorCallback callback);
        [DllImport("__Internal")]
        internal static extern void SetOnCloseDelegate(OnCloseCallback callback);

        internal static void HandleInstanceDestroy(int instanceId)
        {
            sockets.Remove(instanceId);
            DestroyWebSocketInstance(instanceId);
        }

        [MonoPInvokeCallback(typeof(OnOpenCallback))]
        internal static void DelegateOnOpenEvent(int instanceId)
        {
            if (sockets.ContainsKey(instanceId))
            {
                sockets[instanceId].OnOpen();
            }
        }

        [MonoPInvokeCallback(typeof(OnMessageCallback))]
        internal static void DelegateOnMessageEvent(int instanceId, IntPtr payloadPointer, int size)
        {
            if (sockets.ContainsKey(instanceId))
            {
                Marshal.Copy(payloadPointer, _buffer, 0, size);
                sockets[instanceId].OnPayload(new ArraySegment<byte>(_buffer, 0, size));
            }
        }

        [MonoPInvokeCallback(typeof(OnErrorCallback))]
        internal static void DelegateOnErrorEvent(int instanceId, IntPtr errorPointer)
        {
            if (sockets.ContainsKey(instanceId))
            {
                string errorMessage = Marshal.PtrToStringAuto(errorPointer);
                sockets[instanceId].OnError(errorMessage);
            }
        }

        [MonoPInvokeCallback(typeof(OnCloseCallback))]
        internal static void DelegateOnCloseEvent(int instanceId, int disconnectCode)
        {
            if (sockets.ContainsKey(instanceId))
            {
                DisconnectCode code = (DisconnectCode)disconnectCode;

                if (!Enum.IsDefined(typeof(DisconnectCode), code))
                {
                    code = DisconnectCode.Unknown;
                }

                sockets[instanceId].OnClose(code);
            }
        }
#endif
        public static IWebSocketClient Create(string url)
        {
#if JSLIB
            // Set the js delegates
            SetOnOpenDelegate(DelegateOnOpenEvent);
            SetOnMessageDelegate(DelegateOnMessageEvent);
            SetOnErrorDelegate(DelegateOnErrorEvent);
            SetOnCloseDelegate(DelegateOnCloseEvent);

            // Create js websocket instance
            int instanceId = CreateWebSocketInstance(url);

            // Construct a wrapper for the JSLIB
            JsWebSocketClient webSocket = new JsWebSocketClient(instanceId);
            sockets.Add(instanceId, webSocket);

            return webSocket;
#else
            // Return a native websocket wrapper
            return new NativeWebSocketClient(url);
#endif
        }
    }
}
