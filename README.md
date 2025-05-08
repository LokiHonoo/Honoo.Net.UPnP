# Honoo.Net.UPnP

- [Honoo.Net.UPnP](#honoonetupnp)
  - [INTRODUCTION](#introduction)
  - [GUIDE](#guide)
    - [NuGet](#nuget)
  - [USAGE](#usage)
    - [Namespace](#namespace)
    - [PortMapping](#portmapping)
    - [DLNA](#dlna)
  - [CHANGELOG](#changelog)
    - [1.0.12](#1012)
  - [LICENSE](#license)

## INTRODUCTION

Simple UPnP. Provides port mapping, DLNA e.g..

PortMapping v1 - implemented.  
PortMapping v2 - implemented.  
DLNA v1 - implemented.  
DLNA v2 - implemented but GetStateVariables(), SetStateVariables() has some bugs maybe.   
DLNA v3 - 0 %.

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

```

### DLNA

```c#

private static void TestDlna()
{
    UPnPRootDevice[] devices = UPnP.Discover(TimeSpan.FromMilliseconds(2000), UPnP.URN_UPNP_SERVICE_AV_TRANSPORT_1);
    UPnPRootDevice dlna = devices[0];

    var serviceAV = dlna.FindService(UPnP.URN_UPNP_SERVICE_AV_TRANSPORT_1).Interfaces.AVTransport1;
    var serviceRC = dlna.FindService(UPnP.URN_UPNP_SERVICE_RENDERING_CONTROL_1).Interfaces.RenderingControl1;

    // Need setup port open for firewall. Administrator privileges are required.
    // Maybe need close firewall to test.
    UPnPDlnaServer mediaServer = UPnP.CreateDlnaServer(new Uri("http://192.168.1.11:8080/"));
    mediaServer.RequestFailed += MediaServer_RequestFailed;

    string mediaUrl = mediaServer.AddMedia("E:\\Videos\\The Ankha Zone.mp4");

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

private static void MediaServer_RequestFailed(UPnPServer sender, UPnPRequestFailedEventArgs e)
{
    Console.WriteLine(e.Exception.ToString());
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

```

## CHANGELOG

### 1.0.12

**Features* WANIPConnection:1 - implemented.  
**Features* WANIPConnection:2 - implemented.  
**Features* WANPPPConnection:1 - implemented.  
**Features* AVTransport:1 - implemented.  
**Features* AVTransport:2 - implemented but GetStateVariables(), SetStateVariables() has some bugs maybe.  
**Features* ConnectionManager:1 - implemented.  
**Features* ConnectionManager:2 - implemented.  
**Features* RenderingControl:1 - implemented.  
**Features* RenderingControl:2 - implemented but GetStateVariables(), SetStateVariables() has some bugs maybe.  

## LICENSE

[MIT](LICENSE) license.
