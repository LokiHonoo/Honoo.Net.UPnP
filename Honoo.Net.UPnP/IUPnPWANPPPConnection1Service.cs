using System;
using System.Net;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP IGDv1 "urn:schemas-upnp-org:service:WANPPPConnection:1" interface.
    /// <br />USAGE: <see href="https://upnp.org/specs/gw/UPnP-gw-WANPPPConnection-v1-Service.pdf"/>
    /// </summary>
    public interface IUPnPWANPPPConnection1Service : IUPnPWANConnectionService
    {
        /// <summary>
        /// Gets link layer max bit rates.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPLinkLayerMaxBitRates GetLinkLayerMaxBitRates();

        /// <summary>
        /// Gets password.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetPassword();

        /// <summary>
        /// Gets PPP authentication protocol.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetPPPAuthenticationProtocol();

        /// <summary>
        /// Gets PPP compression protocol.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetPPPCompressionProtocol();

        /// <summary>
        /// Gets PPP encryption protocol.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetPPPEncryptionProtocol();

        /// <summary>
        /// Gets user name.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetUserName();
    }
}