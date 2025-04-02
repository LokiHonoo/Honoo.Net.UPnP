using System;
using System.Net;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP IGDv2 "urn:schemas-upnp-org:service:WANIPConnection:2" interface.
    /// <br />USAGE: <see href="https://upnp.org/specs/gw/UPnP-gw-WANIPConnection-v2-Service.pdf"/>
    /// </summary>
    public interface IUPnPWANIPConnection2Service : IUPnPWANIPConnection1Service
    {
        /// <summary>
        /// Add any port mapping, and gets reserved port.
        /// </summary>
        /// <param name="protocol">The protocol to mapping. This property accepts the following: "TCP", "UDP".</param>
        /// <param name="remoteHost">This argument refers to the remote host which sends packets to the IGD.If a wildcard is defined then packets MAY be sent by any host,
        /// <br />but if a specific IP address is defined, then only packets sent by this IP address are forwarded. Can set to <see cref="string.Empty"/>.</param>
        /// <param name="externalPort">The external port to mapping.</param>
        /// <param name="internalClient">The internal client to mapping.</param>
        /// <param name="internalPort">The internal port to mapping.</param>
        /// <param name="enabled">Enabled.</param>
        /// <param name="description">Port mapping description.</param>
        /// <param name="leaseDuration">Lease duration. This property accepts the following 0 - 604800. Unit is seconds. Set 0 to permanents.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        ushort AddAnyPortMapping(string protocol,
                                 string remoteHost,
                                 ushort externalPort,
                                 IPAddress internalClient,
                                 ushort internalPort,
                                 bool enabled,
                                 string description,
                                 uint leaseDuration);

        /// <summary>
        /// Delete port mapping range.
        /// </summary>
        /// <param name="protocol">The protocol to delete mapping. This property accepts the following: "TCP", "UDP".</param>
        /// <param name="startPort">The start port of search.</param>
        /// <param name="endPort">The end port of search.</param>
        /// <param name="manage">If a control point wants to get or to remove only its own port mappings it SHOULD be set to “0” (false).
        /// <br />If the intent is to manage port mappings for other clients, then NewManage SHOULD be set to “1” (true).
        /// <br />This flag does not supersede access control based on control points IP address.
        /// </param>
        /// <exception cref="Exception"/>
        void DeletePortMappingRange(string protocol, ushort startPort, ushort endPort, bool manage);

        /// <summary>
        /// Get list of port mapping entries.
        /// </summary>
        /// <param name="protocol">The protocol to query. This property accepts the following: "TCP", "UDP".</param>
        /// <param name="startPort">The start port of search.</param>
        /// <param name="endPort">The end port of search.</param>
        /// <param name="manage">If a control point wants to get or to remove only its own port mappings it SHOULD be set to “0” (false).
        /// <br />If the intent is to manage port mappings for other clients, then NewManage SHOULD be set to “1” (true).
        /// <br />This flag does not supersede access control based on control points IP address.
        /// </param>
        /// <param name="maxCount">Returns count of max.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetListOfPortMappings(string protocol, ushort startPort, ushort endPort, bool manage, ushort maxCount);
    }
}