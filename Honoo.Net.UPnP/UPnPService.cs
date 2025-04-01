using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Xml;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP service. Convert to the interface to call the relevant method.
    /// </summary>
    public sealed class UPnPService :
        IUPnPService,
        IUPnPWANIPConnection1Service,
        IUPnPWANIPConnection2Service,
        IUPnPWANPPPConnection1Service,
        IUPnPAVTransport1Service,
        IUPnPAVTransport2Service
    {
        #region Members

        private readonly string _controlUrl;
        private readonly string _eventSubUrl;
        private readonly UPnPServiceInterfaces _interfaces;
        private readonly UPnPDevice _parentDevice;
        private readonly UPnPRootDevice _rootDevice;
        private readonly string _scpdUrl;
        private readonly string _serviceID;
        private readonly string _serviceType;

        /// <summary>
        /// Control url.
        /// </summary>
        public string ControlUrl => _controlUrl;

        /// <summary>
        /// Event sub url.
        /// </summary>
        public string EventSubUrl => _eventSubUrl;

        /// <summary>
        /// UPnP service interfaces.
        /// </summary>
        public UPnPServiceInterfaces Interfaces => _interfaces;

        /// <summary>
        /// Parent device.
        /// </summary>
        public UPnPDevice ParentDevice => _parentDevice;

        /// <summary>
        /// Root device.
        /// </summary>
        public UPnPRootDevice RootDevice => _rootDevice;

        /// <summary>
        /// Scpd url.
        /// </summary>
        public string ScpdUrl => _scpdUrl;

        /// <summary>
        /// Service ID.
        /// </summary>
        public string ServiceID => _serviceID;

        /// <summary>
        /// Service type.
        /// </summary>
        public string ServiceType => _serviceType;

        #endregion Members

        #region Construction

        /// <summary>
        /// Initializes a new instance of the UPnPService class.
        /// </summary>
        /// <param name="serviceNode">Service XmlNode.</param>
        /// <param name="nm">XmlNamespaceManager.</param>
        /// <param name="parentDevice">The UPnPDevice to which the device belongs.</param>
        /// <param name="rootDevice">The UPnPRootDevice to which the root device belongs.</param>
        /// <exception cref="Exception"/>
        internal UPnPService(XmlNode serviceNode, XmlNamespaceManager nm, UPnPDevice parentDevice, UPnPRootDevice rootDevice)
        {
            _controlUrl = serviceNode.SelectSingleNode("default:controlURL", nm).InnerText.Trim();
            _eventSubUrl = serviceNode.SelectSingleNode("default:eventSubURL", nm).InnerText.Trim();
            _scpdUrl = serviceNode.SelectSingleNode("default:SCPDURL", nm).InnerText.Trim();
            _serviceID = serviceNode.SelectSingleNode("default:serviceId", nm).InnerText.Trim();
            _serviceType = serviceNode.SelectSingleNode("default:serviceType", nm).InnerText.Trim();
            _parentDevice = parentDevice;
            _rootDevice = rootDevice;
            _interfaces = new UPnPServiceInterfaces(this);
        }

        #endregion Construction

        #region Common

        /// <summary>
        /// Gets SCPD information form SCPD url.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string GetScpdInformation()
        {
            _rootDevice.Client.Headers.Clear();
            _rootDevice.Client.Headers.Add("Cache-Control: no-store");
            _rootDevice.Client.Headers.Add("Pragma: no-cache");
            _rootDevice.Client.Headers.Add("Content-Type: text/xml; charset=utf-8");
            _rootDevice.Client.Headers.Add("User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
            return _rootDevice.Client.DownloadString(_scpdUrl);
        }

        /// <summary>
        /// Post action, and gets response xml string. Query actions from service's SCPD information description page.
        /// </summary>
        /// <param name="action">action name.</param>
        /// <param name="arguments">action arguments. The arguments must conform to the order specified. Set 'null' if haven't arguments.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string PostAction(string action, IDictionary<string, string> arguments)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            sb.AppendLine("  <s:Body>");
            sb.AppendLine("    <u:" + action + " xmlns:u=\"" + _serviceType + "\">");
            if (arguments != null && arguments.Count > 0)
            {
                foreach (KeyValuePair<string, string> argument in arguments)
                {
                    sb.AppendLine("      <" + argument.Key + ">" + argument.Value + "</" + argument.Key + ">");
                }
            }
            sb.AppendLine("    </u:" + action + ">");
            sb.AppendLine("  </s:Body>");
            sb.AppendLine("</s:Envelope>");
            string body = sb.ToString();
            _rootDevice.Client.Headers.Clear();
            _rootDevice.Client.Headers.Add("Cache-Control: no-store");
            _rootDevice.Client.Headers.Add("Pragma: no-cache");
            _rootDevice.Client.Headers.Add("Content-Type: text/xml; charset=utf-8");
            _rootDevice.Client.Headers.Add("User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
            _rootDevice.Client.Headers.Add($"SOAPAction: \"{_serviceType}#{action}\"");
            if (_rootDevice.HeaderExtensions.Count > 0)
            {
                foreach (var header in _rootDevice.HeaderExtensions)
                {
                    _rootDevice.Client.Headers.Add(header.Key, header.Value);
                }
            }
            return _rootDevice.Client.UploadString(_controlUrl, "POST", body);
        }

        /// <summary>
        ///  Remove event subscription.
        /// </summary>
        /// <param name="sid">Event subscription sid.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public void RemoveEventSubscription(string sid)
        {
            _rootDevice.Client.Headers.Clear();
            _rootDevice.Client.Headers.Add("Cache-Control: no-store");
            _rootDevice.Client.Headers.Add("Pragma: no-cache");
            _rootDevice.Client.Headers.Add("Content-Type: text/xml; charset=utf-8");
            _rootDevice.Client.Headers.Add("User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
            _rootDevice.Client.Headers.Add($"SID: {sid}");
            _rootDevice.Client.UploadString(_eventSubUrl, "UNSUBSCRIBE", string.Empty);
        }

        /// <summary>
        ///  Renewal event subscription.
        /// </summary>
        /// <param name="sid">Event subscription sid.</param>
        /// <param name="durationSecond">Subscription duration. Unit is second.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public void RenewalEventSubscription(string sid, uint durationSecond)
        {
            _rootDevice.Client.Headers.Clear();
            _rootDevice.Client.Headers.Add("Cache-Control: no-store");
            _rootDevice.Client.Headers.Add("Pragma: no-cache");
            _rootDevice.Client.Headers.Add("Content-Type: text/xml; charset=utf-8");
            _rootDevice.Client.Headers.Add("User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
            _rootDevice.Client.Headers.Add($"SID: {sid}");
            _rootDevice.Client.Headers.Add($"TIMEOUT: Second-{durationSecond}");
            _rootDevice.Client.UploadString(_eventSubUrl, "SUBSCRIBE", string.Empty);
        }

        /// <summary>
        /// Set event subscription by event subscriber url and gets event SID.
        /// </summary>
        /// <param name="subscriberUrl">Event subscriber url.</param>
        /// <param name="durationSecond">Subscription duration. Unit is second.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public string SetEventSubscription(string subscriberUrl, uint durationSecond)
        {
            _rootDevice.Client.Headers.Clear();
            _rootDevice.Client.Headers.Add("Cache-Control: no-store");
            _rootDevice.Client.Headers.Add("Pragma: no-cache");
            _rootDevice.Client.Headers.Add("Content-Type: text/xml; charset=utf-8");
            _rootDevice.Client.Headers.Add("User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
            _rootDevice.Client.Headers.Add("NT: upnp:event");
            _rootDevice.Client.Headers.Add($"CALLBACK: <{subscriberUrl}>");
            _rootDevice.Client.Headers.Add($"TIMEOUT: Second-{durationSecond}");
            _rootDevice.Client.UploadString(_eventSubUrl, "SUBSCRIBE", string.Empty);
            return _rootDevice.Client.ResponseHeaders["SID"];
        }

        #endregion Common

        #region WANIPConnection1

        /// <summary>
        /// Add port mapping.
        /// </summary>
        /// <param name="protocol">The protocol to mapping. This property accepts the following: "TCP", "UDP".</param>
        /// <param name="externalPort">The external port to mapping.</param>
        /// <param name="internalClient">The internal client to mapping.</param>
        /// <param name="internalPort">The internal port to mapping.</param>
        /// <param name="enabled">Enabled.</param>
        /// <param name="description">Port mapping description.</param>
        /// <param name="leaseDuration">Lease duration. This property accepts the following 0 - 604800. Unit is seconds. Set 0 to permanents.</param>
        /// <exception cref="Exception"/>
        void IUPnPWANConnectionService.AddPortMapping(string protocol,
                                                         ushort externalPort,
                                                         IPAddress internalClient,
                                                         ushort internalPort,
                                                         bool enabled,
                                                         string description,
                                                         uint leaseDuration)
        {
            if (internalClient is null)
            {
                throw new ArgumentNullException(nameof(internalClient));
            }
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewProtocol", protocol },
                { "NewRemoteHost", string.Empty },
                { "NewExternalPort", externalPort.ToString(CultureInfo.InvariantCulture) },
                { "NewInternalClient", internalClient.ToString() },
                { "NewInternalPort", internalPort.ToString(CultureInfo.InvariantCulture) },
                { "NewEnabled", enabled ? "1" : "0" },
                { "NewPortMappingDescription", description },
                { "NewLeaseDuration", leaseDuration.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("AddPortMapping", arguments);
        }

        /// <summary>
        /// Delete port mapping.
        /// </summary>
        /// <param name="protocol">The protocol to delete mapping. This property accepts the following: "TCP", "UDP".</param>
        /// <param name="externalPort">The external port to delete mapping.</param>
        /// <exception cref="Exception"/>
        void IUPnPWANConnectionService.DeletePortMapping(string protocol, ushort externalPort)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewProtocol", protocol },
                { "NewRemoteHost", string.Empty },
                { "NewExternalPort", externalPort.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("DeletePortMapping", arguments);
        }

        /// <summary>
        /// Force termination to change ConnectionStatus to Disconnected.
        /// </summary>
        /// <exception cref="Exception"/>
        void IUPnPWANConnectionService.ForceTermination()
        {
            PostAction("ForceTermination", null);
        }

        /// <summary>
        /// Get auto disconnect time in seconds.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        uint IUPnPWANConnectionService.GetAutoDisconnectTime()
        {
            string response = PostAction("GetAutoDisconnectTime", null);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetAutoDisconnectTimeResponse", ns);
            return uint.Parse(node.SelectSingleNode("NewAutoDisconnectTime").InnerText.Trim(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get Connection type info. Possible types maybe wrong because I don't know what the separator is. :(
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPConnectionTypeInfo IUPnPWANConnectionService.GetConnectionTypeInfo()
        {
            string response = PostAction("GetConnectionTypeInfo", null);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetConnectionTypeInfoResponse", ns);
            return new UPnPConnectionTypeInfo(node);
        }

        /// <summary>
        /// Get external IPAddress.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string IUPnPWANConnectionService.GetExternalIPAddress()
        {
            string response = PostAction("GetExternalIPAddress", null);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetExternalIPAddressResponse", ns);
            return node.SelectSingleNode("NewExternalIPAddress").InnerText.Trim();
        }

        /// <summary>
        /// Get generic port mapping entry.
        /// </summary>
        /// <param name="index">The index of entry.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPPortMappingEntry IUPnPWANConnectionService.GetGenericPortMappingEntry(uint index)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewPortMappingIndex", index.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetGenericPortMappingEntry", arguments);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetGenericPortMappingEntryResponse", ns);
            return new UPnPPortMappingEntry(node);
        }

        /// <summary>
        /// Get Idle disconnect time in seconds.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        uint IUPnPWANConnectionService.GetIdleDisconnectTime()
        {
            string response = PostAction("GetIdleDisconnectTime", null);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetIdleDisconnectTimeResponse", ns);
            return uint.Parse(node.SelectSingleNode("NewIdleDisconnectTime").InnerText.Trim(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get NAT RSIP status.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPNatRsipStatus IUPnPWANConnectionService.GetNATRSIPStatus()
        {
            string response = PostAction("GetNATRSIPStatus", null);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetNATRSIPStatusResponse", ns);
            return new UPnPNatRsipStatus(node);
        }

        /// <summary>
        /// Get specific port mapping entry.
        /// </summary>
        /// <param name="protocol">The protocol to query. This property accepts the following: "TCP", "UDP".</param>
        /// <param name="externalPort">The external port to query.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPPortMappingEntry IUPnPWANConnectionService.GetSpecificPortMappingEntry(string protocol, ushort externalPort)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewProtocol", protocol },
                { "NewRemoteHost", string.Empty },
                { "NewExternalPort", externalPort.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetSpecificPortMappingEntry", arguments);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetSpecificPortMappingEntryResponse", ns);
            return new UPnPPortMappingEntry(protocol, string.Empty, externalPort, node);
        }

        /// <summary>
        /// Get status info.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPStatusInfo IUPnPWANConnectionService.GetStatusInfo()
        {
            string response = PostAction("GetStatusInfo", null);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetStatusInfoResponse", ns);
            return new UPnPStatusInfo(node);
        }

        /// <summary>
        /// Get warn disconnect delay in seconds.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        uint IUPnPWANConnectionService.GetWarnDisconnectDelay()
        {
            string response = PostAction("GetWarnDisconnectDelay", null);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetWarnDisconnectDelayResponse", ns);
            return uint.Parse(node.SelectSingleNode("NewWarnDisconnectDelay").InnerText.Trim(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Request connection.
        /// </summary>
        /// <exception cref="Exception"/>
        void IUPnPWANConnectionService.RequestConnection()
        {
            PostAction("RequestConnection", null);
        }

        /// <summary>
        /// Request termination to change ConnectionStatus to Disconnected.
        /// </summary>
        /// <exception cref="Exception"/>
        void IUPnPWANConnectionService.RequestTermination()
        {
            PostAction("RequestTermination", null);
        }

        /// <summary>
        /// Set auto disconnect time in seconds.
        /// </summary>
        /// <param name="seconds">Sets the time (in seconds) after which an active connection is automatically disconnected.</param>
        /// <exception cref="Exception"/>
        void IUPnPWANConnectionService.SetAutoDisconnectTime(uint seconds)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewAutoDisconnectTime", seconds.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetAutoDisconnectTime", arguments);
        }

        /// <summary>
        /// Set connection type.
        /// </summary>
        /// <param name="connectionType">The connection type. This property accepts the following: "Unconfigured", "IP_Routed", "IP_Bridged".</param>
        /// <exception cref="Exception"/>
        void IUPnPWANConnectionService.SetConnectionType(string connectionType)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewConnectionType", connectionType },
            };
            PostAction("SetConnectionType", arguments);
        }

        /// <summary>
        /// Set Idle disconnect time in seconds.
        /// </summary>
        /// <param name="seconds">Specifies the idle time (in seconds) after which a connection may be disconnected. The actual disconnect will occur after WarnDisconnectDelay time elapses.</param>
        /// <exception cref="Exception"/>
        void IUPnPWANConnectionService.SetIdleDisconnectTime(uint seconds)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewIdleDisconnectTime", seconds.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetIdleDisconnectTime", arguments);
        }

        /// <summary>
        /// Set warn disconnect delay in seconds.
        /// </summary>
        /// <param name="seconds">Specifies the number of seconds of warning to each (potentially) active user of a connection before a connection is terminated.</param>
        /// <exception cref="Exception"/>
        void IUPnPWANConnectionService.SetWarnDisconnectDelay(uint seconds)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewWarnDisconnectDelay", seconds.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetWarnDisconnectDelay", arguments);
        }

        #endregion WANIPConnection1

        #region WANIPConnection2

        /// <summary>
        /// Add any port mapping, and gets reserved port.
        /// </summary>
        /// <param name="protocol">The protocol to mapping. This property accepts the following: "TCP", "UDP".</param>
        /// <param name="externalPort">The external port to mapping.</param>
        /// <param name="internalClient">The internal client to mapping.</param>
        /// <param name="internalPort">The internal port to mapping.</param>
        /// <param name="enabled">Enabled.</param>
        /// <param name="description">Port mapping description.</param>
        /// <param name="leaseDuration">Lease duration. This property accepts the following 0 - 604800. Unit is seconds. Set 0 to permanents.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        ushort IUPnPWANIPConnection2Service.AddAnyPortMapping(string protocol,
                                                               ushort externalPort,
                                                               IPAddress internalClient,
                                                               ushort internalPort,
                                                               bool enabled,
                                                               string description,
                                                               uint leaseDuration)
        {
            if (internalClient is null)
            {
                throw new ArgumentNullException(nameof(internalClient));
            }
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewProtocol", protocol },
                { "NewRemoteHost", string.Empty },
                { "NewExternalPort", externalPort.ToString(CultureInfo.InvariantCulture) },
                { "NewInternalClient", internalClient.ToString() },
                { "NewInternalPort", internalPort.ToString(CultureInfo.InvariantCulture) },
                { "NewEnabled", enabled ? "1" : "0" },
                { "NewPortMappingDescription", description },
                { "NewLeaseDuration", leaseDuration.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("AddAnyPortMapping", arguments);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:AddAnyPortMappingResponse", ns);
            return ushort.Parse(node.SelectSingleNode("NewReservedPort").InnerText.Trim(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Delete port mapping range.
        /// </summary>
        /// <param name="protocol">The protocol to delete mapping. This property accepts the following: "TCP", "UDP".</param>
        /// <param name="startPort">The start port of search.</param>
        /// <param name="endPort">The end port of search.</param>
        /// <param name="manage">Elevate privileges.</param>
        /// <exception cref="Exception"/>
        void IUPnPWANIPConnection2Service.DeletePortMappingRange(string protocol, ushort startPort, ushort endPort, bool manage)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewProtocol", protocol },
                { "NewStartPort", startPort.ToString(CultureInfo.InvariantCulture) },
                { "NewEndPort", endPort.ToString(CultureInfo.InvariantCulture) },
                { "NewManage", manage ? "1" : "0" },
            };
            PostAction("DeletePortMappingRange", arguments);
        }

        /// <summary>
        /// Get list of port mapping entries.
        /// </summary>
        /// <param name="protocol">The protocol to query. This property accepts the following: "TCP", "UDP".</param>
        /// <param name="startPort">The start port of search.</param>
        /// <param name="endPort">The end port of search.</param>
        /// <param name="manage">Elevate privileges.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string IUPnPWANIPConnection2Service.GetListOfPortMappings(string protocol, ushort startPort, ushort endPort, bool manage)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewProtocol", protocol },
                { "NewStartPort", startPort.ToString(CultureInfo.InvariantCulture) },
                { "NewEndPort", endPort.ToString(CultureInfo.InvariantCulture) },
                { "NewManage", manage ? "1" : "0" },
                { "NewNumberOfPorts", "65535" },
            };
            string response = PostAction("GetListOfPortMappings", arguments);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetListOfPortMappingsResponse", ns);
            return node.SelectSingleNode("NewPortListing").InnerText.Trim();
        }

        #endregion WANIPConnection2

        #region WANPPPConnection1

        /// <summary>
        /// Gets link layer max bit rates.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPLinkLayerMaxBitRates IUPnPWANPPPConnection1Service.GetLinkLayerMaxBitRates()
        {
            string response = PostAction("GetLinkLayerMaxBitRates", null);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetLinkLayerMaxBitRatesResponse", ns);
            return new UPnPLinkLayerMaxBitRates(node);
        }

        /// <summary>
        /// Gets password.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string IUPnPWANPPPConnection1Service.GetPassword()
        {
            string response = PostAction("GetPassword", null);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetPasswordResponse", ns);
            return node.SelectSingleNode("NewPassword").InnerText.Trim();
        }

        /// <summary>
        /// Gets PPP authentication protocol.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string IUPnPWANPPPConnection1Service.GetPPPAuthenticationProtocol()
        {
            string response = PostAction("GetPPPAuthenticationProtocol", null);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetPPPAuthenticationProtocolResponse", ns);
            return node.SelectSingleNode("NewPPPAuthenticationProtocol").InnerText.Trim();
        }

        /// <summary>
        /// Gets PPP compression protocol.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string IUPnPWANPPPConnection1Service.GetPPPCompressionProtocol()
        {
            string response = PostAction("GetPPPCompressionProtocol", null);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetPPPCompressionProtocolResponse", ns);
            return node.SelectSingleNode("NewPPPCompressionProtocol").InnerText.Trim();
        }

        /// <summary>
        /// Gets PPP encryption protocol.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string IUPnPWANPPPConnection1Service.GetPPPEncryptionProtocol()
        {
            string response = PostAction("GetPPPEncryptionProtocol", null);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetPPPEncryptionProtocolResponse", ns);
            return node.SelectSingleNode("NewPPPEncryptionProtocol").InnerText.Trim();
        }

        /// <summary>
        /// Gets user name.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string IUPnPWANPPPConnection1Service.GetUserName()
        {
            string response = PostAction("GetUserName", null);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetUserNameResponse", ns);
            return node.SelectSingleNode("NewUserName").InnerText.Trim();
        }

        #endregion WANPPPConnection1

        #region AVTransport1

        /// <summary>
        /// Get current transport actions.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string IUPnPAVTransport1Service.GetCurrentTransportActions(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetCurrentTransportActions", arguments);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetCurrentTransportActionsResponse", ns);
            return node.SelectSingleNode("Actions").InnerText.Trim();
        }

        /// <summary>
        /// Get device capabilities.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPDeviceCapabilities IUPnPAVTransport1Service.GetDeviceCapabilities(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetDeviceCapabilities", arguments);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetDeviceCapabilitiesResponse", ns);
            return new UPnPDeviceCapabilities(node);
        }

        /// <summary>
        /// Get media info.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPMediaInfo IUPnPAVTransport1Service.GetMediaInfo(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetMediaInfo", arguments);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetMediaInfoResponse", ns);
            return new UPnPMediaInfo(node);
        }

        /// <summary>
        /// Get position info.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPPositionInfo IUPnPAVTransport1Service.GetPositionInfo(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetPositionInfo", arguments);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetPositionInfoResponse", ns);
            return new UPnPPositionInfo(node);
        }

        /// <summary>
        /// Get transport info.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPTransportInfo IUPnPAVTransport1Service.GetTransportInfo(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetTransportInfo", arguments);

            //XElement element = XElement.Parse(response);
            //XNamespace ns1 = "http://schemas.xmlsoap.org/soap/envelope/";
            //XNamespace ns2 = _serviceType;
            //element.Element(ns1 + "Envelope").Element(ns1 + "Body").Element(ns2 + "GetTransportInfoResponse");

            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetTransportInfoResponse", ns);
            return new UPnPTransportInfo(node);
        }

        /// <summary>
        /// Get transport settings.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPTransportSettings IUPnPAVTransport1Service.GetTransportSettings(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetTransportSettings", arguments);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetTransportSettingsResponse", ns);
            return new UPnPTransportSettings(node);
        }

        /// <summary>
        /// Jump next.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <exception cref="Exception"/>
        void IUPnPAVTransport1Service.Next(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("Next", arguments);
        }

        /// <summary>
        /// Pause.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <exception cref="Exception"/>
        void IUPnPAVTransport1Service.Pause(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("Pause", arguments);
        }

        /// <summary>
        /// Play.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="speed">Transport play speed. This property is usually "1".</param>
        /// <exception cref="Exception"/>
        void IUPnPAVTransport1Service.Play(uint instanceID, string speed)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "Speed", speed },
            };
            PostAction("Play", arguments);
        }

        /// <summary>
        /// Jump previous.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <exception cref="Exception"/>
        void IUPnPAVTransport1Service.Previous(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("Previous", arguments);
        }

        /// <summary>
        /// Record. whether the device outputs the resource to a screen or speakers while recording is device dependent.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <exception cref="Exception"/>
        void IUPnPAVTransport1Service.Record(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("Record", arguments);
        }

        /// <summary>
        /// Seek. This state variable is introduced to provide type information for the “target” parameter in action “Seek”. It
        /// <br />indicates the target position of the seek action, in terms of units defined by state variable A_ARG_TYPE_SeekMode.
        /// <br />The data type of this variable is ‘string’. However, depending on the actual seek mode used, it must contains
        /// <br />string representations of values of UPnP types ‘ui4’ (ABS_COUNT, REL_COUNT, TRACK_NR, TAPE-INDEX, FRAME),
        /// <br />‘time’ (ABS_TIME, REL_TIME) or ‘float‘ (CHANNEL_FREQ, in Hz). Supported ranges of these integer, time or float
        /// <br />values are device-dependent.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="unit">The seek mode.</param>
        /// <param name="target">Target by seek mode. REL_TIME: 00:33:33, TRACK_NR(Track number of CD-DA): 1.</param>
        /// <exception cref="Exception"/>
        void IUPnPAVTransport1Service.Seek(uint instanceID, string unit, string target)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "Unit", unit },
                { "Target", target },
            };
            PostAction("Seek", arguments);
        }

        /// <summary>
        /// Set audio/video transport uri. Need DLNA http server. Used by "UPnPDlnaServer" or design by youself.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="currentURI">Current audio/video transport uri.</param>
        /// <param name="currentURIMetaData">Current audio/video transport uri meta data.</param>
        /// <exception cref="Exception"/>
        void IUPnPAVTransport1Service.SetAVTransportURI(uint instanceID, string currentURI, string currentURIMetaData)
        {
            currentURI = $"<![CDATA[{currentURI}]]>";
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "CurrentURI", currentURI },
                { "CurrentURIMetaData", currentURIMetaData },
            };
            PostAction("SetAVTransportURI", arguments);
        }

        /// <summary>
        /// Set audio/video transport uri. Need DLNA http server. Used by "UPnPDlnaServer" or design by youself.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="nextURI">Next audio/video transport uri.</param>
        /// <param name="nextURIMetaData">Next audio/video transport uri meta data.</param>
        /// <exception cref="Exception"/>
        void IUPnPAVTransport1Service.SetNextAVTransportURI(uint instanceID, string nextURI, string nextURIMetaData)
        {
            nextURI = $"<![CDATA[{nextURI}]]>";
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "NextURI", nextURI },
                { "NextURIMetaData", nextURIMetaData },
            };
            PostAction("SetNextAVTransportURI", arguments);
        }

        /// <summary>
        /// Set play mode.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="playMode">Current play mode. This property accepts the following: "NORMAL", "SHUFFLE", "REPEAT_ONE", "REPEAT_ALL", "RANDOM", "DIRECT_1", "INTRO", Vendor-defined.
        /// </param>
        /// <exception cref="Exception"/>
        void IUPnPAVTransport1Service.SetPlayMode(uint instanceID, string playMode)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "NewPlayMode", playMode },
            };
            PostAction("SetPlayMode", arguments);
        }

        /// <summary>
        /// Set record quality mode.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="recordQualityMode">Record quality mode. This property accepts the following: "0:EP", "1:LP", "2:SP", "0:BASIC", "1:MEDIUM", "2:HIGH", "NOT_IMPLEMENTED", Vendor-defined.
        /// </param>
        /// <exception cref="Exception"/>
        void IUPnPAVTransport1Service.SetRecordQualityMode(uint instanceID, string recordQualityMode)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "NewRecordQualityMode", recordQualityMode },
            };
            PostAction("SetRecordQualityMode", arguments);
        }

        /// <summary>
        /// Stop.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <exception cref="Exception"/>
        void IUPnPAVTransport1Service.Stop(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("Stop", arguments);
        }

        #endregion AVTransport1

        #region AVTransport2

        /// <summary>
        /// Get DRMState.
        /// <br />This property accepts the following: "OK", "UNKNOWN", "PROCESSING_CONTENT_KEY", "CONTENT_KEY_FAILURE", "ATTEMPTING_AUTHENTICATION", "FAILED_AUTHENTICATION", "NOT_AUTHENTICATED", "DEVICE_REVOCATION".
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string IUPnPAVTransport2Service.GetDRMState(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetDRMState", arguments);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetDRMStateResponse", ns);
            return node.SelectSingleNode("CurrentDRMState").InnerText.Trim();
        }

        /// <summary>
        /// Get media info ext.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPMediaInfoExt IUPnPAVTransport2Service.GetMediaInfoExt(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetMediaInfo_Ext", arguments);
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.LoadXml(response);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.AddNamespace("u", _serviceType);
            XmlNode node = doc.SelectSingleNode("/s:Envelope/s:Body/u:GetMediaInfo_ExtResponse", ns);
            return new UPnPMediaInfoExt(node);
        }

        /// <summary>
        /// Get state variables. Allways throw <see cref="NotImplementedException"/>() because I'm not a AVTransport2 device :(.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="stateVariableList"> If the argument is set to "<see langword="*"/>", the action MUST return all the supported state variables of the service, including the vendor-extended state variables except for LastChange and any A_ARG_TYPE_xxx variables.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"/>
        IDictionary<string, string> IUPnPAVTransport2Service.GetStateVariables(uint instanceID, string stateVariableList)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set state variables. Allways throw <see cref="NotImplementedException"/>() because I'm not a AVTransport2 device :(.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="avTransportUDN"></param>
        /// <param name="serviceType"></param>
        /// <param name="serviceId"></param>
        /// <param name="stateVariableValuePairs"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        string IUPnPAVTransport2Service.SetStateVariables(uint instanceID, string avTransportUDN, string serviceType, string serviceId, IDictionary<string, string> stateVariableValuePairs)
        {
            throw new NotImplementedException();
        }

        #endregion AVTransport2
    }
}