using Honoo.Net;
using Honoo.Windows;
using System;
using System.Net;
using System.Threading;

namespace Test
{
    internal class Program
    {
        #region Main

        private static void Main()
        {
            //string lastChangeEvent = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
            //    "<propertyset xmlns:e=\"urn:schemas-upnp-org:event-1-0\">\r\n" +
            //    "<property>\r\n" +
            //    "<LastChange>\r\n" +
            //    "&lt;Event xmlns=&quot;urn:schemas-upnp-org:metadata-1-0/AVT/&quot;&gt;\r\n" +
            //    "&lt;InstanceID val=&quot;0&quot;&gt;\r\n" +
            //    "&lt;AVTransportURI val=&quot;http://192.168.17.10:8080/A25AD3AD154DF1D119C7871F331815F4707138991E3A25F34899174C2C374ADE&quot;/&gt;\r\n" +
            //    "&lt;CurrentTrackURI val=&quot;null&quot;/&gt;\r\n" +
            //    "&lt;TransportState val=&quot;PLAYING&quot;/&gt;\r\n" +
            //    "&lt;CurrentTransportActions val=&quot;Play,Pause,Stop&quot;/&gt;\r\n" +
            //    "&lt;CurrentTrackDuration val=&quot;00:01:31&quot;/&gt;\r\n" +
            //    "&lt;CurrentMediaDuration val=&quot;00:01:31&quot;/&gt;\r\n" +
            //    "&lt;AbsoluteTimePosition val=&quot;00:00:00&quot;/&gt;\r\n" +
            //    "&lt;RelativeTimePosition val=&quot;00:00:00&quot;/&gt;\r\n" +
            //    "&lt;NumberOfTracks val=&quot;1&quot;/&gt;\r\n" +
            //    "&lt;CurrentTrack val=&quot;1&quot;/&gt;\r\n" +
            //    "&lt;TransportStatus val=&quot;OK&quot;/&gt;\r\n" +
            //    "&lt;/InstanceID&gt;&lt;/Event&gt;\r\n" +
            //    "</LastChange>\r\n" +
            //    "</property>\r\n" +
            //    "</propertyset>";

            //string post = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\r\n" +
            //    "<s:Body>\r\n  " +
            //    "  <u:GetSpecificPortMappingEntryResponse xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\">\r\n" +
            //    "    <NewInternalPort>4788</NewInternalPort>\r\n" +
            //    "    <NewInternalClient>192.168.1.11</NewInternalClient>\r\n" +
            //    "    <NewEnabled>1</NewEnabled>\r\n" +
            //    "    <NewPortMappingDescription>test</NewPortMappingDescription>\r\n" +
            //    "    <NewLeaseDuration>604036</NewLeaseDuration>\r\n" +
            //    "  </u:GetSpecificPortMappingEntryResponse>\r\n" +
            //    "</s:Body>\r\n" +
            //    "</s:Envelope>";

            //string stateVariableValuePairs = "<stateVariableValuePairs xmlns=\"urn:schemas-upnp-org:av:avs\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"urn:schemas-upnp-org:av:avs http://www.upnp.org/schemas/av/avs.xsd\">\r\n" +
            //    "  <stateVariable variableName=\"CurrentPlayMode\">NORMAL</stateVariable>\r\n" +
            //    "  <stateVariable variableName=\"CurrentTrack\">3</stateVariable>\r\n" +
            //    "</stateVariableValuePairs> ";

            ConsoleHelper.DisableQuickEditMode();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=======================================================================================");
                Console.WriteLine();
                Console.WriteLine("                        Honoo.Net.UPnP TEST   runtime " + Environment.Version);
                Console.WriteLine();
                Console.WriteLine("=======================================================================================");
                //
                Console.WriteLine();
                Console.WriteLine("  1. All Devices");
                Console.WriteLine();
                Console.WriteLine("  2. Port Mapping");
                Console.WriteLine("  3. Dlna - Need setup port open for firewall. Administrator privileges are required.");
                Console.WriteLine();
                Console.WriteLine("  X. Exit");
                Console.WriteLine();
                Console.WriteLine();
                Console.Write("Choice a project:");
                while (true)
                {
                    var kc = Console.ReadKey(true).KeyChar;
                    switch (kc)
                    {
                        case '1': Console.Clear(); GetAll(); break;
                        case '2': Console.Clear(); TestPortMapping(); break;
                        case '3': Console.Clear(); TestDlna(); break;
                        case 'x': case 'X': return;
                        default: continue;
                    }
                    break;
                }
                Console.WriteLine();
                Console.Write("Press any key to Main Menu...");
                Console.ReadKey(true);
            }
        }

        #endregion Main

        private static void GetAll()
        {
            UPnPRootDevice[] devices = UPnP.Discover(TimeSpan.FromMilliseconds(3000));
            foreach (var device in devices)
            {
                Console.WriteLine("==================================================");
                ReadDevice(device.Device, string.Empty);
            }
        }

        private static void MediaServer_RequestFailed(UPnPServer sender, UPnPRequestFailedEventArgs e)
        {
            Console.WriteLine(e.Exception.ToString());
        }

        private static void ReadDevice(UPnPDevice device, string indent)
        {
            Console.WriteLine(indent + "device: " + device.DeviceType + "    " + device.FriendlyName);
            if (device.Devices.Count > 0)
            {
                Console.WriteLine(indent + "deviceList");
                foreach (var child in device.Devices)
                {
                    ReadDevice(child, indent + "  ");
                }
                Console.WriteLine(indent + "deviceList");
            }
            if (device.Services.Count > 0)
            {
                Console.WriteLine(indent + "serviceList");
                foreach (var child in device.Services)
                {
                    Console.WriteLine(indent + "  " + "service: " + child.ServiceType);
                }
                Console.WriteLine(indent + "serviceList");
            }
        }

        private static void TestDlna()
        {
            UPnPRootDevice[] devices = UPnP.Discover(TimeSpan.FromMilliseconds(2000), UPnP.URN_UPNP_SERVICE_AV_TRANSPORT_1);
            UPnPRootDevice dlna = devices[0];

            var serviceAV = dlna.FindService(UPnP.URN_UPNP_SERVICE_AV_TRANSPORT_1).Interfaces.AVTransport1;
            var serviceRC = dlna.FindService(UPnP.URN_UPNP_SERVICE_RENDERING_CONTROL_1).Interfaces.RenderingControl1;

            // Need setup port open for firewall. Administrator privileges are required.
            // Maybe need close firewall to test.
            UPnPDlnaServer mediaServer = UPnP.CreateDlnaServer(new Uri("http://192.168.17.10:8080/"));
            mediaServer.RequestFailed += MediaServer_RequestFailed;

            string mediaUrl = mediaServer.AddMedia("E:\\Videos\\OP-ED\\[CASO][Girls-High][NCED][DVDRIP][x264_Vorbis][8D8A632B].mkv");

            string callbackUrl = mediaServer.AddEventSubscriber(UPnPAVTEventRaisedCallback, null);
            string sid = serviceAV.SetEventSubscription(callbackUrl, 3600);

            mediaServer.Start();

            serviceAV.SetAVTransportURI(0, mediaUrl, string.Empty);

            serviceAV.Play(0, "1");
            Thread.Sleep(2000);
            serviceRC.SetVolume(0, "Master", 50); // Device maybe not supported
            Thread.Sleep(2000);
            serviceRC.SetVolume(0, "Master", 14); // Device maybe not supported

            Console.ReadKey(true);
            serviceAV.Stop(0);
            //
            serviceAV.RemoveEventSubscription(sid);
            mediaServer.RemoveEventSubscriber(callbackUrl);
            mediaServer.Dispose();
        }

        private static void TestPortMapping()
        {
            UPnPRootDevice[] devices = UPnP.Discover(TimeSpan.FromMilliseconds(2000), UPnP.URN_UPNP_SERVICE_WAN_IP_CONNECTION_1);
            UPnPRootDevice router = devices[0];

            var service = router.FindService(UPnP.URN_UPNP_SERVICE_WAN_IP_CONNECTION_1).Interfaces.WANIPConnection1;

            UPnPPortMappingEntry entry;
            try
            {
                entry = service.GetSpecificPortMappingEntry("TCP", string.Empty, 4788);
            }
            catch
            {
                service.AddPortMapping("TCP", string.Empty, 4788, IPAddress.Parse("192.168.1.11"), 4788, true, "test", 0);
                entry = service.GetSpecificPortMappingEntry("TCP", string.Empty, 4788);
            }
            Console.WriteLine(entry.Protocol + " " + entry.ExternalPort + " " + entry.InternalClient + ":" + entry.InternalPort);
            service.DeletePortMapping("TCP", string.Empty, 4788);
        }

        private static void UPnPAVTEventRaisedCallback(UPnPServer sender, UPnPEventMessage message, object userState)
        {
            var @interface = message.Interfaces.MediaRenderer;
            foreach (var instance in @interface.Instances.Values)
            {
                Console.WriteLine(DateTime.Now + "     ================================================");
                Console.WriteLine($"InstanceID: {instance.InstanceID}");
                foreach (var property in instance.Properties.Values)
                {
                    Console.Write(property.Name + ":");
                    // Console.WriteLine(property.Attributes["val"]);
                    foreach (var att in property.Attributes)
                    {
                        Console.Write($" {att.Key}=\"{att.Value}\"");
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}