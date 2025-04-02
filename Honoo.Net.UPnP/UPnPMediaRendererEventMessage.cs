using System.Collections.Generic;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP event LastChange message.
    /// </summary>
    public sealed class UPnPMediaRendererEventMessage : UPnPEventMessage, IUPnPMediaRendererEventMessage
    {
        #region Members

        private readonly Dictionary<uint, UPnPChangeInstance> _instances = new Dictionary<uint, UPnPChangeInstance>();

        /// <inheritdoc/>
        public Dictionary<uint, UPnPChangeInstance> Instances => _instances;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPMediaRendererEventMessage class.
        /// </summary>
        /// <param name="eventSubscriberUrl">Event subscriber url.</param>
        /// <param name="eventXmlString">Event LastChange xml string.</param>

        internal UPnPMediaRendererEventMessage(string eventSubscriberUrl, string eventXmlString) : base(eventSubscriberUrl, eventXmlString)
        {
        }
    }
}