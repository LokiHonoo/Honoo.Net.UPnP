namespace Honoo.Net
{
    /// <summary>
    /// UPnP event LastChange message.
    /// </summary>
    public abstract class UPnPEventMessage : IUPnPEventMessage
    {
        #region Members

        private readonly string _eventSubscriptionUrl;
        private readonly string _eventXmlString;
        private readonly UPnPEventMessageInterfaces _interfaces;

        /// <inheritdoc/>
        public string EventSubscriptionUrl => _eventSubscriptionUrl;

        /// <inheritdoc/>
        public string EventXmlString => _eventXmlString;

        /// <summary>
        /// UPnP event LastChange message interfaces.
        /// </summary>
        public UPnPEventMessageInterfaces Interfaces => _interfaces;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPEventMessage class.
        /// </summary>
        /// <param name="eventSubscriptionUrl">Event subscription url.</param>
        /// <param name="eventXmlString">Event LastChange xml string.</param>
        protected UPnPEventMessage(string eventSubscriptionUrl, string eventXmlString)
        {
            _eventSubscriptionUrl = eventSubscriptionUrl;
            _eventXmlString = eventXmlString;
            _interfaces = new UPnPEventMessageInterfaces(this);
        }
    }
}