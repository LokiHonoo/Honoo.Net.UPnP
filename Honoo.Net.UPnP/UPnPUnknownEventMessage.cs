namespace Honoo.Net
{
    /// <summary>
    /// UPnP event LastChange message.
    /// </summary>
    public sealed class UPnPUnknownEventMessage : UPnPEventMessage
    {
        /// <summary>
        /// Initializes a new instance of the UPnPUnknownEventMessage class.
        /// </summary>
        /// <param name="eventSubscriberUrl">Event subscriber url.</param>
        /// <param name="eventXmlString">Event LastChange xml string.</param>
        internal UPnPUnknownEventMessage(string eventSubscriberUrl, string eventXmlString) : base(eventSubscriberUrl, eventXmlString)
        {
        }
    }
}