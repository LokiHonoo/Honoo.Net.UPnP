using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Xml.Linq;

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
        IUPnPAVTransport2Service,
        IUPnPConnectionManager1Service,
        IUPnPConnectionManager2Service,
        IUPnPRenderingControl1Service,
        IUPnPRenderingControl2Service

    {
        #region Members

        private readonly string _controlUrl;
        private readonly string _eventSubUrl;
        private readonly UPnPServiceInterfaces _interfaces;
        private readonly UPnPDevice _parentDevice;
        private readonly UPnPRootDevice _rootDevice;
        private readonly string _scpdUrl;
        private readonly string _serviceId;
        private readonly string _serviceType;

        /// <inheritdoc/>
        public string ControlUrl => _controlUrl;

        /// <inheritdoc/>
        public string EventSubUrl => _eventSubUrl;

        /// <summary>
        /// UPnP service interfaces.
        /// </summary>
        public UPnPServiceInterfaces Interfaces => _interfaces;

        /// <inheritdoc/>
        public UPnPDevice ParentDevice => _parentDevice;

        /// <inheritdoc/>
        public UPnPRootDevice RootDevice => _rootDevice;

        /// <inheritdoc/>
        public string ScpdUrl => _scpdUrl;

        /// <inheritdoc/>
        public string ServiceId => _serviceId;

        /// <inheritdoc/>
        public string ServiceType => _serviceType;

        #endregion Members

        #region Construction

        /// <summary>
        /// Initializes a new instance of the UPnPService class.
        /// </summary>
        /// <param name="serviceElement">Service element.</param>
        /// <param name="parentDevice">The UPnPDevice to which the device belongs.</param>
        /// <param name="rootDevice">The UPnPRootDevice to which the root device belongs.</param>
        /// <exception cref="Exception"/>
        internal UPnPService(XElement serviceElement, UPnPDevice parentDevice, UPnPRootDevice rootDevice)
        {
            XNamespace nm = serviceElement.GetDefaultNamespace();
            _controlUrl = serviceElement.Element(nm + "controlURL")?.Value.Trim();
            _eventSubUrl = serviceElement.Element(nm + "eventSubURL")?.Value.Trim();
            _scpdUrl = serviceElement.Element(nm + "SCPDURL")?.Value.Trim();
            _serviceId = serviceElement.Element(nm + "serviceId").Value.Trim();
            _serviceType = serviceElement.Element(nm + "serviceType").Value.Trim();
            _parentDevice = parentDevice;
            _rootDevice = rootDevice;
            _interfaces = new UPnPServiceInterfaces(this);
        }

        #endregion Construction

        #region Common

        /// <inheritdoc/>
        public XElement DecodeResponse(string response, string action)
        {
            XDocument doc = XDocument.Parse(response);
            XNamespace nms = doc.Root.GetNamespaceOfPrefix("s");
            XNamespace nmu = _serviceType;
            return doc.Root.Element(nms + "Body").Element(nmu + (action + "Response"));

            //XmlDocument doc = new XmlDocument() { XmlResolver = null };
            //doc.LoadXml(response);
            //XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            //ns.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            //ns.AddNamespace("u", _serviceType);
            //return doc.SelectSingleNode($"/s:Envelope/s:Body/u:{action}Response", ns);
        }

        /// <inheritdoc/>
        public string GetScpdInformation()
        {
            _rootDevice.Client.Headers.Clear();
            _rootDevice.Client.Headers.Add(_rootDevice.Headers);
            return _rootDevice.Client.DownloadString(_scpdUrl);
        }

        /// <inheritdoc/>
        public string PostAction(string action, IDictionary<string, string> arguments)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<?xml version=\"1.0\"?>");
            sb.AppendLine($"<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            sb.AppendLine($"  <s:Body>");
            sb.AppendLine($"    <u:{action} xmlns:u=\"{_serviceType}\">");
            if (arguments != null && arguments.Count > 0)
            {
                foreach (KeyValuePair<string, string> argument in arguments)
                {
                    sb.AppendLine($"      <{argument.Key}>{argument.Value}</{argument.Key}>");
                }
            }
            sb.AppendLine($"    </u:{action}>");
            sb.AppendLine($"  </s:Body>");
            sb.AppendLine($"</s:Envelope>");
            string body = sb.ToString();
            _rootDevice.Client.Headers.Clear();
            _rootDevice.Client.Headers.Add(_rootDevice.Headers);
            _rootDevice.Client.Headers.Add($"SOAPAction: \"{_serviceType}#{action}\"");
            return _rootDevice.Client.UploadString(_controlUrl, "POST", body);
        }

        /// <inheritdoc/>
        public void RemoveEventSubscription(string sid)
        {
            _rootDevice.Client.Headers.Clear();
            _rootDevice.Client.Headers.Add(_rootDevice.Headers);
            _rootDevice.Client.Headers.Add($"SID: {sid}");
            _rootDevice.Client.UploadString(_eventSubUrl, "UNSUBSCRIBE", string.Empty);
        }

        /// <inheritdoc/>
        public void RenewalEventSubscription(string sid, uint durationSecond)
        {
            _rootDevice.Client.Headers.Clear();
            _rootDevice.Client.Headers.Add(_rootDevice.Headers);
            _rootDevice.Client.Headers.Add($"SID: {sid}");
            _rootDevice.Client.Headers.Add($"TIMEOUT: Second-{durationSecond}");
            _rootDevice.Client.UploadString(_eventSubUrl, "SUBSCRIBE", string.Empty);
        }

        /// <inheritdoc/>
        public string SetEventSubscription(string subscriberUrl, uint durationSecond)
        {
            _rootDevice.Client.Headers.Clear();
            _rootDevice.Client.Headers.Add(_rootDevice.Headers);
            _rootDevice.Client.Headers.Add("NT: upnp:event");
            _rootDevice.Client.Headers.Add($"CALLBACK: <{subscriberUrl}>");
            _rootDevice.Client.Headers.Add($"TIMEOUT: Second-{durationSecond}");
            _rootDevice.Client.UploadString(_eventSubUrl, "SUBSCRIBE", string.Empty);
            return _rootDevice.Client.ResponseHeaders["SID"];
        }

        #endregion Common

        #region WANIPConnection1

        /// <inheritdoc/>
        void IUPnPWANConnectionService.AddPortMapping(string protocol,
                                                      string remoteHost,
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
                { "NewRemoteHost", remoteHost },
                { "NewExternalPort", externalPort.ToString(CultureInfo.InvariantCulture) },
                { "NewInternalClient", internalClient.ToString() },
                { "NewInternalPort", internalPort.ToString(CultureInfo.InvariantCulture) },
                { "NewEnabled", enabled ? "1" : "0" },
                { "NewPortMappingDescription", description },
                { "NewLeaseDuration", leaseDuration.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("AddPortMapping", arguments);
        }

        /// <inheritdoc/>
        void IUPnPWANConnectionService.DeletePortMapping(string protocol, string remoteHost, ushort externalPort)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewProtocol", protocol },
                { "NewRemoteHost", remoteHost },
                { "NewExternalPort", externalPort.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("DeletePortMapping", arguments);
        }

        /// <inheritdoc/>
        void IUPnPWANConnectionService.ForceTermination()
        {
            PostAction("ForceTermination", null);
        }

        /// <inheritdoc/>
        uint IUPnPWANConnectionService.GetAutoDisconnectTime()
        {
            string response = PostAction("GetAutoDisconnectTime", null);
            XElement element = DecodeResponse(response, "GetAutoDisconnectTime");
            return uint.Parse(element.Element("NewAutoDisconnectTime").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        UPnPConnectionTypeInfo IUPnPWANConnectionService.GetConnectionTypeInfo()
        {
            string response = PostAction("GetConnectionTypeInfo", null);
            XElement element = DecodeResponse(response, "GetConnectionTypeInfo");
            return new UPnPConnectionTypeInfo(element);
        }

        /// <inheritdoc/>
        string IUPnPWANConnectionService.GetExternalIPAddress()
        {
            string response = PostAction("GetExternalIPAddress", null);
            XElement element = DecodeResponse(response, "GetExternalIPAddress");
            return element.Element("NewExternalIPAddress").Value.Trim();
        }

        /// <inheritdoc/>
        UPnPPortMappingEntry IUPnPWANConnectionService.GetGenericPortMappingEntry(uint index)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewPortMappingIndex", index.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetGenericPortMappingEntry", arguments);
            XElement element = DecodeResponse(response, "GetGenericPortMappingEntry");
            return new UPnPPortMappingEntry(element);
        }

        /// <inheritdoc/>
        uint IUPnPWANConnectionService.GetIdleDisconnectTime()
        {
            string response = PostAction("GetIdleDisconnectTime", null);
            XElement element = DecodeResponse(response, "GetIdleDisconnectTime");
            return uint.Parse(element.Element("NewIdleDisconnectTime").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        UPnPNatRsipStatus IUPnPWANConnectionService.GetNATRSIPStatus()
        {
            string response = PostAction("GetNATRSIPStatus", null);
            XElement element = DecodeResponse(response, "GetNATRSIPStatus");
            return new UPnPNatRsipStatus(element);
        }

        /// <inheritdoc/>
        UPnPPortMappingEntry IUPnPWANConnectionService.GetSpecificPortMappingEntry(string protocol, string remoteHost, ushort externalPort)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewProtocol", protocol },
                { "NewRemoteHost", remoteHost },
                { "NewExternalPort", externalPort.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetSpecificPortMappingEntry", arguments);
            XElement element = DecodeResponse(response, "GetSpecificPortMappingEntry");
            return new UPnPPortMappingEntry(protocol, string.Empty, externalPort, element);
        }

        /// <inheritdoc/>
        UPnPStatusInfo IUPnPWANConnectionService.GetStatusInfo()
        {
            string response = PostAction("GetStatusInfo", null);
            XElement element = DecodeResponse(response, "GetStatusInfo");
            return new UPnPStatusInfo(element);
        }

        /// <inheritdoc/>
        uint IUPnPWANConnectionService.GetWarnDisconnectDelay()
        {
            string response = PostAction("GetWarnDisconnectDelay", null);
            XElement element = DecodeResponse(response, "GetWarnDisconnectDelay");
            return uint.Parse(element.Element("NewWarnDisconnectDelay").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        void IUPnPWANConnectionService.RequestConnection()
        {
            PostAction("RequestConnection", null);
        }

        /// <inheritdoc/>
        void IUPnPWANConnectionService.RequestTermination()
        {
            PostAction("RequestTermination", null);
        }

        /// <inheritdoc/>
        void IUPnPWANConnectionService.SetAutoDisconnectTime(uint seconds)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewAutoDisconnectTime", seconds.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetAutoDisconnectTime", arguments);
        }

        /// <inheritdoc/>
        void IUPnPWANConnectionService.SetConnectionType(string connectionType)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewConnectionType", connectionType },
            };
            PostAction("SetConnectionType", arguments);
        }

        /// <inheritdoc/>
        void IUPnPWANConnectionService.SetIdleDisconnectTime(uint seconds)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewIdleDisconnectTime", seconds.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetIdleDisconnectTime", arguments);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        ushort IUPnPWANIPConnection2Service.AddAnyPortMapping(string protocol,
                                                              string remoteHost,
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
                { "NewRemoteHost", remoteHost },
                { "NewExternalPort", externalPort.ToString(CultureInfo.InvariantCulture) },
                { "NewInternalClient", internalClient.ToString() },
                { "NewInternalPort", internalPort.ToString(CultureInfo.InvariantCulture) },
                { "NewEnabled", enabled ? "1" : "0" },
                { "NewPortMappingDescription", description },
                { "NewLeaseDuration", leaseDuration.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("AddAnyPortMapping", arguments);
            XElement element = DecodeResponse(response, "AddAnyPortMapping");
            return ushort.Parse(element.Element("NewReservedPort").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        string IUPnPWANIPConnection2Service.GetListOfPortMappings(string protocol, ushort startPort, ushort endPort, bool manage, ushort maxCount)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "NewProtocol", protocol },
                { "NewStartPort", startPort.ToString(CultureInfo.InvariantCulture) },
                { "NewEndPort", endPort.ToString(CultureInfo.InvariantCulture) },
                { "NewManage", manage ? "1" : "0" },
                { "NewNumberOfPorts", maxCount.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetListOfPortMappings", arguments);
            XElement element = DecodeResponse(response, "GetListOfPortMappings");
            return element.Element("NewPortListing").Value.Trim();
        }

        #endregion WANIPConnection2

        #region WANPPPConnection1

        /// <inheritdoc/>
        UPnPLinkLayerMaxBitRates IUPnPWANPPPConnection1Service.GetLinkLayerMaxBitRates()
        {
            string response = PostAction("GetLinkLayerMaxBitRates", null);
            XElement element = DecodeResponse(response, "GetLinkLayerMaxBitRates");
            return new UPnPLinkLayerMaxBitRates(element);
        }

        /// <inheritdoc/>
        string IUPnPWANPPPConnection1Service.GetPassword()
        {
            string response = PostAction("GetPassword", null);
            XElement element = DecodeResponse(response, "GetPassword");
            return element.Element("NewPassword").Value.Trim();
        }

        /// <inheritdoc/>
        string IUPnPWANPPPConnection1Service.GetPPPAuthenticationProtocol()
        {
            string response = PostAction("GetPPPAuthenticationProtocol", null);
            XElement element = DecodeResponse(response, "GetPPPAuthenticationProtocol");
            return element.Element("NewPPPAuthenticationProtocol").Value.Trim();
        }

        /// <inheritdoc/>
        string IUPnPWANPPPConnection1Service.GetPPPCompressionProtocol()
        {
            string response = PostAction("GetPPPCompressionProtocol", null);
            XElement element = DecodeResponse(response, "GetPPPCompressionProtocol");
            return element.Element("NewPPPCompressionProtocol").Value.Trim();
        }

        /// <inheritdoc/>
        string IUPnPWANPPPConnection1Service.GetPPPEncryptionProtocol()
        {
            string response = PostAction("GetPPPEncryptionProtocol", null);
            XElement element = DecodeResponse(response, "GetPPPEncryptionProtocol");
            return element.Element("NewPPPEncryptionProtocol").Value.Trim();
        }

        /// <inheritdoc/>
        string IUPnPWANPPPConnection1Service.GetUserName()
        {
            string response = PostAction("GetUserName", null);
            XElement element = DecodeResponse(response, "GetUserName");
            return element.Element("NewUserName").Value.Trim();
        }

        #endregion WANPPPConnection1

        #region AVTransport1

        /// <inheritdoc/>
        string IUPnPAVTransport1Service.GetCurrentTransportActions(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetCurrentTransportActions", arguments);
            XElement element = DecodeResponse(response, "GetCurrentTransportActions");
            return element.Element("Actions").Value.Trim();
        }

        /// <inheritdoc/>
        UPnPDeviceCapabilities IUPnPAVTransport1Service.GetDeviceCapabilities(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetDeviceCapabilities", arguments);
            XElement element = DecodeResponse(response, "GetDeviceCapabilities");
            return new UPnPDeviceCapabilities(element);
        }

        /// <inheritdoc/>
        UPnPMediaInfo IUPnPAVTransport1Service.GetMediaInfo(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetMediaInfo", arguments);
            XElement element = DecodeResponse(response, "GetMediaInfo");
            return new UPnPMediaInfo(element);
        }

        /// <inheritdoc/>
        UPnPPositionInfo IUPnPAVTransport1Service.GetPositionInfo(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetPositionInfo", arguments);
            XElement element = DecodeResponse(response, "GetPositionInfo");
            return new UPnPPositionInfo(element);
        }

        /// <inheritdoc/>
        UPnPTransportInfo IUPnPAVTransport1Service.GetTransportInfo(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetTransportInfo", arguments);
            XElement element = DecodeResponse(response, "GetTransportInfo");
            return new UPnPTransportInfo(element);
        }

        /// <inheritdoc/>
        UPnPTransportSettings IUPnPAVTransport1Service.GetTransportSettings(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetTransportSettings", arguments);
            XElement element = DecodeResponse(response, "GetTransportSettings");
            return new UPnPTransportSettings(element);
        }

        /// <inheritdoc/>
        void IUPnPAVTransport1Service.Next(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("Next", arguments);
        }

        /// <inheritdoc/>
        void IUPnPAVTransport1Service.Pause(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("Pause", arguments);
        }

        /// <inheritdoc/>
        void IUPnPAVTransport1Service.Play(uint instanceID, string speed)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "Speed", speed },
            };
            PostAction("Play", arguments);
        }

        /// <inheritdoc/>
        void IUPnPAVTransport1Service.Previous(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("Previous", arguments);
        }

        /// <inheritdoc/>
        void IUPnPAVTransport1Service.Record(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("Record", arguments);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        void IUPnPAVTransport1Service.SetPlayMode(uint instanceID, string playMode)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "NewPlayMode", playMode },
            };
            PostAction("SetPlayMode", arguments);
        }

        /// <inheritdoc/>
        void IUPnPAVTransport1Service.SetRecordQualityMode(uint instanceID, string recordQualityMode)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "NewRecordQualityMode", recordQualityMode },
            };
            PostAction("SetRecordQualityMode", arguments);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        string IUPnPAVTransport2Service.GetDRMState(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetDRMState", arguments);
            XElement element = DecodeResponse(response, "GetDRMState");
            return element.Element("CurrentDRMState").Value.Trim();
        }

        /// <inheritdoc/>
        UPnPMediaInfoExt IUPnPAVTransport2Service.GetMediaInfoExt(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetMediaInfo_Ext", arguments);
            XElement element = DecodeResponse(response, "GetMediaInfo_Ext");
            return new UPnPMediaInfoExt(element);
        }

        /// <inheritdoc/>
        IDictionary<string, string> IUPnPAVTransport2Service.GetStateVariables(uint instanceID, string stateVariableList)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "StateVariableList", stateVariableList },
            };
            string response = PostAction("GetStateVariables", arguments);
            XElement element = DecodeResponse(response, "GetStateVariables");
            string stateVariableValuePairs = element.Element("StateVariableValuePairs").Value.Trim();
            stateVariableValuePairs = stateVariableValuePairs.Replace("&lt;", "<").Replace("&quot;", "\"").Replace("&gt;", ">");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            XDocument doc = XDocument.Parse(stateVariableValuePairs);
            XNamespace nm = doc.Root.GetDefaultNamespace();
            foreach (XElement stateVariable in doc.Descendants(nm + "stateVariable"))
            {
                string name = stateVariable.Attribute("variableName").Value.Trim();
                string value = stateVariable.Value.Trim();
                dictionary.Add(name, value);
            }
            return dictionary;
        }

        /// <inheritdoc/>
        string IUPnPAVTransport2Service.SetStateVariables(uint instanceID, string avTransportUDN, string serviceType, string serviceId, IDictionary<string, string> stateVariableValuePairs)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<stateVariableValuePairs xmlns=\"urn:schemas-upnp-org:av:avs\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"urn:schemas-upnp-org:av:avs http://www.upnp.org/schemas/av/avs.xsd\">");
            if (stateVariableValuePairs != null && stateVariableValuePairs.Count > 0)
            {
                foreach (KeyValuePair<string, string> stateVariableValuePair in stateVariableValuePairs)
                {
                    sb.AppendLine($"  <stateVariable variableName=\"{stateVariableValuePair.Key}\">{stateVariableValuePair.Value}</stateVariable>");
                }
            }
            sb.AppendLine($"</stateVariableValuePairs>");
            string svvps = sb.ToString().Replace("<", "&lt;").Replace("\"", "&quot;").Replace(">", "&gt;");
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "AVTransportUDN", avTransportUDN },
                { "ServiceType", serviceType },
                { "ServiceId", serviceId },
                { "StateVariableValuePairs", svvps },
            };
            string response = PostAction("SetStateVariables", arguments);
            XElement element = DecodeResponse(response, "SetStateVariables");
            return element.Element("StateVariableList").Value.Trim();
        }

        #endregion AVTransport2

        #region ConnectionManager1

        /// <inheritdoc/>
        void IUPnPConnectionManager1Service.ConnectionComplete(uint connectionID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "ConnectionID", connectionID.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("ConnectionComplete", arguments);
        }

        /// <inheritdoc/>
        string IUPnPConnectionManager1Service.GetCurrentConnectionIDs()
        {
            string response = PostAction("GetCurrentConnectionIDs", null);
            XElement element = DecodeResponse(response, "GetCurrentConnectionIDs");
            return element.Element("CurrentConnectionIDs").Value.Trim();
        }

        /// <inheritdoc/>
        UPnPAVConnectionInfo IUPnPConnectionManager1Service.GetCurrentConnectionInfo(uint connectionID)
        {
            string response = PostAction("GetCurrentConnectionInfo", null);
            XElement element = DecodeResponse(response, "GetCurrentConnectionInfo");
            return new UPnPAVConnectionInfo(element);
        }

        /// <inheritdoc/>
        UPnPProtocolInfo IUPnPConnectionManager1Service.GetProtocolInfo()
        {
            string response = PostAction("GetProtocolInfo", null);
            XElement element = DecodeResponse(response, "GetProtocolInfo");
            return new UPnPProtocolInfo(element);
        }

        /// <inheritdoc/>
        UPnPAVPrepareConnectionInfo IUPnPConnectionManager1Service.PrepareForConnection(string remoteProtocolInfo, string peerConnectionManager, int peerConnectionID, string direction)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "RemoteProtocolInfo", remoteProtocolInfo },
                { "PeerConnectionManager", peerConnectionManager },
                { "PeerConnectionID", peerConnectionID.ToString(CultureInfo.InvariantCulture) },
                { "Direction", direction },
            };
            string response = PostAction("PrepareForConnection", arguments);
            XElement element = DecodeResponse(response, "PrepareForConnection");
            return new UPnPAVPrepareConnectionInfo(element);
        }

        #endregion ConnectionManager1

        #region RenderingControl1

        /// <inheritdoc/>
        ushort IUPnPRenderingControl1Service.GetBlueVideoBlackLevel(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetBlueVideoBlackLevel", arguments);
            XElement element = DecodeResponse(response, "GetBlueVideoBlackLevel");
            return ushort.Parse(element.Element("CurrentBlueVideoBlackLevel").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        ushort IUPnPRenderingControl1Service.GetBlueVideoGain(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetBlueVideoGain", arguments);
            XElement element = DecodeResponse(response, "GetBlueVideoGain");
            return ushort.Parse(element.Element("CurrentBlueVideoGain").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        ushort IUPnPRenderingControl1Service.GetBrightness(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetBrightness", arguments);
            XElement element = DecodeResponse(response, "GetBrightness");
            return ushort.Parse(element.Element("CurrentBrightness").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        ushort IUPnPRenderingControl1Service.GetColorTemperature(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetColorTemperature", arguments);
            XElement element = DecodeResponse(response, "GetColorTemperature");
            return ushort.Parse(element.Element("CurrentColorTemperature").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        ushort IUPnPRenderingControl1Service.GetContrast(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetContrast", arguments);
            XElement element = DecodeResponse(response, "GetContrast");
            return ushort.Parse(element.Element("CurrentContrast").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        ushort IUPnPRenderingControl1Service.GetGreenVideoBlackLevel(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetGreenVideoBlackLevel", arguments);
            XElement element = DecodeResponse(response, "GetGreenVideoBlackLevel");
            return ushort.Parse(element.Element("CurrentGreenVideoBlackLevel").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        ushort IUPnPRenderingControl1Service.GetGreenVideoGain(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetGreenVideoGain", arguments);
            XElement element = DecodeResponse(response, "GetGreenVideoGain");
            return ushort.Parse(element.Element("CurrentGreenVideoGain").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        short IUPnPRenderingControl1Service.GetHorizontalKeystone(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetHorizontalKeystone", arguments);
            XElement element = DecodeResponse(response, "GetHorizontalKeystone");
            return short.Parse(element.Element("CurrentHorizontalKeystone").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        bool IUPnPRenderingControl1Service.GetLoudness(uint instanceID, string channel)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "Channel", channel },
            };
            string response = PostAction("GetLoudness", arguments);
            XElement element = DecodeResponse(response, "GetLoudness");
            return Convert.ToBoolean(int.Parse(element.Element("CurrentLoudness").Value.Trim(), CultureInfo.InvariantCulture));
        }

        /// <inheritdoc/>
        bool IUPnPRenderingControl1Service.GetMute(uint instanceID, string channel)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "Channel", channel },
            };
            string response = PostAction("GetMute", arguments);
            XElement element = DecodeResponse(response, "GetMute");
            return Convert.ToBoolean(int.Parse(element.Element("CurrentMute").Value.Trim(), CultureInfo.InvariantCulture));
        }

        /// <inheritdoc/>
        ushort IUPnPRenderingControl1Service.GetRedVideoBlackLevel(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetRedVideoBlackLevel", arguments);
            XElement element = DecodeResponse(response, "GetRedVideoBlackLevel");
            return ushort.Parse(element.Element("CurrentRedVideoBlackLevel").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        ushort IUPnPRenderingControl1Service.GetRedVideoGain(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetRedVideoGain", arguments);
            XElement element = DecodeResponse(response, "GetRedVideoGain");
            return ushort.Parse(element.Element("CurrentRedVideoGain").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        ushort IUPnPRenderingControl1Service.GetSharpness(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetSharpness", arguments);
            XElement element = DecodeResponse(response, "GetSharpness");
            return ushort.Parse(element.Element("CurrentSharpness").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        short IUPnPRenderingControl1Service.GetVerticalKeystone(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("GetVerticalKeystone", arguments);
            XElement element = DecodeResponse(response, "GetVerticalKeystone");
            return short.Parse(element.Element("CurrentVerticalKeystone").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        ushort IUPnPRenderingControl1Service.GetVolume(uint instanceID, string channel)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "Channel", channel },
            };
            string response = PostAction("GetVolume", arguments);
            XElement element = DecodeResponse(response, "GetVolume");
            return ushort.Parse(element.Element("CurrentVolume").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        short IUPnPRenderingControl1Service.GetVolumeDB(uint instanceID, string channel)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "Channel", channel },
            };
            string response = PostAction("GetVolumeDB", arguments);
            XElement element = DecodeResponse(response, "GetVolumeDB");
            return short.Parse(element.Element("CurrentVolume").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        UPnPVolumeDBRange IUPnPRenderingControl1Service.GetVolumeDBRange(uint instanceID, string channel)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "Channel", channel },
            };
            string response = PostAction("GetVolumeDBRange", arguments);
            XElement element = DecodeResponse(response, "GetVolumeDBRange");
            return new UPnPVolumeDBRange(element);
        }

        /// <inheritdoc/>
        string IUPnPRenderingControl1Service.ListPresets(uint instanceID)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
            };
            string response = PostAction("ListPresets", arguments);
            XElement element = DecodeResponse(response, "ListPresets");
            return element.Element("CurrentPresetNameList").Value.Trim();
        }

        /// <inheritdoc/>
        void IUPnPRenderingControl1Service.SelectPreset(uint instanceID, string presetName)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "PresetName", presetName },
            };
            PostAction("SelectPreset", arguments);
        }

        /// <inheritdoc/>
        void IUPnPRenderingControl1Service.SetBlueVideoBlackLevel(uint instanceID, ushort blueVideoBlackLevel)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "DesiredBlueVideoBlackLevel", blueVideoBlackLevel.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetBlueVideoBlackLevel", arguments);
        }

        /// <inheritdoc/>
        void IUPnPRenderingControl1Service.SetBlueVideoGain(uint instanceID, ushort blueVideoGain)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "DesiredBlueVideoGain", blueVideoGain.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetBlueVideoGain", arguments);
        }

        /// <inheritdoc/>
        void IUPnPRenderingControl1Service.SetBrightness(uint instanceID, ushort brightness)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "DesiredBrightness", brightness.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetBrightness", arguments);
        }

        /// <inheritdoc/>
        void IUPnPRenderingControl1Service.SetColorTemperature(uint instanceID, ushort colorTemperature)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "DesiredColorTemperature", colorTemperature.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetColorTemperature", arguments);
        }

        /// <inheritdoc/>
        void IUPnPRenderingControl1Service.SetContrast(uint instanceID, ushort contrast)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "DesiredContrast", contrast.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetContrast", arguments);
        }

        /// <inheritdoc/>
        void IUPnPRenderingControl1Service.SetGreenVideoBlackLevel(uint instanceID, ushort greenVideoBlackLevel)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "DesiredGreenVideoBlackLevel", greenVideoBlackLevel.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetGreenVideoBlackLevel", arguments);
        }

        /// <inheritdoc/>
        void IUPnPRenderingControl1Service.SetGreenVideoGain(uint instanceID, ushort greenVideoGain)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "DesiredGreenVideoGain", greenVideoGain.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetGreenVideoGain", arguments);
        }

        /// <inheritdoc/>
        void IUPnPRenderingControl1Service.SetHorizontalKeystone(uint instanceID, short horizontalKeystone)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "DesiredHorizontalKeystone", horizontalKeystone.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetHorizontalKeystone", arguments);
        }

        /// <inheritdoc/>
        void IUPnPRenderingControl1Service.SetLoudness(uint instanceID, string channel, bool loudness)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "Channel", channel },
                { "DesiredLoudness", loudness? "1" : "0" },
            };
            PostAction("SetLoudness", arguments);
        }

        /// <inheritdoc/>
        void IUPnPRenderingControl1Service.SetMute(uint instanceID, string channel, bool mute)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "Channel", channel },
                { "DesiredMute", mute? "1" : "0" },
            };
            PostAction("SetMute", arguments);
        }

        /// <inheritdoc/>
        void IUPnPRenderingControl1Service.SetRedVideoBlackLevel(uint instanceID, ushort redVideoBlackLevel)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "DesiredRedVideoBlackLevel", redVideoBlackLevel.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetRedVideoBlackLevel", arguments);
        }

        /// <inheritdoc/>
        void IUPnPRenderingControl1Service.SetRedVideoGain(uint instanceID, ushort redVideoGain)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "DesiredRedVideoGain", redVideoGain.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetRedVideoGain", arguments);
        }

        /// <inheritdoc/>
        void IUPnPRenderingControl1Service.SetSharpness(uint instanceID, ushort sharpness)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "DesiredSharpness", sharpness.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetSharpness", arguments);
        }

        /// <inheritdoc/>
        void IUPnPRenderingControl1Service.SetVerticalKeystone(uint instanceID, short verticalKeystone)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "DesiredVerticalKeystone", verticalKeystone.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetVerticalKeystone", arguments);
        }

        /// <inheritdoc/>
        void IUPnPRenderingControl1Service.SetVolume(uint instanceID, string channel, ushort volume)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "Channel", channel },
                { "DesiredVolume", volume.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetVolume", arguments);
        }

        /// <inheritdoc/>
        void IUPnPRenderingControl1Service.SetVolumeDB(uint instanceID, string channel, short volume)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "Channel", channel },
                { "DesiredVolume", volume.ToString(CultureInfo.InvariantCulture) },
            };
            PostAction("SetVolumeDB", arguments);
        }

        #endregion RenderingControl1

        #region RenderingControl2

        /// <inheritdoc/>
        IDictionary<string, string> IUPnPRenderingControl2Service.GetStateVariables(uint instanceID, string stateVariableList)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "StateVariableList", stateVariableList },
            };
            string response = PostAction("GetStateVariables", arguments);
            XElement element = DecodeResponse(response, "GetStateVariables");
            string stateVariableValuePairs = element.Element("StateVariableValuePairs").Value.Trim();
            stateVariableValuePairs = stateVariableValuePairs.Replace("&lt;", "<").Replace("&quot;", "\"").Replace("&gt;", ">");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            XDocument doc = XDocument.Parse(stateVariableValuePairs);
            XNamespace nm = doc.Root.GetDefaultNamespace();
            foreach (XElement stateVariable in doc.Descendants(nm + "stateVariable"))
            {
                string name = stateVariable.Attribute("variableName").Value.Trim();
                string value = stateVariable.Value.Trim();
                dictionary.Add(name, value);
            }
            return dictionary;
        }

        /// <inheritdoc/>
        string IUPnPRenderingControl2Service.SetStateVariables(uint instanceID, string renderingControlUDN, string serviceType, string serviceId, IDictionary<string, string> stateVariableValuePairs)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<stateVariableValuePairs xmlns=\"urn:schemas-upnp-org:av:avs\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"urn:schemas-upnp-org:av:avs http://www.upnp.org/schemas/av/avs.xsd\">");
            if (stateVariableValuePairs != null && stateVariableValuePairs.Count > 0)
            {
                foreach (KeyValuePair<string, string> stateVariableValuePair in stateVariableValuePairs)
                {
                    sb.AppendLine($"  <stateVariable variableName=\"{stateVariableValuePair.Key}\">{stateVariableValuePair.Value}</stateVariable>");
                }
            }
            sb.AppendLine($"</stateVariableValuePairs>");
            string svvps = sb.ToString().Replace("<", "&lt;").Replace("\"", "&quot;").Replace(">", "&gt;");
            Dictionary<string, string> arguments = new Dictionary<string, string>
            {
                { "InstanceID", instanceID.ToString(CultureInfo.InvariantCulture) },
                { "RenderingControlUDN", renderingControlUDN },
                { "ServiceType", serviceType },
                { "ServiceId", serviceId },
                { "StateVariableValuePairs", svvps },
            };
            string response = PostAction("SetStateVariables", arguments);
            XElement element = DecodeResponse(response, "SetStateVariables");
            return element.Element("StateVariableList").Value.Trim();
        }

        #endregion RenderingControl2
    }
}