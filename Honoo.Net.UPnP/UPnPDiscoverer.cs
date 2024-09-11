using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Honoo.Net.UPnP
{
    /// <summary>
    /// UPnP discoverer.
    /// </summary>
    public static class UPnPDiscoverer
    {
        /// <summary>
        /// Discover UPnP devices.
        /// </summary>
        /// <param name="searchTarget">
        /// Http header "ST". Look this: <see cref="UPnPSearchTarget.UPNP_ROOT_DEVICE"/> and so on.
        /// <br/>Can used URN string as "urn:schemas-upnp-org:service:WANIPConnection:1".
        /// <br/>Can used UUID string as "uuid:xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx".
        /// </param>
        /// <param name="durationMilliseconds">Search duration. Unit is milliseconds.</param>
        /// <param name="mxSeconds">Maximum response timeout. Unit is seconds.</param>
        /// <returns></returns>
        public static UPnPRootDevice[] Discover(string searchTarget = UPnPSearchTarget.UPNP_ROOT_DEVICE, int durationMilliseconds = 2000, int mxSeconds = 2)
        {
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
                request.AppendLine($"MX: {mxSeconds}");
                request.AppendLine($"ST: {searchTarget}");
                // request.AppendLine("ST: ssdp:all");
                // request.AppendLine("ST: upnp:rootdevice");
                // request.AppendLine("ST: uuid:xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx");
                // request.AppendLine("ST: urn:schemas-upnp-org:service:WANIPConnection:1");
                request.AppendLine(); // This blank line is necessary.
                byte[] requestBytes = Encoding.UTF8.GetBytes(request.ToString());
                //
                EndPoint ep = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);
                socket.ReceiveTimeout = durationMilliseconds;
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