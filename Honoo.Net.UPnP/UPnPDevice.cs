using System;
using System.Collections.Generic;
using System.Xml;

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
        private readonly UPnPIcon[] _icons;
        private readonly UPnPDeviceInterfaces _interfaces;
        private readonly string _manufacturer;
        private readonly string _manufacturerUrl;
        private readonly string _modelDescription;
        private readonly string _modelName;
        private readonly string _modelNumber;
        private readonly string _modelUrl;
        private readonly UPnPDevice _parentDevice;
        private readonly string _presentationURL;
        private readonly UPnPRootDevice _rootDevice;
        private readonly string _serialNumber;
        private readonly UPnPService[] _services;
        private readonly string _udn;
        private readonly string _upc;

        /// <summary>
        /// Child devices.
        /// </summary>
        public ICollection<UPnPDevice> Devices => _devices;

        /// <summary>
        /// Device type.
        /// </summary>
        public string DeviceType => _deviceType;

        /// <summary>
        /// Friendly name.
        /// </summary>
        public string FriendlyName => _friendlyName;

        /// <summary>
        /// Icons.
        /// </summary>
        public ICollection<UPnPIcon> Icons => _icons;

        /// <summary>
        /// UPnP device interfaces.
        /// </summary>
        public UPnPDeviceInterfaces Interfaces => _interfaces;

        /// <summary>
        /// Manufacturer.
        /// </summary>
        public string Manufacturer => _manufacturer;

        /// <summary>
        /// Manufacturer url.
        /// </summary>
        public string ManufacturerUrl => _manufacturerUrl;

        /// <summary>
        /// Model description.
        /// </summary>
        public string ModelDescription => _modelDescription;

        /// <summary>
        /// Model name.
        /// </summary>
        public string ModelName => _modelName;

        /// <summary>
        /// Model number.
        /// </summary>
        public string ModelNumber => _modelNumber;

        /// <summary>
        /// Model url.
        /// </summary>
        public string ModelUrl => _modelUrl;

        /// <summary>
        /// Parent device.
        /// </summary>
        public UPnPDevice ParentDevice => _parentDevice;

        /// <summary>
        /// Presentation URL.
        /// </summary>
        public string PresentationURL => _presentationURL;

        /// <summary>
        /// Root device.
        /// </summary>
        public UPnPRootDevice RootDevice => _rootDevice;

        /// <summary>
        /// Serial number.
        /// </summary>
        public string SerialNumber => _serialNumber;

        /// <summary>
        /// Services.
        /// </summary>
        public ICollection<UPnPService> Services => _services;

        /// <summary>
        /// Unique device name.
        /// </summary>
        public string Udn => _udn;

        /// <summary>
        /// Universal product code.
        /// </summary>
        public string Upc => _upc;

        #endregion Members

        #region Construction

        /// <summary>
        /// Initializes a new instance of the UPnPDevice class.
        /// </summary>
        /// <param name="deviceNode">Device XmlNode.</param>
        /// <param name="nm">XmlNamespaceManager.</param>
        /// <param name="parentDevice">The UPnPDevice to which the device belongs.</param>
        /// <param name="rootDevice">The UPnPRootDevice to which the root device belongs.</param>
        /// <exception cref="Exception"/>
        internal UPnPDevice(XmlNode deviceNode, XmlNamespaceManager nm, UPnPDevice parentDevice, UPnPRootDevice rootDevice)
        {
            _deviceType = deviceNode.SelectSingleNode("default:deviceType", nm).InnerText.Trim();
            _friendlyName = deviceNode.SelectSingleNode("default:friendlyName", nm)?.InnerText.Trim();
            _manufacturer = deviceNode.SelectSingleNode("default:manufacturer", nm)?.InnerText.Trim();
            _manufacturerUrl = deviceNode.SelectSingleNode("default:manufacturerURL", nm)?.InnerText.Trim();
            _modelDescription = deviceNode.SelectSingleNode("default:modelDescription", nm)?.InnerText.Trim();
            _modelName = deviceNode.SelectSingleNode("default:modelName", nm)?.InnerText.Trim();
            _modelNumber = deviceNode.SelectSingleNode("default:modelNumber", nm)?.InnerText.Trim();
            _modelUrl = deviceNode.SelectSingleNode("default:modelURL", nm)?.InnerText.Trim();
            _serialNumber = deviceNode.SelectSingleNode("default:serialNumber", nm)?.InnerText.Trim();
            _udn = deviceNode.SelectSingleNode("default:UDN", nm)?.InnerText.Trim();
            _upc = deviceNode.SelectSingleNode("default:UPC", nm)?.InnerText.Trim();
            _presentationURL = deviceNode.SelectSingleNode("default:presentationURL", nm)?.InnerText.Trim();

            //_xDlnaDoc = deviceNode.SelectSingleNode("dlna:X_DLNADOC", nm)?.InnerText.Trim();

            _parentDevice = parentDevice;
            _rootDevice = rootDevice;
            List<UPnPIcon> icons = new List<UPnPIcon>();
            XmlNodeList childIconNodes = deviceNode.SelectNodes("default:iconList/default:icon", nm);
            if (childIconNodes != null)
            {
                foreach (XmlNode childIconNode in childIconNodes)
                {
                    icons.Add(new UPnPIcon(childIconNode, nm, this, rootDevice));
                }
            }
            List<UPnPDevice> devices = new List<UPnPDevice>();
            XmlNodeList childDeviceNodes = deviceNode.SelectNodes("default:deviceList/default:device", nm);
            if (childDeviceNodes != null)
            {
                foreach (XmlNode childDeviceNode in childDeviceNodes)
                {
                    devices.Add(new UPnPDevice(childDeviceNode, nm, this, rootDevice));
                }
            }
            List<UPnPService> services = new List<UPnPService>();
            XmlNodeList serviceNodes = deviceNode.SelectNodes("default:serviceList/default:service", nm);
            if (serviceNodes != null)
            {
                foreach (XmlNode serviceNode in serviceNodes)
                {
                    services.Add(new UPnPService(serviceNode, nm, this, rootDevice));
                }
            }
            _icons = icons.ToArray();
            _devices = devices.ToArray();
            _services = services.ToArray();
            _interfaces = new UPnPDeviceInterfaces(this);
        }

        #endregion Construction

        /// <summary>
        /// Find the specified type of device if this device contains, else return "null".
        /// </summary>
        /// <param name="deviceType">Device type. Can used URN string as "urn:schemas-upnp-org:device:WANConnectionDevice:1".
        ///</param>
        /// <returns></returns>
        internal UPnPDevice FindDevice(string deviceType)
        {
            UPnPDevice result = null;
            if (_deviceType.Equals(deviceType, StringComparison.OrdinalIgnoreCase))
            {
                result = this;
            }
            else
            {
                foreach (UPnPDevice childDevice in _devices)
                {
                    result = childDevice.FindDevice(deviceType);
                    if (result != null)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Find the specified type of service if this device provides, else return "null".
        /// </summary>
        /// <param name="serviceType">Service type. Can used URN string as "urn:schemas-upnp-org:service:WANIPConnection:1".
        ///</param>
        /// <returns></returns>
        internal UPnPService FindService(string serviceType)
        {
            UPnPService result = null;
            foreach (UPnPService service in _services)
            {
                if (service.ServiceType.Equals(serviceType, StringComparison.OrdinalIgnoreCase))
                {
                    result = service;
                    break;
                }
            }
            if (result == null)
            {
                foreach (UPnPDevice childDevice in _devices)
                {
                    result = childDevice.FindService(serviceType);
                    if (result != null)
                    {
                        break;
                    }
                }
            }
            return result;
        }
    }
}