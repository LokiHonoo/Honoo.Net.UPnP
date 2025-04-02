using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP device.
    /// </summary>
    public sealed class UPnPDevice : IUPnPDevice
    {
        #region Members

        private readonly UPnPDevice[] _devices;
        private readonly string _deviceType;
        private readonly string _friendlyName;
        private readonly UPnPIcon[] _iconList;
        private readonly UPnPDeviceInterfaces _interfaces;
        private readonly string _manufacturer;
        private readonly string _manufacturerUrl;
        private readonly string _modelDescription;
        private readonly string _modelName;
        private readonly string _modelNumber;
        private readonly string _modelUrl;
        private readonly UPnPDevice _parentDevice;
        private readonly string _presentationUrl;
        private readonly UPnPRootDevice _rootDevice;
        private readonly string _serialNumber;
        private readonly UPnPService[] _services;
        private readonly string _udn;
        private readonly string _upc;

        /// <inheritdoc/>
        public ICollection<UPnPDevice> Devices => _devices;

        /// <inheritdoc/>
        public string DeviceType => _deviceType;

        /// <inheritdoc/>
        public string FriendlyName => _friendlyName;

        /// <inheritdoc/>
        public ICollection<UPnPIcon> IconList => _iconList;

        /// <summary>
        /// UPnP device interfaces.
        /// </summary>
        public UPnPDeviceInterfaces Interfaces => _interfaces;

        /// <inheritdoc/>
        public string Manufacturer => _manufacturer;

        /// <inheritdoc/>
        public string ManufacturerUrl => _manufacturerUrl;

        /// <inheritdoc/>
        public string ModelDescription => _modelDescription;

        /// <inheritdoc/>
        public string ModelName => _modelName;

        /// <inheritdoc/>
        public string ModelNumber => _modelNumber;

        /// <inheritdoc/>
        public string ModelUrl => _modelUrl;

        /// <inheritdoc/>
        public UPnPDevice ParentDevice => _parentDevice;

        /// <inheritdoc/>
        public string PresentationUrl => _presentationUrl;

        /// <inheritdoc/>
        public UPnPRootDevice RootDevice => _rootDevice;

        /// <inheritdoc/>
        public string SerialNumber => _serialNumber;

        /// <inheritdoc/>
        public ICollection<UPnPService> Services => _services;

        /// <inheritdoc/>
        public string UDN => _udn;

        /// <inheritdoc/>
        public string UPC => _upc;

        #endregion Members

        #region Construction

        /// <summary>
        /// Initializes a new instance of the UPnPDevice class.
        /// </summary>
        /// <param name="deviceElement">Device element.</param>
        /// <param name="parentDevice">The UPnPDevice to which the device belongs.</param>
        /// <param name="rootDevice">The UPnPRootDevice to which the root device belongs.</param>
        /// <exception cref="Exception"/>
        internal UPnPDevice(XElement deviceElement, UPnPDevice parentDevice, UPnPRootDevice rootDevice)
        {
            XNamespace nm = deviceElement.GetDefaultNamespace();
            _deviceType = deviceElement.Element(nm + "deviceType").Value.Trim();
            _friendlyName = deviceElement.Element(nm + "friendlyName")?.Value.Trim();
            _manufacturer = deviceElement.Element(nm + "manufacturer")?.Value.Trim();
            _manufacturerUrl = deviceElement.Element(nm + "manufacturerURL")?.Value.Trim();
            _modelDescription = deviceElement.Element(nm + "modelDescription")?.Value.Trim();
            _modelName = deviceElement.Element(nm + "modelName")?.Value.Trim();
            _modelNumber = deviceElement.Element(nm + "modelNumber")?.Value.Trim();
            _modelUrl = deviceElement.Element(nm + "modelURL")?.Value.Trim();
            _serialNumber = deviceElement.Element(nm + "serialNumber")?.Value.Trim();
            _udn = deviceElement.Element(nm + "UDN")?.Value.Trim();
            _upc = deviceElement.Element(nm + "UPC")?.Value.Trim();
            _presentationUrl = deviceElement.Element(nm + "presentationURL")?.Value.Trim();

            _parentDevice = parentDevice;
            _rootDevice = rootDevice;
            List<UPnPIcon> iconList = new List<UPnPIcon>();
            var iconElements = deviceElement.Element(nm + "iconList")?.Elements(nm + "icon");
            if (iconElements != null)
            {
                foreach (XElement iconElement in iconElements)
                {
                    iconList.Add(new UPnPIcon(iconElement, this, rootDevice));
                }
            }
            List<UPnPDevice> devices = new List<UPnPDevice>();
            var deviceElements = deviceElement.Element(nm + "deviceList")?.Elements(nm + "device");
            if (deviceElements != null)
            {
                foreach (XElement childDevice in deviceElements)
                {
                    devices.Add(new UPnPDevice(childDevice, this, rootDevice));
                }
            }
            List<UPnPService> services = new List<UPnPService>();
            var serviceElements = deviceElement.Element(nm + "serviceList")?.Elements(nm + "service");
            if (serviceElements != null)
            {
                foreach (XElement serviceElement in serviceElements)
                {
                    services.Add(new UPnPService(serviceElement, this, rootDevice));
                }
            }
            _iconList = iconList.ToArray();
            _devices = devices.ToArray();
            _services = services.ToArray();
            _interfaces = new UPnPDeviceInterfaces(this);
        }

        #endregion Construction

        /// <summary>
        /// Find the specified type of device if this device contains, else return "null".
        /// </summary>
        /// <param name="deviceType">Device type. Can used URN string as "urn:schemas-upnp-org:device:WANConnectionDevice:1".</param>
        /// <returns></returns>
        internal UPnPDevice FindDevice(string deviceType)
        {
            if (_deviceType.Equals(deviceType, StringComparison.OrdinalIgnoreCase))
            {
                return this;
            }
            else
            {
                foreach (UPnPDevice childDevice in _devices)
                {
                    UPnPDevice device = childDevice.FindDevice(deviceType);
                    if (device != null)
                    {
                        return device;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Find the specified type of device if this device contains,
        /// </summary>
        /// <param name="deviceType">Device type. Can used URN string as "urn:schemas-upnp-org:device:WANConnectionDevice:1".</param>
        /// <param name="devices">Devices found.</param>
        /// <returns></returns>
        internal void FindDevices(string deviceType, List<UPnPDevice> devices)
        {
            if (_deviceType.Equals(deviceType, StringComparison.OrdinalIgnoreCase))
            {
                devices.Add(this);
            }
            foreach (UPnPDevice childDevice in _devices)
            {
                childDevice.FindDevices(deviceType, devices);
            }
        }

        /// <summary>
        /// Find the specified type of service if this device provides, else return "null".
        /// </summary>
        /// <param name="serviceType">Service type. Can used URN string as "urn:schemas-upnp-org:service:WANIPConnection:1".</param>
        /// <returns></returns>
        internal UPnPService FindService(string serviceType)
        {
            foreach (UPnPService service in _services)
            {
                if (service.ServiceType.Equals(serviceType, StringComparison.OrdinalIgnoreCase))
                {
                    return service;
                }
            }
            foreach (UPnPDevice childDevice in _devices)
            {
                UPnPService service = childDevice.FindService(serviceType);
                if (service != null)
                {
                    return service;
                }
            }
            return null;
        }

        /// <summary>
        /// Find the specified type of service if this device provides,
        /// </summary>
        /// <param name="serviceType">Service type. Can used URN string as "urn:schemas-upnp-org:service:WANIPConnection:1".</param>
        /// <param name="services">Services found.</param>
        /// <returns></returns>
        internal void FindServices(string serviceType, List<UPnPService> services)
        {
            foreach (UPnPService service in _services)
            {
                if (service.ServiceType.Equals(serviceType, StringComparison.OrdinalIgnoreCase))
                {
                    services.Add(service);
                }
            }
            foreach (UPnPDevice childDevice in _devices)
            {
                childDevice.FindServices(serviceType, services);
            }
        }
    }
}