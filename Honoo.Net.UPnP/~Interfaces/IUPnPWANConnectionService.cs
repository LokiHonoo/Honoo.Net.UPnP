using System;
using System.Net;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP virtual WANConnection interface.
    /// </summary>
    public interface IUPnPWANConnectionService : IUPnPService
    {
        /// <summary>
        /// Add port mapping.
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
        /// <exception cref="Exception"/>
        void AddPortMapping(string protocol,
                            string remoteHost,
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
        /// <param name="remoteHost">This argument refers to the remote host which sends packets to the IGD.If a wildcard is defined then packets MAY be sent by any host,
        /// <br />but if a specific IP address is defined, then only packets sent by this IP address are forwarded. Can set to <see cref="string.Empty"/>.</param>
        /// <param name="externalPort">The external port to delete mapping.</param>
        /// <exception cref="Exception"/>
        void DeletePortMapping(string protocol, string remoteHost, ushort externalPort);

        /// <summary>
        /// Force termination.
        /// </summary>
        /// <exception cref="Exception"/>
        void ForceTermination();

        /// <summary>
        /// Get auto disconnect time in seconds.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        uint GetAutoDisconnectTime();

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
        /// Get Idle disconnect time in seconds.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        uint GetIdleDisconnectTime();

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
        /// <param name="remoteHost">This argument refers to the remote host which sends packets to the IGD.If a wildcard is defined then packets MAY be sent by any host,
        /// <br />but if a specific IP address is defined, then only packets sent by this IP address are forwarded. Can set to <see cref="string.Empty"/>.</param>
        /// <param name="externalPort">The external port to query.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPPortMappingEntry GetSpecificPortMappingEntry(string protocol, string remoteHost, ushort externalPort);

        /// <summary>
        /// Get status info.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPStatusInfo GetStatusInfo();

        /// <summary>
        /// Get warn disconnect delay in seconds.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        uint GetWarnDisconnectDelay();

        /// <summary>
        /// Request connection.
        /// </summary>
        /// <exception cref="Exception"/>
        void RequestConnection();

        /// <summary>
        /// Request termination to change ConnectionStatus to Disconnected.
        /// </summary>
        /// <exception cref="Exception"/>
        void RequestTermination();

        /// <summary>
        /// Set auto disconnect time in seconds.
        /// </summary>
        /// <param name="seconds">Sets the time (in seconds) after which an active connection is automatically disconnected.</param>
        /// <exception cref="Exception"/>
        void SetAutoDisconnectTime(uint seconds);

        /// <summary>
        /// Set connection type.
        /// </summary>
        /// <param name="connectionType">The connection type. This property accepts the following: "Unconfigured", "IP_Routed", "IP_Bridged".</param>
        /// <exception cref="Exception"/>
        void SetConnectionType(string connectionType);

        /// <summary>
        /// Set Idle disconnect time in seconds.
        /// </summary>
        /// <param name="seconds">Specifies the idle time (in seconds) after which a connection may be disconnected. The actual disconnect will occur after WarnDisconnectDelay time elapses.</param>
        /// <exception cref="Exception"/>
        void SetIdleDisconnectTime(uint seconds);

        /// <summary>
        /// Set warn disconnect delay in seconds.
        /// </summary>
        /// <param name="seconds">Specifies the number of seconds of warning to each (potentially) active user of a connection before a connection is terminated.</param>
        /// <exception cref="Exception"/>
        void SetWarnDisconnectDelay(uint seconds);
    }
}