using System.Text;
using System.Xml.Linq;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP device capabilities.
    /// </summary>
    public sealed class UPnPDeviceCapabilities
    {
        #region Members

        private readonly string _playMedia;
        private readonly string _recMedia;
        private readonly string _recQualityModes;

        /// <summary>
        /// Playback storage media.
        /// <br />This property accepts the following: "UNKNOWN","DV", "MINI-DV", "VHS", "W-VHS", "S-VHS", "D-VHS", "VHSC", "VIDEO8","HI8", "CD-ROM",
        /// <br />"CD-DA", ">CD-R", "CD-RW", "VIDEO-CD", "SACD","MD-AUDIO", "MD-PICTURE", "DVD-ROM", "DVD-VIDEO", "DVD-R", "DVD+RW","DVD-RW",
        /// <br />"DVD-RAM", "DVD-AUDIO", "DAT", "LD", "HDD", "MICRO-MV","NETWORK", "NONE", "NOT_IMPLEMENTED", Vendor-defined.
        /// </summary>
        public string PlayMedia => _playMedia;

        /// <summary>
        /// Record storage media.
        /// <br />This property accepts the following: "UNKNOWN","DV", "MINI-DV", "VHS", "W-VHS", "S-VHS", "D-VHS", "VHSC", "VIDEO8","HI8", "CD-ROM",
        /// <br />"CD-DA", ">CD-R", "CD-RW", "VIDEO-CD", "SACD","MD-AUDIO", "MD-PICTURE", "DVD-ROM", "DVD-VIDEO", "DVD-R", "DVD+RW","DVD-RW",
        /// <br />"DVD-RAM", "DVD-AUDIO", "DAT", "LD", "HDD", "MICRO-MV","NETWORK", "NONE", "NOT_IMPLEMENTED", Vendor-defined.
        /// </summary>
        public string RecMedia => _recMedia;

        /// <summary>
        /// Record quality modes. Value is CSV string.
        /// <br />This property accepts the following: "0:EP", "1:LP", "2:SP", "0:BASIC", "1:MEDIUM", "2:HIGH", "NOT_IMPLEMENTED", Vendor-defined.
        /// </summary>
        public string RecQualityModes => _recQualityModes;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPDeviceCapabilities class.
        /// </summary>
        /// <param name="element">Response element.</param>
        internal UPnPDeviceCapabilities(XElement element)
        {
            _playMedia = element.Element("PlayMedia").Value.Trim();
            _recMedia = element.Element("RecMedia").Value.Trim();
            _recQualityModes = element.Element("RecQualityModes").Value.Trim();
        }

        /// <summary>
        /// This method has been overridden. Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"PlayMedia:{_playMedia}");
            builder.AppendLine($"RecMedia:{_recMedia}");
            builder.AppendLine($"RecQualityModes:{_recQualityModes}");
            return builder.ToString();
        }
    }
}