using System;
using System.Net.WebSockets;

namespace MLAPI.WebSockets
{
    internal static class ErrorParser
    {
        internal static WebSocketException GetWebSocketExceptionFromErrorCode(int errorCode, Exception innerException)
        {
            switch (errorCode)
            {
                case -1:
                    {
                        return new WebSocketException(WebSocketError.InvalidState, "Instance not found", innerException);
                    }
                case -2:
                    {
                        return new WebSocketException(WebSocketError.InvalidState, "Instance is already connected or connecting", innerException);
                    }
                case -3:
                    {
                        return new WebSocketException(WebSocketError.InvalidState, "Instance is not connected", innerException);
                    }
                case -4:
                    {
                        return new WebSocketException(WebSocketError.InvalidState, "Instance is already closing", innerException);
                    }
                case -5:
                    {
                        return new WebSocketException(WebSocketError.InvalidState, "Instance is already closed", innerException);
                    }
                case -6:
                    {
                        return new WebSocketException(WebSocketError.InvalidState, "Instance is not open", innerException);
                    }
                case -7:
                    {
                        return new WebSocketException(WebSocketError.Faulted, "The close code or close reason was too long", innerException);
                    }
                default:
                    {
                        return new WebSocketException(WebSocketError.Faulted, "Unknown error code", innerException);
                    }
            }
        }
    }
}
