using System;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP IGDv1 "urn:schemas-upnp-org:service:AVTransport:1" interface.
    /// </summary>
    public interface IUPnPAVTransport1Service : IUPnPService
    {
        /// <summary>
        /// Get current transport actions.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string GetCurrentTransportActions(uint instanceID);

        /// <summary>
        /// Get device capabilities.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPDeviceCapabilities GetDeviceCapabilities(uint instanceID);

        /// <summary>
        /// Get media info.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPMediaInfo GetMediaInfo(uint instanceID);

        /// <summary>
        /// Get position info.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPPositionInfo GetPositionInfo(uint instanceID);

        /// <summary>
        /// Get transport info.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPTransportInfo GetTransportInfo(uint instanceID);

        /// <summary>
        /// Get transport settings.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPTransportSettings GetTransportSettings(uint instanceID);

        /// <summary>
        /// Jump next track.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <exception cref="Exception"/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:标识符不应与关键字匹配", Justification = "<挂起>")]
        void Next(uint instanceID);

        /// <summary>
        /// Pause.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <exception cref="Exception"/>
        void Pause(uint instanceID);

        /// <summary>
        /// Play.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="speed">Transport play speed. This property is usually "1".</param>
        /// <exception cref="Exception"/>
        void Play(uint instanceID, string speed);

        /// <summary>
        /// Jump previous track.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <exception cref="Exception"/>
        void Previous(uint instanceID);

        /// <summary>
        /// Seek.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="unit">The seek mode. This property accepts the following: "REL_TIME", "TRACK_NR".</param>
        /// <param name="target">Target by seek mode. REL_TIME: 00:33:33, TRACK_NR(Track number of CD-DA): 1.</param>
        /// <exception cref="Exception"/>
        void Seek(uint instanceID, string unit, string target);

        /// <summary>
        /// Set audio/video transport uri. Need DLNA http server. Use "UPnPDlnaServer" or design by youself.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="currentURI">Current audio/video transport uri.</param>
        /// <param name="currentURIMetaData">Current audio/video transport uri meta data.</param>
        /// <exception cref="Exception"/>
        void SetAVTransportURI(uint instanceID, string currentURI, string currentURIMetaData);

        /// <summary>
        /// Set play mode.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="playMode"> Current play mode. This property accepts the following:
        /// "NORMAL", "REPEAT_ONE", "REPEAT_ALL", "SHUFFLE", "SHUFFLE_NOREPEAT".</param>
        /// <exception cref="Exception"/>
        void SetPlayMode(uint instanceID, string playMode);

        /// <summary>
        /// Stop.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <exception cref="Exception"/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:标识符不应与关键字匹配", Justification = "<挂起>")]
        void Stop(uint instanceID);
    }
}