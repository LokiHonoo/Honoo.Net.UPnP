using Honoo.Net.UPnP;
using Honoo.Windows;
using System;
using System.Collections.Generic;
using System.Net;

namespace Test
{
    internal class Program
    {
        private static void DlanEventCallback(UPnPEventMessage[] messages)
        {
            foreach (var message in messages)
            {
                Console.WriteLine(DateTime.Now + " ====================================================");
                Console.WriteLine(message.InstanceID);
                foreach (KeyValuePair<string, string> change in message.Changes)
                {
                    Console.WriteLine(change.Key + ":" + change.Value);
                }
            }
        }

        private static void Main()
        {
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
                Console.WriteLine("  1. Port Mapping");
                Console.WriteLine("  2. Dlna");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.Write("Choice a project:");
                while (true)
                {
                    var kc = Console.ReadKey(true).KeyChar;
                    switch (kc)
                    {
                        case '1': Console.Clear(); TestPortMapping(); break;
                        case '2': Console.Clear(); TestDlna(); break;
                        default: continue;
                    }
                    break;
                }
                Console.WriteLine();
                Console.Write("Press any key to Main Menu...");
                Console.ReadKey(true);
            }
        }

        private static void TestDlna()
        {
            UPnPRootDevice[] devices = UPnP.Discover(UPnP.URN_UPNP_SERVICE_AV_TRANSPORT_1);
            UPnPRootDevice dlna = devices[0];

            Console.WriteLine(dlna.Device.Interfaces.MediaRenderer1.XDlnaDoc);

            //var service = dlna.FindService(UPnP.URN_UPNP_SERVICE_AV_TRANSPORT_1).Interfaces.AVTransport1;
            IUPnPAVTransport1Service service = dlna.FindService(UPnP.URN_UPNP_SERVICE_AV_TRANSPORT_1);

            //Console.WriteLine(service.GetDeviceCapabilities(0));
            //Console.WriteLine(service.GetTransportSettings(0));
            //Console.WriteLine(service.GetMediaInfo(0));
            //Console.WriteLine(service.GetPositionInfo(0));
            //Console.WriteLine(service.GetTransportInfo(0));
            //Console.WriteLine(service.GetCurrentTransportActions(0));

            // Need setup firewall. Administrator privileges are required.
            UPnPDlnaServer mediaServer = new UPnPDlnaServer(new Uri("http://192.168.18.4:8080/"));

            string callbackUrl = mediaServer.AddEventSubscriber(DlanEventCallback);
            string sid = service.SetEventSubscription(callbackUrl, 3600);

            string mediaUrl = mediaServer.AddMedia("E:\\Videos\\OP-ED\\[CASO][Girls-High][NCED][DVDRIP][x264_Vorbis][8D8A632B].mkv");
            service.SetAVTransportURI(0, mediaUrl, string.Empty);
            service.Play(0, "1");
            //service.Seek(0, "rel_time", "00:33:33");
            Console.ReadKey(true);
            service.Stop(0);
            //
            service.RemoveEventSubscription(sid);
            //mediaServer.RemoveEventSubscriber(callbackUrl);
            mediaServer.Dispose();
        }

        private static void TestPortMapping()
        {
            UPnPRootDevice[] devices = UPnP.Discover(UPnP.URN_UPNP_SERVICE_WAN_IP_CONNECTION_1);
            UPnPRootDevice router = devices[0];

            //var service = router.FindService(UPnP.URN_UPNP_SERVICE_WAN_IP_CONNECTION_1).Interfaces.WANIPConnection1;
            IUPnPWANIPConnection1Service service = router.FindService(UPnP.URN_UPNP_SERVICE_WAN_IP_CONNECTION_1);

            //Console.WriteLine(service.GetNATRSIPStatus());
            //Console.WriteLine(service.GetExternalIPAddress());
            //Console.WriteLine(service.GetStatusInfo());
            //Console.WriteLine(service.GetConnectionTypeInfo());
            //Console.WriteLine(service.GetGenericPortMappingEntry(1));

            UPnPPortMappingEntry entry;
            try
            {
                entry = service.GetSpecificPortMappingEntry("TCP", 4788);
            }
            catch
            {
                service.AddPortMapping("TCP", 4788, IPAddress.Parse("192.168.1.11"), 4788, true, "test", 0);
                entry = service.GetSpecificPortMappingEntry("TCP", 4788);
            }
            Console.WriteLine(entry.Protocol + " " + entry.ExternalPort + " " + entry.InternalClient + ":" + entry.InternalPort);
            service.DeletePortMapping("TCP", 4788);
        }
    }
}