# Honoo.Net.UPnP

- [Honoo.Net.UPnP](#honoonetupnp)
  - [INTRODUCTION](#introduction)
  - [GUIDE](#guide)
    - [GitHub](#github)
  - [USAGE](#usage)
    - [Namespace](#namespace)
    - [PortMapping](#portmapping)
    - [DLNA](#dlna)
  - [LICENSE](#license)

## INTRODUCTION

Simple UPnP. Provides port mapping, DLNA e.g..

## GUIDE

### GitHub

<https://github.com/LokiHonoo/Honoo.Net.UPnP/>

## USAGE

### Namespace

```c#

using Honoo.Net.UPnP;

```

### PortMapping

```c#

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

    Console.ReadKey(true);
}

```

### DLNA

```c#

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
    UPnPDlnaServer mediaServer = UPnP.CreateDlnaServer(new Uri("http://192.168.1.11:8080/"), true);

    string callbackUrl = mediaServer.AddEventSubscriber(DlanEventCallback);
    string sid = service.SetEventSubscription(callbackUrl, 3600);

    string mediaUrl = mediaServer.AddMedia("E:\\Videos\\The Ankha Zone.mp4");
    service.SetAVTransportURI(0, mediaUrl, string.Empty);
    service.Play(0, "1");

    Console.ReadKey(true);
    service.Stop(0);
    //
    service.RemoveEventSubscription(sid);
    mediaServer.RemoveEventSubscriber(callbackUrl);
    mediaServer.Dispose();
}

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

```

## LICENSE

[MIT](LICENSE) license.
