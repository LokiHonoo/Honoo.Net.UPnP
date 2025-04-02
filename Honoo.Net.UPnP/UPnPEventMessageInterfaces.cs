namespace Honoo.Net
{
    /// <summary>
    /// UPnP event LastChange message interfaces.
    /// </summary>
    public sealed class UPnPEventMessageInterfaces
    {
        internal UPnPEventMessageInterfaces(UPnPEventMessage eventMessage)
        {
            this.MainInterface = eventMessage;
            this.MediaRenderer = (UPnPMediaRendererEventMessage)eventMessage;
        }

        /// <summary>
        /// Gets main interface.
        /// </summary>
        public IUPnPEventMessage MainInterface { get; }

        /// <summary>
        /// Gets media renderer interface.
        /// </summary>
        public IUPnPMediaRendererEventMessage MediaRenderer { get; }
    }
}