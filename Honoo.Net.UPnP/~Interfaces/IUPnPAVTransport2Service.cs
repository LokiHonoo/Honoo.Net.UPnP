using System;
using System.Collections.Generic;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP IGD "urn:schemas-upnp-org:service:AVTransport:2" interface.
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

        /// <summary>
        /// Get state variables. It maybe didn't work because I'm not a AVTransport2 device to test it  :(.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="stateVariableList">If the argument is set to "<see langword="*"/>", the action MUST return all the supported state variables of the service, including the
        /// <br />vendor-extended state variables except for LastChange and any A_ARG_TYPE_xxx variables. Value is CSV string.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        IDictionary<string, string> GetStateVariables(uint instanceID, string stateVariableList);

        /// <summary>
        /// Set state variables. It maybe didn't work because I'm not a AVTransport2 device to test it  :(.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="avTransportUDN"></param>
        /// <param name="serviceType"></param>
        /// <param name="serviceId"></param>
        /// <param name="stateVariableValuePairs"></param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string SetStateVariables(uint instanceID, string avTransportUDN, string serviceType, string serviceId, IDictionary<string, string> stateVariableValuePairs);
    }
}