using System;
using System.Net;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP event updated callback.
    /// </summary>
    /// <param name="server">The sender who raised the event.</param>
    /// <param name="messages">Event updated response messages.</param>
    public delegate void UPnPEventRaisedCallback(UPnPServer server, UPnPEventMessage[] messages);

    /// <summary>
    /// Executes after client request failed.
    /// </summary>
    /// <param name="server">The sender who raised the event.</param>
    /// <param name="request">Dlna client request.</param>
    /// <param name="exception">Exception.</param>
    public delegate void UPnPRequestFailedEventHandler(UPnPServer server, HttpListenerRequest request, Exception exception);
}