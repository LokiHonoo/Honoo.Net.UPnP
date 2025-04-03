namespace Honoo.Net
{
    /// <summary>
    /// UPnP RequestFailed status.
    /// </summary>
    public enum UPnPRequestFailedStatus
    {
        /// <summary>
        /// None.
        /// </summary>
        None,

        /// <summary>
        /// Server listener broken.
        /// </summary>
        ListenerBroken,

        /// <summary>
        /// No one handle it.
        /// </summary>
        Unhandled,

        /// <summary>
        /// Event subscriber handled but Failed.
        /// </summary>
        EventAnalyzeFailed,

        /// <summary>
        /// Dlna media server handled but Failed.
        /// </summary>
        MediaTransportFailed,
    }
}