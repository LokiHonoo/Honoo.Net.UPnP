using System;
using System.Collections.Generic;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP IGD "urn:schemas-upnp-org:service:RenderingControl:2" interface.
    /// <br />USAGE: <see href="https://upnp.org/specs/av/UPnP-av-RenderingControl-v2-Service.pdf"/>
    /// </summary>
    public interface IUPnPRenderingControl2Service : IUPnPRenderingControl1Service
    {
        /// <summary>
        /// Get state variables. It maybe didn't work because I'm not a RenderingControl2 device to test it  :(.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="stateVariableList">If the argument is set to "<see langword="*"/>", the action MUST return all the supported state variables of the service, including the
        /// <br />vendor-extended state variables except for LastChange and any A_ARG_TYPE_xxx variables. Value is CSV string.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        IDictionary<string, string> GetStateVariables(uint instanceID, string stateVariableList);

        /// <summary>
        /// Set state variables. It maybe didn't work because I'm not a RenderingControl2 device to test it  :(.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="renderingControlUDN"></param>
        /// <param name="serviceType"></param>
        /// <param name="serviceId"></param>
        /// <param name="stateVariableValuePairs"></param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string SetStateVariables(uint instanceID, string renderingControlUDN, string serviceType, string serviceId, IDictionary<string, string> stateVariableValuePairs);
    }
}