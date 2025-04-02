using System;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP IGD "urn:schemas-upnp-org:service:ConnectionManager:1" interface.
    /// <br />USAGE: <see href="https://upnp.org/specs/av/UPnP-av-ConnectionManager-v1-Service.pdf"/>
    /// </summary>
    public interface IUPnPConnectionManager1Service : IUPnPService
    {
        /// <summary>
        /// A control point should call the ConnectionComplete action for all connections that it created via
        /// <br />PrepareForConnection to ensure that all resources associated with the connection are freed up.In addition,
        ///  <br />a ConnectionManager may implemented "automatic" or "autonomous" closing of connections, in a protocol
        ///  <br />and vendor-specfic way, see Appendix A for details.
        /// </summary>
        /// <param name="ConnectionID">Connection id.</param>
        /// <exception cref="Exception"/>
        void ConnectionComplete(uint ConnectionID);

        /// <summary>
        /// Returns a comma-separated list of ConnectionIDs of currently ongoing Connections. A ConnectionID can
        /// <br />be used to manually terminate a Connection via action ConnectionComplete, or to retrieve additional
        /// <br />information about the ongoing Connection via action GetCurrentConnectionInfo. Value is CSV string.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetCurrentConnectionIDs();

        /// <summary>
        /// Returns associated information of the connection referred to by the "ConnectionID" parameter. The
        /// <br />"AVTransportID" and "PeerConnectionManager" parameters may be NULL(empty string) in cases where
        /// <br />the connection has been setup completely out of band, e.g., not involving a PrepareForConnection action.
        /// </summary>
        /// <param name="ConnectionID">Connection id.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPAVConnectionInfo GetCurrentConnectionInfo(uint ConnectionID);

        /// <summary>
        /// Returns the protocol-related info that this ConnectionManager supports in its current state.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPProtocolInfo GetProtocolInfo();

        /// <summary>
        /// Prepare connection.
        /// </summary>
        /// <param name="remoteProtocolInfo">It identifies the protocol, network, and format that should be used to transfer the content.</param>
        /// <param name="peerConnectionManager">It identifies the ConnectionManager service on the other side of the connection.</param>
        /// <param name="peerConnectionID">It identifies the specific connection on that ConnectionManager service.</param>
        /// <param name="direction">This property accepts the following: "Output", "Input".</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPAVPrepareConnectionInfo PrepareForConnection(string remoteProtocolInfo, string peerConnectionManager, int peerConnectionID, string direction);
    }
}