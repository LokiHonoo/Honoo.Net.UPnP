using System;
using System.Collections.Generic;

namespace Honoo.Net.UPnP
{
    /// <summary>
    /// UPnP service interface.
    /// </summary>
    public interface IUPnPService
    {
        /// <summary>
        /// Control url.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:类 URI 属性不应是字符串", Justification = "<挂起>")]
        string ControlUrl { get; }

        /// <summary>
        /// Event sub url.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:类 URI 属性不应是字符串", Justification = "<挂起>")]
        string EventSubUrl { get; }

        /// <summary>
        /// Parent device.
        /// </summary>
        UPnPDevice ParentDevice { get; }

        /// <summary>
        /// Root device.
        /// </summary>
        UPnPRootDevice RootDevice { get; }

        /// <summary>
        /// Scpd url.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:类 URI 属性不应是字符串", Justification = "<挂起>")]
        string ScpdUrl { get; }

        /// <summary>
        /// Service ID.
        /// </summary>
        string ServiceID { get; }

        /// <summary>
        /// Service type.
        /// </summary>
        string ServiceType { get; }

        /// <summary>
        /// Gets SCPD information form SCPD url.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetScpdInformation();

        /// <summary>
        /// Post action, and gets response xml string. Query actions from service's SCPD information description page.
        /// </summary>
        /// <param name="action">action name.</param>
        /// <param name="arguments">action arguments. The arguments must conform to the order specified. Set 'null' if haven't arguments.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string PostAction(string action, IList<KeyValuePair<string, string>> arguments);

        /// <summary>
        ///  Remove event subscription.
        /// </summary>
        /// <param name="sid">Event subscription sid.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        void RemoveEventSubscription(string sid);

        /// <summary>
        ///  Renewal event subscription.
        /// </summary>
        /// <param name="sid">Event subscription sid.</param>
        /// <param name="durationSecond">Subscription duration. Unit is second.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        void RenewalEventSubscription(string sid, uint durationSecond);

        /// <summary>
        /// Set event subscription by event subscriber url and gets event SID.
        /// </summary>
        /// <param name="subscriberUrl">Event subscriber url.</param>
        /// <param name="durationSecond">Subscription duration. Unit is second.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:类 URI 参数不应为字符串", Justification = "<挂起>")]
        string SetEventSubscription(string subscriberUrl, uint durationSecond);
    }
}