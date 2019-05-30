using System;

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
                        return new WebSocketException("Instance not found", innerException);
                    }
                case -2:
                    {
                        return new WebSocketException("Instance is already connected or connecting", innerException);
                    }
                case -3:
                    {
                        return new WebSocketException("Instance is not connected", innerException);
                    }
                case -4:
                    {
                        return new WebSocketException("Instance is already closing", innerException);
                    }
                case -5:
                    {
                        return new WebSocketException("Instance is already closed", innerException);
                    }
                case -6:
                    {
                        return new WebSocketException("Instance is not open", innerException);
                    }
                case -7:
                    {
                        return new WebSocketException("The close code or close reason was too long", innerException);
                    }
                default:
                    {
                        return new WebSocketException("Unknown error code", innerException);
                    }
            }
        }
    }

    public class WebSocketException : Exception
    {
        public WebSocketException()
        {
        }

        public WebSocketException(string message) : base(message)
        {
        }

        public WebSocketException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
