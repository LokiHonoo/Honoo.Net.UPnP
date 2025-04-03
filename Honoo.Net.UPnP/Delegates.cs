namespace Honoo.Net
{
    /// <summary>
    /// UPnP event updated callback.
    /// </summary>
    /// <param name="sender">The sender who raised the event.</param>
    /// <param name="message">Event LastChange message.</param>
    /// <param name="userState">Pass user state.</param>
    public delegate void UPnPEventRaisedCallback(UPnPServer sender, UPnPEventMessage message, object userState);

    /// <summary>
    /// Executes after client request failed.
    /// </summary>
    /// <param name="sender">The sender who raised the event.</param>
    /// <param name="e">EventArgs.</param>
    public delegate void UPnPRequestFailedEventHandler(UPnPServer sender, UPnPRequestFailedEventArgs e);
}