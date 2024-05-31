namespace Honoo.Net.UPnP
{
    /// <summary>
    /// UPnP IGDv1 "urn:schemas-upnp-org:device:MediaRenderer:1" interface.
    /// </summary>
    public interface IUPnPMediaRendererDevice : IUPnPDevice
    {
        /// <summary>
        /// Gets the x dlna doc if this device provides dlna service, else return "null".
        /// </summary>
        string XDlnaDoc { get; }
    }
}