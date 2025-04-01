# Honoo.Net.UPnP

- [Honoo.Net.UPnP](#honoonetupnp)
  - [INTRODUCTION](#introduction)
  - [GUIDE](#guide)
    - [NuGet](#nuget)
  - [USAGE](#usage)
    - [Namespace](#namespace)
    - [PortMapping](#portmapping)
    - [DLNA](#dlna)
  - [LICENSE](#license)

## INTRODUCTION

Simple UPnP. Provides port mapping, DLNA e.g..

## GUIDE

### NuGet

<https://www.nuget.org/packages/Honoo.Net.UPnP/>

## USAGE

### Namespace

```c#

using Honoo.Net;

```

### PortMapping

```c#

private static void TestPortMapping()
{
    UPnPRootDevice[] devices = UPnP.Discover(TimeSpan.FromMilliseconds(2000), UPnP.URN_UPNP_SERVICE_WAN_IP_CONNECTION_1);
    UPnPRootDevice router = devices[0];

    //IUPnPWANIPConnection1Service service = router.FindService(UPnP.URN_UPNP_SERVICE_WAN_IP_CONNECTION_1);
    var service = router.FindService(UPnP.URN_UPNP_SERVICE_WAN_IP_CONNECTION_1).Interfaces.WANIPConnection1;

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
    UPnPRootDevice[] devices = UPnP.Discover(TimeSpan.FromMilliseconds(2000), UPnP.URN_UPNP_SERVICE_AV_TRANSPORT_1);
    UPnPRootDevice dlna = devices[0];

    //IUPnPAVTransport1Service service = dlna.FindService(UPnP.URN_UPNP_SERVICE_AV_TRANSPORT_1);
    var service = dlna.FindService(UPnP.URN_UPNP_SERVICE_AV_TRANSPORT_1).Interfaces.AVTransport1;

    // Need setup firewall. Administrator privileges are required.
    // Maybe need close firewall to test.
    UPnPDlnaServer mediaServer = UPnP.CreateDlnaServer(new Uri("http://192.168.1.11:8080/"));
    mediaServer.RequestFailed += MediaServer_RequestFailed;

    string mediaUrl = mediaServer.AddMedia("E:\\Videos\\The Ankha Zone.mp4");

    string callbackUrl = mediaServer.AddEventSubscriber(UPnPEventRaisedCallback);
    string sid = service.SetEventSubscription(callbackUrl, 3600);

    mediaServer.Start();

    service.SetAVTransportURI(0, mediaUrl, string.Empty);
    service.Play(0, "1");

    Console.ReadKey(true);
    service.Stop(0);
    //
    service.RemoveEventSubscription(sid);
    mediaServer.RemoveEventSubscriber(callbackUrl);
    mediaServer.Dispose();
}

private static void MediaServer_RequestFailed(UPnPServer server, HttpListenerRequest request, Exception exception)
{
    Console.WriteLine(exception.ToString());
}

private static void UPnPEventRaisedCallback(UPnPEventMessage[] messages)
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
