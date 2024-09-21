using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;

namespace Honoo.Net.UPnP
{
    /// <summary>
    /// UPnP root device.
    /// </summary>
    public sealed class UPnPRootDevice : IDisposable
    {
        #region Properties

        private readonly Uri _baseUri;
        private readonly WebClient _client;
        private readonly string _descriptionUrl;
        private readonly UPnPDevice _device;
        private readonly IDictionary<string, string> _headerExtensions = new Dictionary<string, string>();
        private readonly Version _specVersion;
        private bool _disposed;

        /// <summary>
        /// Base uri.
        /// </summary>
        public Uri BaseUri => _baseUri;

        /// <summary>
        /// Description url.
        /// </summary>
        public string DescriptionUrl => _descriptionUrl;

        /// <summary>
        /// Main device.
        /// </summary>
        public UPnPDevice Device => _device;

        /// <summary>
        /// Append custom header if this device doesn't work. Basic for
        /// <br/>"Cache-Control: no-store"
        /// <br/>"Pragma: no-cache"
        /// <br/>"Content-Type: text/xml; charset=utf-8"
        /// <br/>"User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0"
        /// <br/>
        /// <br/>Append MAN HTTP Header If throw 405 WebException. MAN: "http://schemas.xmlsoap.org/soap/envelope/"; ns=01
        /// </summary>
        public IDictionary<string, string> HeaderExtensions => _headerExtensions;

        /// <summary>
        /// Specification version.
        /// </summary>
        public Version SpecVersion => _specVersion;

        internal WebClient Client => _client;

        #endregion Properties

        #region Construction

        /// <summary>
        /// Initializes a new instance of the UPnPRootDevice class.
        /// </summary>
        /// <param name="descriptionUrl">Description page url.</param>
        internal UPnPRootDevice(string descriptionUrl)
        {
            _descriptionUrl = descriptionUrl;
            Uri descriptionUri = new Uri(descriptionUrl);
            _baseUri = new Uri(descriptionUri.GetLeftPart(UriPartial.Authority));
            _client = new WebClient
            {
                BaseAddress = _baseUri.AbsoluteUri,
                Encoding = Encoding.UTF8
            };
            _client.Headers.Add("Cache-Control: no-store");
            _client.Headers.Add("Pragma: no-cache");
            _client.Headers.Add("Content-Type: text/xml; charset=utf-8");
            _client.Headers.Add("User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
            if (_headerExtensions.Count > 0)
            {
                foreach (var header in _headerExtensions)
                {
                    _client.Headers.Add(header.Key, header.Value);
                }
            }
            string description = _client.DownloadString(descriptionUri);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(description);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("default", "urn:schemas-upnp-org:device-1-0");
            ns.AddNamespace("dlna", "urn:schemas-dlna-org:device-1-0");
            string major = doc.SelectSingleNode("/default:root/default:specVersion/default:major", ns).InnerText.Trim();
            string minor = doc.SelectSingleNode("/default:root/default:specVersion/default:minor", ns).InnerText.Trim();
            _specVersion = Version.Parse(major + "." + minor);
            XmlNode deviceNode = doc.SelectSingleNode("/default:root/default:device", ns);
            _device = new UPnPDevice(deviceNode, ns, null, this);
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
        /// <param name="deviceType">Device type.</param>
        /// <returns></returns>
        public UPnPDevice FindDevice(string deviceType)
        {
            return _device.FindDevice(deviceType);
        }

        /// <summary>
        /// Find the specified type of service if this device provides, else return "null".
        /// </summary>
        /// <param name="serviceType">Service type.</param>
        /// <returns></returns>
        public UPnPService FindService(string serviceType)
        {
            return _device.FindService(serviceType);
        }
    }
}