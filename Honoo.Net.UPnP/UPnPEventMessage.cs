namespace Honoo.Net
{
    /// <summary>
    /// UPnP event LastChange message.
    /// </summary>
    public abstract class UPnPEventMessage : IUPnPEventMessage
    {
        #region Members

        private readonly UPnPEventMessageInterfaces _interfaces;
        private readonly string _lastChangeXml;
        private readonly string _subscriptionUrl;

        /// <summary>
        /// UPnP event LastChange message interfaces.
        /// </summary>
        public UPnPEventMessageInterfaces Interfaces => _interfaces;

        /// <inheritdoc/>
        public string LastChangeXml => _lastChangeXml;

        /// <inheritdoc/>
        public string SubscriptionUrl => _subscriptionUrl;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPEventMessage class.
        /// </summary>
        /// <param name="subscriptionUrl">Event subscription url.</param>
        /// <param name="lastChangeXml">Event LastChange xml string.</param>
        protected UPnPEventMessage(string subscriptionUrl, string lastChangeXml)
        {
            _subscriptionUrl = subscriptionUrl;
            _lastChangeXml = lastChangeXml;
            _interfaces = new UPnPEventMessageInterfaces(this);
        }
    }
}