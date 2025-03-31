using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP discoverer.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
    public static class UPnP
    {
        /// <summary></summary>
        public const string SSDP_ALL = "ssdp:all";

        /// <summary></summary>
        public const string UPNP_ROOT_DEVICE = "upnp:rootdevice";

        /// <summary></summary>
        public const string URN_MICROSOFT_SERVICE_OS_INFO_1 = "urn:schemas-microsoft-com:service:OSInfo:1";

        /// <summary></summary>
        public const string URN_MICROSOFT_SERVICE_WAN_IPV6_FIREWALL_CONTROL_1 = "urn:schemas-microsoft-com:service:WANIPv6FirewallControl:1";

        /// <summary></summary>
        public const string URN_MULTISCREEN_SERVICE_DIAL_1 = "urn:dial-multiscreen-org:service:dial:1";

        /// <summary></summary>
        public const string URN_SONY_SERVICE_IRCC_1 = "urn:schemas-sony-com:service:IRCC:1";

        /// <summary></summary>
        public const string URN_SONY_SERVICE_SCALAR_WEB_API_1 = "urn:schemas-sony-com:service:ScalarWebAPI:1";

        /// <summary></summary>
        public const string URN_TENCENT_SERVICE_QPLAY_1 = "urn:schemas-tencent-com:service:QPlay:1";

        /// <summary></summary>
        public const string URN_UPNP_DEVICE_INTERNET_GATEWAY_DEVICE_1 = "urn:schemas-upnp-org:device:InternetGatewayDevice:1";

        /// <summary></summary>
        public const string URN_UPNP_DEVICE_INTERNET_GATEWAY_DEVICE_2 = "urn:schemas-upnp-org:device:InternetGatewayDevice:2";

        /// <summary></summary>
        public const string URN_UPNP_DEVICE_MEDIA_RENDERER_1 = "urn:schemas-upnp-org:device:MediaRenderer:1";

        /// <summary></summary>
        public const string URN_UPNP_DEVICE_WAN_CONNECTION_DEVICE_1 = "urn:schemas-upnp-org:device:WANConnectionDevice:1";

        /// <summary></summary>
        public const string URN_UPNP_DEVICE_WAN_CONNECTION_DEVICE_2 = "urn:schemas-upnp-org:device:WANConnectionDevice:2";

        /// <summary></summary>
        public const string URN_UPNP_DEVICE_WAN_DEVICE_1 = "urn:schemas-upnp-org:device:WANDevice:1";

        /// <summary></summary>
        public const string URN_UPNP_SERVICE_AV_TRANSPORT_1 = "urn:schemas-upnp-org:service:AVTransport:1";

        /// <summary></summary>
        public const string URN_UPNP_SERVICE_CONNECTION_MANAGER_1 = "urn:schemas-upnp-org:service:ConnectionManager:1";

        /// <summary></summary>
        public const string URN_UPNP_SERVICE_DEVICE_PROTECTION_1 = "urn:schemas-upnp-org:service:DeviceProtection:1";

        /// <summary></summary>
        public const string URN_UPNP_SERVICE_LAN_HOST_CONFIG_MANAGEMENT_1 = "urn:schemas-upnp-org:service:LANHostConfigManagement:1";

        /// <summary></summary>
        public const string URN_UPNP_SERVICE_LAYER_3_FORWARDING_1 = "urn:schemas-upnp-org:service:Layer3Forwarding:1";

        /// <summary></summary>
        public const string URN_UPNP_SERVICE_PRIVATE_SERVER_1 = "urn:schemas-upnp-org:service:PrivateServer:1";

        /// <summary></summary>
        public const string URN_UPNP_SERVICE_RENDERING_CONTROL_1 = "urn:schemas-upnp-org:service:RenderingControl:1";

        /// <summary></summary>
        public const string URN_UPNP_SERVICE_WAN_CABLE_LINK_CONFIG_1 = "urn:schemas-upnp-org:service:WANCableLinkConfig:1";

        /// <summary></summary>
        public const string URN_UPNP_SERVICE_WAN_COMMON_INTERFACE_CONFIG_1 = "urn:schemas-upnp-org:service:WANCommonInterfaceConfig:1";

        /// <summary></summary>
        public const string URN_UPNP_SERVICE_WAN_DSL_LINK_CONFIG_1 = "urn:schemas-upnp-org:service:WANDSLLinkConfig:1";

        /// <summary></summary>
        public const string URN_UPNP_SERVICE_WAN_ETHERNET_LINK_CONFIG_1 = "urn:schemas-upnp-org:service:WANEthernetLinkConfig:1";

        /// <summary></summary>
        public const string URN_UPNP_SERVICE_WAN_IP_CONNECTION_1 = "urn:schemas-upnp-org:service:WANIPConnection:1";

        /// <summary></summary>
        public const string URN_UPNP_SERVICE_WAN_IP_CONNECTION_2 = "urn:schemas-upnp-org:service:WANIPConnection:2";

        /// <summary></summary>
        public const string URN_UPNP_SERVICE_WAN_POTS_LINK_CONFIG_1 = "urn:schemas-upnp-org:service:WANPOTSLinkConfig:1";

        /// <summary></summary>
        public const string URN_UPNP_SERVICE_WAN_PPP_CONNECTION_1 = "urn:schemas-upnp-org:service:WANPPPConnection:1";

        /// <summary>
        /// Create a new DLNA media server. Need setup port open for firewall. Administrator privileges are required.
        /// </summary>
        /// <param name="localHost">Create HttpListener by the local host used external address:port. e.g. <see langword="http://192.168.1.100:8080"/>.</param>
        /// <returns></returns>
        public static UPnPDlnaServer CreateDlnaServer(Uri localHost)
        {
            return new UPnPDlnaServer(localHost);
        }

        /// <summary>
        /// Create a new event subscribing server. Need setup port open for firewall. Administrator privileges are required.
        /// </summary>
        /// <param name="localHost">Create HttpListener by the local host used external address:port. e.g. <see langword="http://192.168.1.100:8080"/>.</param>
        /// <returns></returns>
        public static UPnPEventServer CreateEventServer(Uri localHost)
        {
            return new UPnPEventServer(localHost);
        }

        /// <summary>
        /// Discover UPnP devices.
        /// </summary>
        /// <param name="timeout">Response timeout.</param>
        /// <param name="searchTarget">
        /// Http header "ST". Look this: <see cref="UPNP_ROOT_DEVICE"/> and so on.
        /// <br/>Can used URN string as "urn:schemas-upnp-org:service:WANIPConnection:1".
        /// <br/>Can used UUID string as "uuid:xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx".
        /// </param>
        /// <returns></returns>
        public static UPnPRootDevice[] Discover(TimeSpan timeout, string searchTarget = UPNP_ROOT_DEVICE)
        {
            if (timeout.Milliseconds < 1000)
            {
                timeout = TimeSpan.FromMilliseconds(1000);
            }
            if (string.IsNullOrWhiteSpace(searchTarget))
            {
                throw new ArgumentException($"“{nameof(searchTarget)}”cannot null or white space.", nameof(searchTarget));
            }
            List<UPnPRootDevice> rootDevices = new List<UPnPRootDevice>();
            List<string> responses = new List<string>();
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                StringBuilder request = new StringBuilder();
                request.AppendLine("M-SEARCH * HTTP/1.1");
                request.AppendLine("HOST: 239.255.255.250:1900");
                request.AppendLine("Cache-Control: no-store");
                request.AppendLine("Pragma: no-cache");
                request.AppendLine("User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
                request.AppendLine("MAN: \"ssdp:discover\"");
                request.AppendLine($"MX: {timeout.Seconds}");
                request.AppendLine($"ST: {searchTarget}");
                // request.AppendLine("ST: ssdp:all");
                // request.AppendLine("ST: upnp:rootdevice");
                // request.AppendLine("ST: uuid:xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx");
                // request.AppendLine("ST: urn:schemas-upnp-org:service:WANIPConnection:1");
                request.AppendLine(); // This blank line is necessary.
                byte[] requestBytes = Encoding.UTF8.GetBytes(request.ToString());
                //
                EndPoint ep = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);
                socket.ReceiveTimeout = timeout.Milliseconds + 500;
                socket.EnableBroadcast = true;
                socket.MulticastLoopback = false;
                socket.Bind(new IPEndPoint(IPAddress.Any, 0));
                socket.SendTo(requestBytes, ep);
                byte[] buffer = new byte[8 * 1024];
                while (true)
                {
                    try
                    {
                        int len = socket.ReceiveFrom(buffer, ref ep);
                        responses.Add(Encoding.UTF8.GetString(buffer, 0, len));
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            if (responses.Count > 0)
            {
                foreach (string response in responses)
                {
                    if (response.IndexOf("HTTP/1.1 200 OK", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        string find = "LOCATION:";
                        int index = response.IndexOf(find, StringComparison.OrdinalIgnoreCase);
                        if (index >= 0)
                        {
                            index += find.Length;
                            int count = response.IndexOf(Environment.NewLine, index, StringComparison.OrdinalIgnoreCase) - index;
                            if (count > 0)
                            {
                                string descriptionUrl = response.Substring(index, count).Trim();
                                bool exists = false;
                                foreach (UPnPRootDevice rootDevice in rootDevices)
                                {
                                    if (rootDevice.DescriptionUrl == descriptionUrl)
                                    {
                                        exists = true;
                                        break;
                                    }
                                }
                                if (!exists)
                                {
                                    try
                                    {
                                        rootDevices.Add(new UPnPRootDevice(descriptionUrl));
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return rootDevices.ToArray();
        }
    }
}