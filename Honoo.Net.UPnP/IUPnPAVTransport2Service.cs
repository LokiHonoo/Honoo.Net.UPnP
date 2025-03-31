using System;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP IGDv1 "urn:schemas-upnp-org:service:AVTransport:2" interface.
    /// <br />USAGE: <see href="https://upnp.org/specs/av/UPnP-av-AVTransport-v2-Service.pdf"/>
    /// </summary>
    public interface IUPnPAVTransport2Service : IUPnPAVTransport1Service
    {
        /// <summary>
        /// Get DRMState. This property accepts the following: "OK", "UNKNOWN", "PROCESSING_CONTENT_KEY", "CONTENT_KEY_FAILURE", "ATTEMPTING_AUTHENTICATION", "FAILED_AUTHENTICATION", "NOT_AUTHENTICATED", "DEVICE_REVOCATION".
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetDRMState(uint instanceID);

        /// <summary>
        /// Get media info ext.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPMediaInfoExt GetMediaInfoExt(uint instanceID);
    }
}