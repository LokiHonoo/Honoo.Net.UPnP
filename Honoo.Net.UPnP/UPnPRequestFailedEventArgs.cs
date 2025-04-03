using System;
using System.Net;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP RequestFailed EventArg.
    /// </summary>
    public sealed class UPnPRequestFailedEventArgs : EventArgs
    {
        internal UPnPRequestFailedEventArgs(UPnPRequestFailedStatus status, HttpListenerRequest request, Exception exception)
        {
            Status = status;
            Request = request;
            Exception = exception;
        }

        /// <summary>
        /// Gets exception.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets request maybe "null".
        /// </summary>
        public HttpListenerRequest Request { get; }

        /// <summary>
        /// Gets status.
        /// </summary>
        public UPnPRequestFailedStatus Status { get; }
    }
}