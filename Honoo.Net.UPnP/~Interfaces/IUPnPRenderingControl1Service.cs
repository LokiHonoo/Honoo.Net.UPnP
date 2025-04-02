using System;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP IGD "urn:schemas-upnp-org:service:RenderingControl:1" interface.
    /// <br />USAGE: <see href="https://upnp.org/specs/av/UPnP-av-RenderingControl-v1-Service.pdf"/>
    /// </summary>
    public interface IUPnPRenderingControl1Service : IUPnPService
    {
        /// <summary>
        /// This action retrieves the current value of the BlueVideoBlackLevel state variable of the specified instance of this service.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        ushort GetBlueVideoBlackLevel(uint instanceID);

        /// <summary>
        /// This action retrieves the current value of the BlueVideoGain state variable of the specified instance of this service.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        ushort GetBlueVideoGain(uint instanceID);

        /// <summary>
        /// This action retrieves the current value of the Brightness state variable of the specified instance of this service.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        ushort GetBrightness(uint instanceID);

        /// <summary>
        /// This action retrieves the current value of the ColorTemperature state variable of the specified instance of this service.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        ushort GetColorTemperature(uint instanceID);

        /// <summary>
        ///  This action retrieves the current value of the Contrast state variable of the specified instance of this service.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        ushort GetContrast(uint instanceID);

        /// <summary>
        /// This action retrieves the current value of the GreenVideoBlackLevel state variable of the specified instance of this service.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        ushort GetGreenVideoBlackLevel(uint instanceID);

        /// <summary>
        /// This action retrieves the current value of the GreenVideoGain state variable of the specified instance of this service.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        ushort GetGreenVideoGain(uint instanceID);

        /// <summary>
        /// This action retrieves the current value of the HorizontalKeystone state variable of the specified instance of this service.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        short GetHorizontalKeystone(uint instanceID);

        /// <summary>
        /// This action retrieves the current value of the Loudness setting of the channel for the specified instance of this service.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="channel">This property accepts the following: "Master", "LF", "RF", "CF", "LFE", "LS", "RS", "LFC", "RFC", "SD", "SL", "SR", "T", "B", Vendor-defined.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        bool GetLoudness(uint instanceID, string channel);

        /// <summary>
        /// This action retrieves the current value of the Mute setting of the channel for the specified instance of this service.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="channel">This property accepts the following: "Master", "LF", "RF", "CF", "LFE", "LS", "RS", "LFC", "RFC", "SD", "SL", "SR", "T", "B", Vendor-defined.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        bool GetMute(uint instanceID, string channel);

        /// <summary>
        /// This action retrieves the current value of the RedVideoBlackLevel state variable of the specified instance of this service.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        ushort GetRedVideoBlackLevel(uint instanceID);

        /// <summary>
        /// This action retrieves the current value of the RedVideoGain state variable of the specified instance of this service.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        ushort GetRedVideoGain(uint instanceID);

        /// <summary>
        /// This action retrieves the current value of the Sharpness state variable of the specified instance of this service.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        ushort GetSharpness(uint instanceID);

        /// <summary>
        /// This action retrieves the current value of the VerticalKeystone state variable of the specified instance of this service.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        short GetVerticalKeystone(uint instanceID);

        /// <summary>
        /// This action retrieves the current value of the Volume state variable of the specified channel for the specified instance of this service.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="channel">This property accepts the following: "Master", "LF", "RF", "CF", "LFE", "LS", "RS", "LFC", "RFC", "SD", "SL", "SR", "T", "B", Vendor-defined.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        ushort GetVolume(uint instanceID, string channel);

        /// <summary>
        /// This action retrieves the current value of the VolumeDB state variable of the channel for the specified instance of this service.
        /// <br />The CurrentVolume(OUT) parameter represents the current volume setting in units of 1/256 decibels(dB).
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="channel">This property accepts the following: "Master", "LF", "RF", "CF", "LFE", "LS", "RS", "LFC", "RFC", "SD", "SL", "SR", "T", "B", Vendor-defined.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        short GetVolumeDB(uint instanceID, string channel);

        /// <summary>
        /// This action retrieves the valid range for the VolumeDB state variable of the channel for the specified instance of this service.
        /// <br />The MinValue and MaxValue(OUT) parameter identify the range of valid values for the VolumeDB state variable in units of 1/256 decibels(dB).
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="channel">This property accepts the following: "Master", "LF", "RF", "CF", "LFE", "LS", "RS", "LFC", "RFC", "SD", "SL", "SR", "T", "B", Vendor-defined.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        UPnPVolumeDBRange GetVolumeDBRange(uint instanceID, string channel);

        /// <summary>
        ///  Its value changes if/when the device changes the set of presets that it supports.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        string ListPresets(uint instanceID);

        /// <summary>
        ///  This action restores (a subset) of the state variables to the values associated with the specified preset.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="presetName">This property accepts the following: "FactoryDefaults", "InstallationDefaults", Vendor-defined.</param>
        /// <exception cref="Exception"/>
        void SelectPreset(uint instanceID, string presetName);

        /// <summary>
        /// This action sets the BlueVideoBlackLevel state variable of the specified instance of this service to the specified value.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="desiredBlueVideoBlackLevel">This unsigned integer variable represents the current setting for the minimum output intensity of blue for the associated display device.</param>
        /// <exception cref="Exception"/>
        void SetBlueVideoBlackLevel(uint instanceID, ushort desiredBlueVideoBlackLevel);

        /// <summary>
        /// This action sets the BlueVideoGain state variable of the specified instance of this service to the specified value.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="desiredBlueVideoGain">This unsigned integer variable represents the current setting of the blue "gain" control for the associated display device.</param>
        /// <exception cref="Exception"/>
        void SetBlueVideoGain(uint instanceID, ushort desiredBlueVideoGain);

        /// <summary>
        /// This action sets the Brightness state variable of the specified instance of this service to the specified value.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="desiredBrightness">This unsigned integer variable represents the current brightness setting of the associated display device.</param>
        /// <exception cref="Exception"/>
        void SetBrightness(uint instanceID, ushort desiredBrightness);

        /// <summary>
        ///  This action sets the ColorTemperature state variable of the specified instance of this service to the specified value.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="desiredColorTemperature">This unsigned integer variable represents the current setting for the "color quality" of white for the associated display device.</param>
        /// <exception cref="Exception"/>
        void SetColorTemperature(uint instanceID, ushort desiredColorTemperature);

        /// <summary>
        ///  This action sets the Contrast state variable of the specified instance of this service to the specified value.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="desiredContrast">This unsigned integer variable represents the current contrast setting of the associated display device.</param>
        /// <exception cref="Exception"/>
        void SetContrast(uint instanceID, ushort desiredContrast);

        /// <summary>
        /// This action sets the GreenVideoBlackLevel state variable of the specified instance of this service to the specified value.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="desiredGreenVideoBlackLevel">This unsigned integer variable represents the current setting for the minimum output intensity of green for the associated display device.</param>
        /// <exception cref="Exception"/>
        void SetGreenVideoBlackLevel(uint instanceID, ushort desiredGreenVideoBlackLevel);

        /// <summary>
        /// This action sets the GreenVideoGain state variable of the specified instance of this service to the specified value.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="desiredGreenVideoGain">This unsigned integer variable represents the current setting of the green "gain" control for the associated display device.</param>
        /// <exception cref="Exception"/>
        void SetGreenVideoGain(uint instanceID, ushort desiredGreenVideoGain);

        /// <summary>
        /// This action sets the HorizontalKeystone state variable of the specified instance of this service to the specified value.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="desiredHorizontalKeystone">This signed integer variable represents the current level of compensation for horizontal distortion (described below) of the associated display device.</param>
        /// <exception cref="Exception"/>
        void SetHorizontalKeystone(uint instanceID, short desiredHorizontalKeystone);

        /// <summary>
        /// This action sets the specified value of the Loudness state variable of the channel for the specified instance of this service to the specified value.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="channel">This property accepts the following: "Master", "LF", "RF", "CF", "LFE", "LS", "RS", "LFC", "RFC", "SD", "SL", "SR", "T", "B", Vendor-defined.</param>
        /// <param name="desiredLoudness"> This boolean variable represents the current "loudness" setting of the associated audio channel.
        /// <br />A value of TRUE(e.g.a numerical value of 1) indicates that the loudness effect is active.
        /// </param>
        /// <exception cref="Exception"/>
        void SetLoudness(uint instanceID, string channel, bool desiredLoudness);

        /// <summary>
        /// This action sets the Mute state variable of the specified instance of this service to the specified value.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="channel">This property accepts the following: "Master", "LF", "RF", "CF", "LFE", "LS", "RS", "LFC", "RFC", "SD", "SL", "SR", "T", "B", Vendor-defined.</param>
        /// <param name="desiredMute"> This boolean variable represents the current "mute" setting of the associated audio channel.
        /// <br />A value of TRUE(e.g.a numerical value of 1) indicates that the output of the associated audio channel is currently muted(i.e.that channel is not producing any sound).
        /// </param>
        /// <exception cref="Exception"/>
        void SetMute(uint instanceID, string channel, bool desiredMute);

        /// <summary>
        /// This action sets the RedVideoBlackLevel state variable of the specified instance of this service to the specified value.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="desiredRedVideoBlackLevel">This unsigned integer variable represents the current setting for the minimum output intensity of red for the associated display device.</param>
        /// <exception cref="Exception"/>
        void SetRedVideoBlackLevel(uint instanceID, ushort desiredRedVideoBlackLevel);

        /// <summary>
        /// This action sets the RedVideoGain state variable of the specified instance of this service to the specified value.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="desiredRedVideoGain">This unsigned integer variable represents the current setting of the red "gain" control for the associated display device.</param>
        /// <exception cref="Exception"/>
        void SetRedVideoGain(uint instanceID, ushort desiredRedVideoGain);

        /// <summary>
        /// This action sets the Sharpness state variable of the specified instance of this service to the specified value.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="desiredSharpness">This unsigned integer variable represents the current sharpness setting of the associated display device.</param>
        /// <exception cref="Exception"/>
        void SetSharpness(uint instanceID, ushort desiredSharpness);

        /// <summary>
        /// This action sets the VerticalKeystone state variable of the specified instance of this service to the specified value.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="desiredVerticalKeystone">This signed integer variable represents the current level of compensation for vertical distortion (described below) of the associated display device.</param>
        /// <exception cref="Exception"/>
        void SetVerticalKeystone(uint instanceID, short desiredVerticalKeystone);

        /// <summary>
        /// This action sets the Volume state variable of the specified Instance and Channel to the specified value.
        /// <br />The DesiredVolume input parameter contains a value ranging from 0 to a device-specific maximum.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="channel">This property accepts the following: "Master", "LF", "RF", "CF", "LFE", "LS", "RS", "LFC", "RFC", "SD", "SL", "SR", "T", "B", Vendor-defined.</param>
        /// <param name="desiredVolume">This unsigned integer variable represents the current volume setting of the associated audio channel.</param>
        /// <exception cref="Exception"/>
        void SetVolume(uint instanceID, string channel, ushort desiredVolume);

        /// <summary>
        /// This action sets the VolumeDB state variable of the specified Instance and Channel to the specified value.
        /// <br />The DesiredVolume parameter represents the desired volume setting in unit of 1/256 decibels(dB).
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="channel">This property accepts the following: "Master", "LF", "RF", "CF", "LFE", "LS", "RS", "LFC", "RFC", "SD", "SL", "SR", "T", "B", Vendor-defined.</param>
        /// <param name="desiredVolume"> This signed integer variable represents the current volume setting of the associated audio channel. Its value represents the current setting in units of "1/256 of a decibel(dB)".</param>
        /// <exception cref="Exception"/>
        void SetVolumeDB(uint instanceID, string channel, short desiredVolume);
    }
}