namespace Honoo.Net
{
    /// <summary>
    /// UPnP event LastChange message interface.
    /// </summary>
    public interface IUPnPEventMessage
    {
        /// <summary>
        /// Event subscription url.
        /// </summary>
        string EventSubscriptionUrl { get; }

        /// <summary>
        /// Event LastChange xml string.
        /// </summary>
        string EventXmlString { get; }
    }
}