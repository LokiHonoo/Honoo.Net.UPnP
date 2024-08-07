# Honoo.Net.UPnP

- [Honoo.Net.UPnP](#honoonetupnp)
  - [INTRODUCTION](#introduction)
  - [USAGE](#usage)
    - [NuGet](#nuget)
  - [QUICK START](#quick-start)
    - [Namespace](#namespace)
    - [PortMapping](#portmapping)
    - [DLNA](#dlna)
  - [LICENSE](#license)

## INTRODUCTION

Simple UPnP. Provides port mapping, DLNA e.g..

## USAGE

### NuGet

<https://www.nuget.org/packages/Honoo.Net.UPnP/>

## QUICK START

### Namespace

```c#

using Honoo.Net.UPnP;

```

### PortMapping

```c#

private static void TestPortMapping()
{
    UPnPRootDevice[] devices = UPnP.Discover(UPnP.URN_UPNP_SERVICE_WAN_IP_CONNECTION_1, 2000);
    UPnPRootDevice router = devices[0];
    IUPnPWANIPConnectionService service = router.FindService(UPnP.URN_UPNP_SERVICE_WAN_IP_CONNECTION_1);

    //Console.WriteLine(service.GetNATRSIPStatus());
    //Console.WriteLine(service.GetExternalIPAddress());
    //Console.WriteLine(service.GetStatusInfo());
    //Console.WriteLine(service.GetConnectionTypeInfo());
    //Console.WriteLine(service.GetGenericPortMappingEntry(1));

    try
    {
        _ = service.GetSpecificPortMappingEntry("TCP", 4788);
    }
    catch
    {
        service.AddPortMapping("TCP", 4788, IPAddress.Parse("192.168.1.11"), 4788, true, "test", 0);
    }
    UPnPPortMappingEntry entry = service.GetSpecificPortMappingEntry("TCP", 4788);
    Console.WriteLine(entry.Protocol + " " + entry.ExternalPort + " " + entry.InternalClient + ":" + entry.InternalPort);
    service.DeletePortMapping("TCP", 4788);

    Console.ReadKey(true);
}

```

### DLNA

```c#

private static void TestDlna()
{
    UPnPRootDevice[] devices = UPnP.Discover(UPnP.URN_UPNP_SERVICE_AV_TRANSPORT_1, 2000);
    UPnPRootDevice dlna = devices[0];

    //Console.WriteLine(((IUPnPMediaRendererDevice)dlna.Device).XDlnaDoc);

    IUPnPAVTransportService service = dlna.FindService(UPnP.URN_UPNP_SERVICE_AV_TRANSPORT_1);

    //Console.WriteLine(service.GetDeviceCapabilities(0));
    //Console.WriteLine(service.GetTransportSettings(0));
    //Console.WriteLine(service.GetMediaInfo(0));
    //Console.WriteLine(service.GetPositionInfo(0));
    //Console.WriteLine(service.GetTransportInfo(0));
    //Console.WriteLine(service.GetCurrentTransportActions(0));

    // Need setup firewall. Administrator privileges are required.
    UPnPDlnaServer mediaServer = new UPnPDlnaServer(new Uri("http://192.168.1.10:8080/"));

    //string callbackUrl = server.AddEventSubscriber(DlanEventHandler);
    //string sid = service.SetEventSubscription(callbackUrl, 3600);

    string mediaUrl = mediaServer.AddMedia("E:\\Videos\\The Ankha Zone.mp4");
    service.SetAVTransportURI(0, mediaUrl, string.Empty);
    service.Play(0, "1");
    Console.ReadKey(true);
    service.Stop(0);
    mediaServer.Dispose();

    //service.RemoveEventSubscription(sid);
    //mediaServer.RemoveEventSubscriber(callbackUrl);
}

```

## LICENSE

[MIT](LICENSE) license.
