using System;
using System.Net;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP IGDv1 "urn:schemas-upnp-org:service:WANPPPConnection:1" interface.
    /// <br />USAGE: <see href="https://upnp.org/specs/gw/UPnP-gw-WANPPPConnection-v1-Service.pdf"/>
    /// </summary>
    public interface IUPnPWANPPPConnection1Service : IUPnPService
    {
        /// <summary>
        /// Add port mapping.
        /// </summary>
        /// <param name="protocol">The protocol to mapping. This property accepts the following: "TCP", "UDP".</param>
        /// <param name="externalPort">The external port to mapping.</param>
        /// <param name="internalClient">The internal client to mapping.</param>
        /// <param name="internalPort">The internal port to mapping.</param>
        /// <param name="enabled">Enabled.</param>
        /// <param name="description">Port mapping description.</param>
        /// <param name="leaseDuration">Lease duration. This property accepts the following 0 - 604800. Unit is seconds. Set 0 to permanents.</param>
        /// <exception cref="Exception"/>
        void AddPortMapping(string protocol,
                            ushort externalPort,
                            IPAddress internalClient,
                            ushort internalPort,
                            bool enabled,
                            string description,
                            uint leaseDuration);

        /// <summary>
        /// Delete port mapping.
        /// </summary>
        /// <param name="protocol">The protocol to delete mapping. This property accepts the following: "TCP", "UDP".</param>
        /// <param name="externalPort">The external port to delete mapping.</param>
        /// <exception cref="Exception"/>
        void DeletePortMapping(string protocol, ushort externalPort);

        /// <summary>
        /// Force termination.
        /// </summary>
        /// <exception cref="Exception"/>
        void ForceTermination();

        /// <summary>
        /// Get Connection type info. Possible types maybe wrong because I don't know what the separator is. :(
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPConnectionTypeInfo GetConnectionTypeInfo();

        /// <summary>
        /// Get external IPAddress.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetExternalIPAddress();

        /// <summary>
        /// Get generic port mapping entry.
        /// </summary>
        /// <param name="index">The index of entry.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPPortMappingEntry GetGenericPortMappingEntry(uint index);

        /// <summary>
        /// Get NAT RSIP status.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPNatRsipStatus GetNATRSIPStatus();

        /// <summary>
        /// Get specific port mapping entry.
        /// </summary>
        /// <param name="protocol">The protocol to query. This property accepts the following: "TCP", "UDP".</param>
        /// <param name="externalPort">The external port to query.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPPortMappingEntry GetSpecificPortMappingEntry(string protocol, ushort externalPort);

        /// <summary>
        /// Get status info.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPStatusInfo GetStatusInfo();

        /// <summary>
        /// Request connection.
        /// </summary>
        /// <exception cref="Exception"/>
        void RequestConnection();

        /// <summary>
        /// Set connection type.
        /// </summary>
        /// <param name="connectionType">The connection type. This property accepts the following: "Unconfigured", "IP_Routed", "IP_Bridged".</param>
        /// <exception cref="Exception"/>
        void SetConnectionType(string connectionType);
    }
}