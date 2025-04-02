using System.Collections.Generic;

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
        ICollection<UPnPDevice> Devices { get; }

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
        ICollection<UPnPIcon> IconList { get; }

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
        /// Presentation URL.
        /// </summary>
        string PresentationUrl { get; }

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
        ICollection<UPnPService> Services { get; }

        /// <summary>
        /// Unique device name. Device's uuid.
        /// </summary>
        string UDN { get; }

        /// <summary>
        /// Universal product code.
        /// </summary>
        string UPC { get; }
    }
}