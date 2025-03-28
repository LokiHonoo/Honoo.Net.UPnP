namespace Honoo.Net
{
    /// <summary>
    /// UPnP device interfaces.
    /// </summary>
    public sealed class UPnPDeviceInterfaces
    {
        internal UPnPDeviceInterfaces(UPnPDevice device)
        {
            this.MainInterface = device;
            this.MediaRenderer1 = device;
        }

        /// <summary>
        /// Gets main interface.
        /// </summary>
        public IUPnPDevice MainInterface { get; }

        /// <summary>
        /// Gets MediaRenderer1 interface.
        /// </summary>
        public IUPnPMediaRenderer1Device MediaRenderer1 { get; }
    }
}