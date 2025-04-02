using System;
using System.Net;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP event updated callback.
    /// </summary>
    /// <param name="server">The sender who raised the event.</param>
    /// <param name="message">Event LastChange message.</param>
    /// <param name="userState">Pass user state.</param>
    public delegate void UPnPEventRaisedCallback(UPnPServer server, UPnPEventMessage message, object userState);

    /// <summary>
    /// Executes after client request failed.
    /// </summary>
    /// <param name="server">The sender who raised the event.</param>
    /// <param name="request">Client request.</param>
    /// <param name="exception">Exception.</param>
    public delegate void UPnPRequestFailedEventHandler(UPnPServer server, HttpListenerRequest request, Exception exception);
}