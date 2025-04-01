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
        }

        /// <summary>
        /// Gets main interface.
        /// </summary>
        public IUPnPDevice MainInterface { get; }
    }
}