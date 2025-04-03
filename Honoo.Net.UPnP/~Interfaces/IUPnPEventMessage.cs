namespace Honoo.Net
{
    /// <summary>
    /// UPnP event LastChange message interface.
    /// </summary>
    public interface IUPnPEventMessage
    {
        /// <summary>
        /// Event LastChange xml string.
        /// </summary>
        string LastChangeXml { get; }

        /// <summary>
        /// Event subscription url.
        /// </summary>
        string SubscriptionUrl { get; }
    }
}