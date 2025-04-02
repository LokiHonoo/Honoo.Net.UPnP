using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP root device.
    /// </summary>
    public sealed class UPnPRootDevice : IDisposable
    {
        #region Members

        private readonly WebClient _client;
        private readonly string _descriptionUrl;
        private readonly UPnPDevice _device;
        private readonly WebHeaderCollection _headers = new WebHeaderCollection();
        private readonly Version _specVersion;
        private readonly Uri _uriBase;
        private bool _disposed;

        /// <summary>
        /// Description url.
        /// </summary>
        public string DescriptionUrl => _descriptionUrl;

        /// <summary>
        /// Main device.
        /// </summary>
        public UPnPDevice Device => _device;

        /// <summary>
        /// Client headers. Basic for
        /// <br/>"Cache-Control: no-store"
        /// <br/>"Pragma: no-cache"
        /// <br/>"Content-Type: text/xml; charset=utf-8"
        /// <br/>"User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0"
        /// <br/>
        /// <br/>Append MAN HTTP Header If throw 405 WebException. <see cref="Headers"/>.Add("MAN: \"http://schemas.xmlsoap.org/soap/envelope/\"; ns=01").
        /// </summary>
        public WebHeaderCollection Headers => _headers;

        /// <summary>
        /// Specification version.
        /// </summary>
        public Version SpecVersion => _specVersion;

        /// <summary>
        /// Url base.
        /// </summary>
        public Uri UriBase => _uriBase;

        internal WebClient Client => _client;

        #endregion Members

        #region Construction

        /// <summary>
        /// Initializes a new instance of the UPnPRootDevice class.
        /// </summary>
        /// <param name="descriptionUrl">Description page url.</param>
        internal UPnPRootDevice(string descriptionUrl)
        {
            _descriptionUrl = descriptionUrl;
            Uri descriptionUri = new Uri(descriptionUrl);
            _uriBase = new Uri(descriptionUri.GetLeftPart(UriPartial.Authority));
            _headers.Add("Cache-Control: no-store");
            _headers.Add("Pragma: no-cache");
            _headers.Add("Content-Type: text/xml; charset=utf-8");
            _headers.Add("User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
            _client = new WebClient
            {
                BaseAddress = _uriBase.AbsoluteUri,
                Encoding = Encoding.UTF8
            };
            _client.Headers.Add(_headers);
            string response = _client.DownloadString(descriptionUri);
            XDocument doc = XDocument.Parse(response);
            XNamespace nm = doc.Root.GetDefaultNamespace();
            XElement specVersion = doc.Root.Element(nm + "specVersion");
            string major = specVersion.Element(nm + "major").Value.Trim();
            string minor = specVersion.Element(nm + "minor").Value.Trim();
            _specVersion = Version.Parse(major + "." + minor);
            XElement device = doc.Root.Element(nm + "device");
            _device = new UPnPDevice(device, null, this);

            //XmlDocument doc = new XmlDocument() { XmlResolver = null };
            //doc.LoadXml(description);
            //XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            //ns.AddNamespace("default", "urn:schemas-upnp-org:device-1-0");
            //ns.AddNamespace("dlna", "urn:schemas-dlna-org:device-1-0");
            //string major = doc.SelectSingleNode("/default:root/default:specVersion/default:major", ns).InnerText.Trim();
            //string minor = doc.SelectSingleNode("/default:root/default:specVersion/default:minor", ns).InnerText.Trim();
            //_specVersion = Version.Parse(major + "." + minor);
            //XmlNode deviceNode = doc.SelectSingleNode("/default:root/default:device", ns);
            //_device = new UPnPDevice(deviceNode, ns, null, this);
        }

        /// <summary>
        /// Releases resources at the instance.
        /// </summary>
        ~UPnPRootDevice()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases resources at the instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources at the instance.
        /// </summary>
        /// <param name="disposing">Releases managed resources.</param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }
                _client.Dispose();
                _disposed = true;
            }
        }

        #endregion Construction

        /// <summary>
        /// Find the specified type of device if this device contains, else return "null".
        /// </summary>
        /// <param name="deviceType">Device type. Can used URN string as "urn:schemas-upnp-org:device:WANConnectionDevice:1".</param>
        /// <returns></returns>
        public UPnPDevice FindDevice(string deviceType)
        {
            return _device.FindDevice(deviceType);
        }

        /// <summary>
        /// Find the specified type of device if this device contains,
        /// </summary>
        /// <param name="deviceType">Device type. Can used URN string as "urn:schemas-upnp-org:device:WANConnectionDevice:1".</param>
        /// <returns></returns>
        public UPnPDevice[] FindDevices(string deviceType)
        {
            var devices = new List<UPnPDevice>();
            _device.FindDevices(deviceType, devices);
            return devices.ToArray();
        }

        /// <summary>
        /// Find the specified type of service if this device provides, else return "null".
        /// </summary>
        /// <param name="serviceType">Service type. Can used URN string as "urn:schemas-upnp-org:service:WANIPConnection:1".</param>
        /// <returns></returns>
        public UPnPService FindService(string serviceType)
        {
            return _device.FindService(serviceType);
        }

        /// <summary>
        /// Find the specified type of service if this device provides,
        /// </summary>
        /// <param name="serviceType">Service type. Can used URN string as "urn:schemas-upnp-org:service:WANIPConnection:1".</param>
        /// <returns></returns>
        public UPnPService[] FindServices(string serviceType)
        {
            var services = new List<UPnPService>();
            _device.FindServices(serviceType, services);
            return services.ToArray();
        }
    }
}