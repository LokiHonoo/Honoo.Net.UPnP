using System;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP IGDv1 "urn:schemas-upnp-org:service:AVTransport:1" interface.
    /// <br />USAGE: <see href="https://upnp.org/specs/av/UPnP-av-AVTransport-v1-Service.pdf"/>
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
        /// Record. whether the device outputs the resource to a screen or speakers while recording is device dependent.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <exception cref="Exception"/>
        void Record(uint instanceID);

        /// <summary>
        /// Seek. This state variable is introduced to provide type information for the “target” parameter in action “Seek”. It
        /// <br />indicates the target position of the seek action, in terms of units defined by state variable A_ARG_TYPE_SeekMode.
        /// <br />The data type of this variable is ‘string’. However, depending on the actual seek mode used, it must contains
        /// <br />string representations of values of UPnP types ‘ui4’ (ABS_COUNT, REL_COUNT, TRACK_NR, TAPE-INDEX, FRAME),
        /// <br />‘time’ (ABS_TIME, REL_TIME) or ‘float‘ (CHANNEL_FREQ, in Hz). Supported ranges of these integer, time or float
        /// <br />values are device-dependent.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="unit">The seek mode.</param>
        /// <param name="target">Target by seek mode. REL_TIME: 00:33:33, TRACK_NR(Track number of CD-DA): 1.</param>
        /// <exception cref="Exception"/>
        void Seek(uint instanceID, string unit, string target);

        /// <summary>
        /// Set audio/video transport uri. Need DLNA http server. Used by "UPnPDlnaServer" or design by youself.
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
        /// "NORMAL", "SHUFFLE", "REPEAT_ONE", "REPEAT_ALL", "RANDOM", "DIRECT_1", "INTRO".</param>
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