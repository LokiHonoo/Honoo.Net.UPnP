namespace Honoo.Net
{
    /// <summary>
    /// UPnP device interfaces.
    /// </summary>
    public sealed class UPnPDeviceInterfaces
    {
        internal UPnPDeviceInterfaces(UPnPDevice device)
        {
            this.MediaRenderer1 = device;
        }

        /// <summary>
        /// Gets MediaRenderer1 interface.
        /// </summary>
        public IUPnPMediaRenderer1Device MediaRenderer1 { get; }
    }
}