using System;
using System.Net;

namespace Honoo.Net.UPnP
{
    /// <summary>
    /// UPnP IGDv2 "urn:schemas-upnp-org:service:WANIPConnection:2" interface.
    /// </summary>
    public interface IUPnPWANIPConnection2Service : IUPnPWANIPConnection1Service
    {
        /// <summary>
        /// Add any port mapping, and gets reserved port.
        /// </summary>
        /// <param name="protocol">The protocol to mapping. This property accepts the following: "TCP", "UDP".</param>
        /// <param name="externalPort">The external port to mapping.</param>
        /// <param name="internalClient">The internal client to mapping.</param>
        /// <param name="internalPort">The internal port to mapping.</param>
        /// <param name="enabled">Enabled.</param>
        /// <param name="description">Port mapping description.</param>
        /// <param name="leaseDuration">Lease duration. This property accepts the following 0 - 604800. Unit is seconds. Set 0 to permanents.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        ushort AddAnyPortMapping(string protocol,
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
        /// <param name="manage">Elevate privileges.</param>
        /// <exception cref="Exception"/>
        void DeletePortMappingRange(string protocol, ushort startPort, ushort endPort, bool manage);

        /// <summary>
        /// Get list of port mapping entries.
        /// </summary>
        /// <param name="protocol">The protocol to query. This property accepts the following: "TCP", "UDP".</param>
        /// <param name="startPort">The start port of search.</param>
        /// <param name="endPort">The end port of search.</param>
        /// <param name="manage">Elevate privileges.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetListOfPortMappings(string protocol, ushort startPort, ushort endPort, bool manage);
    }
}