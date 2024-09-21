namespace Honoo.Net.UPnP
{
    /// <summary>
    /// UPnP device interfaces.
    /// </summary>
    public sealed class UPnPDeviceInterfaces
    {
        internal UPnPDeviceInterfaces(UPnPDevice device)
        {
            this.Interface = device;
            this.MediaRenderer1 = device;
        }

        /// <summary>
        /// Gets main interface.
        /// </summary>
        public IUPnPDevice Interface { get; }

        /// <summary>
        /// Gets MediaRenderer1 interface.
        /// </summary>
        public IUPnPMediaRenderer1Device MediaRenderer1 { get; }
    }
}