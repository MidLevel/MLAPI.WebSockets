using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using WebSocketSharp.Server;

namespace MLAPI.WebSockets
{
    public class NativeWebSocketServer
    {
        private static WebSocketServer webSocketServer;
        private static bool isStarted;
        public static NativeWebSocketServer Instance = new NativeWebSocketServer();


        private NativeWebSocketServer()
        {

        }

        public void Start(IPAddress address, int port, string connectionPath = "/mlapi-connection", X509Certificate2 certificate = null)
        {
            if (isStarted)
            {
                throw new InvalidOperationException("Socket already started");
            }

            isStarted = true;

            webSocketServer = new WebSocketServer(address, port, certificate != null);
            webSocketServer.SslConfiguration.ServerCertificate = certificate;
            webSocketServer.AddWebSocketService<WebSocketServerConnectionBehaviour>(connectionPath);

            webSocketServer.Start();
        }

        public void Close(ulong id, DisconnectCode code = DisconnectCode.Normal, string reason = null)
        {
            if (!isStarted)
            {
                throw new InvalidOperationException("Socket not started");
            }

            WebSocketServerConnectionBehaviour.Close(id, code, reason);
        }

        public WebSocketState GetState(ulong id)
        {
            if (!isStarted)
            {
                throw new InvalidOperationException("Socket not started");
            }

            return WebSocketServerConnectionBehaviour.GetState(id);
        }

        public void Send(ulong id, ArraySegment<byte> payload)
        {
            if (!isStarted)
            {
                throw new InvalidOperationException("Socket not started");
            }

            WebSocketServerConnectionBehaviour.Send(id, payload);
        }

        public void Shutdown()
        {
            if (!isStarted)
            {
                throw new InvalidOperationException("Socket not started");
            }

            isStarted = false;

            WebSocketServerConnectionBehaviour.Reset();

            webSocketServer.Stop();
        }

        public WebSocketServerEvent Poll()
        {
            if (!isStarted)
            {
                throw new InvalidOperationException("Socket not started");
            }

            return WebSocketServerConnectionBehaviour.Poll();
        }
    }
}
