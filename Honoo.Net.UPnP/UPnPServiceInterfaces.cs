namespace Honoo.Net
{
    /// <summary>
    /// UPnP service interfaces.
    /// </summary>
    public sealed class UPnPServiceInterfaces
    {
        internal UPnPServiceInterfaces(UPnPService service)
        {
            this.MainInterface = service;
            this.AVTransport1 = service;
            this.WANIPConnection1 = service;
            this.WANIPConnection2 = service;
            this.WANPPPConnection1 = service;
        }

        /// <summary>
        /// Gets AVTransport1 interface.
        /// </summary>
        public IUPnPAVTransport1Service AVTransport1 { get; }

        /// <summary>
        /// Gets main interface.
        /// </summary>
        public IUPnPService MainInterface { get; }

        /// <summary>
        /// Gets WANIPConnection1 interface.
        /// </summary>
        public IUPnPWANIPConnection1Service WANIPConnection1 { get; }

        /// <summary>
        /// Gets WANIPConnection2 interface.
        /// </summary>
        public IUPnPWANIPConnection2Service WANIPConnection2 { get; }

        /// <summary>
        /// Gets WANPPPConnection1 interface.
        /// </summary>
        public IUPnPWANPPPConnection1Service WANPPPConnection1 { get; }
    }
}