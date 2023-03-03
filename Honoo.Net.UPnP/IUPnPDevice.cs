namespace Honoo.Net
{
    /// <summary>
    /// UPnP device interface.
    /// </summary>
    public interface IUPnPDevice
    {
        /// <summary>
        /// Child devices.
        /// </summary>
        UPnPDevice[] Devices { get; }

        /// <summary>
        /// Device type.
        /// </summary>
        string DeviceType { get; }

        /// <summary>
        /// Friendly name.
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        /// Icons.
        /// </summary>
        UPnPIcon[] Icons { get; }

        /// <summary>
        /// Manufacturer.
        /// </summary>
        string Manufacturer { get; }

        /// <summary>
        /// Manufacturer url.
        /// </summary>
        string ManufacturerUrl { get; }

        /// <summary>
        /// Model description.
        /// </summary>
        string ModelDescription { get; }

        /// <summary>
        /// Model name.
        /// </summary>
        string ModelName { get; }

        /// <summary>
        /// Model number.
        /// </summary>
        string ModelNumber { get; }

        /// <summary>
        /// Model url.
        /// </summary>
        string ModelUrl { get; }

        /// <summary>
        /// Parent device.
        /// </summary>
        UPnPDevice ParentDevice { get; }

        /// <summary>
        /// Root device.
        /// </summary>
        UPnPRootDevice RootDevice { get; }

        /// <summary>
        /// Serial number.
        /// </summary>
        string SerialNumber { get; }

        /// <summary>
        /// Services.
        /// </summary>
        UPnPService[] Services { get; }

        /// <summary>
        /// Unique device name.
        /// </summary>
        string Udn { get; }

        /// <summary>
        /// Universal product code.
        /// </summary>
        string Upc { get; }
    }
}