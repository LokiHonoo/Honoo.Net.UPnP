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
        IUPnPWANConnectionCommonService,
        IUPnPWANIPConnection1Service,
        IUPnPWANIPConnection2Service,
        IUPnPWANPPPConnection1Service,
        IUPnPAVTransport1Service
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

        #region PortMapping

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
        void IUPnPWANConnectionCommonService.AddPortMapping(string protocol,
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
        void IUPnPWANConnectionCommonService.DeletePortMapping(string protocol, ushort externalPort)
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
        /// Force termination.
        /// </summary>
        /// <exception cref="Exception"/>
        void IUPnPWANConnectionCommonService.ForceTermination()
        {
            PostAction("ForceTermination", null);
        }

        /// <summary>
        /// Get Connection type info. Possible types maybe wrong because I don't know what the separator is. :(
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPConnectionTypeInfo IUPnPWANConnectionCommonService.GetConnectionTypeInfo()
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
        string IUPnPWANConnectionCommonService.GetExternalIPAddress()
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
        UPnPPortMappingEntry IUPnPWANConnectionCommonService.GetGenericPortMappingEntry(uint index)
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

        /// <summary>
        /// Get NAT RSIP status.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPNatRsipStatus IUPnPWANConnectionCommonService.GetNATRSIPStatus()
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
        UPnPPortMappingEntry IUPnPWANConnectionCommonService.GetSpecificPortMappingEntry(string protocol, ushort externalPort)
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
        UPnPStatusInfo IUPnPWANConnectionCommonService.GetStatusInfo()
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
        /// Request connection.
        /// </summary>
        /// <exception cref="Exception"/>
        void IUPnPWANConnectionCommonService.RequestConnection()
        {
            PostAction("RequestConnection", null);
        }

        /// <summary>
        /// Set connection type.
        /// </summary>
        /// <param name="connectionType">The connection type. This property accepts the following: "Unconfigured", "IP_Routed", "IP_Bridged".</param>
        /// <exception cref="Exception"/>
        void IUPnPWANConnectionCommonService.SetConnectionType(string connectionType)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewConnectionType", connectionType },
            };
            PostAction("SetConnectionType", arguments);
        }

        #endregion PortMapping

        #region DLNA

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
        /// Seek.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="unit">The seek mode. This property accepts the following: "REL_TIME", "TRACK_NR".</param>
        /// <param name="target">Target by seek mode. for "REL_TIME" 00:33:33, for "TRACK_NR" 4.</param>
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
        /// Set audio/video transport uri. Need DLNA http server. Use "UPnPDlnaServer" or design by youself.
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
        /// Set play mode.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="playMode"> Current play mode. This property accepts the following:
        /// "NORMAL", "REPEAT_ONE", "REPEAT_ALL", "SHUFFLE", "SHUFFLE_NOREPEAT".</param>
        /// <exception cref="Exception"/>
        void IUPnPAVTransport1Service.SetPlayMode(uint instanceID, string playMode)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "PlayMode", playMode },
            };
            PostAction("SetPlayMode", arguments);
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

        #endregion DLNA
    }
}