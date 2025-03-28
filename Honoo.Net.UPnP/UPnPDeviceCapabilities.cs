using System.Collections.Generic;
using System.Text;
using System.Xml;

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
        private readonly string[] _recQualityModes;

        /// <summary>
        /// Playback storage media. Allowed value "NONE", "UNKNOWN", "CD-DA", "HDD", "NETWORK".
        /// </summary>
        public string PlayMedia => _playMedia;

        /// <summary>
        /// Record storage media.
        /// </summary>
        public string RecMedia => _recMedia;

        /// <summary>
        /// Record quality modes.
        /// </summary>
        public ICollection<string> RecQualityModes => _recQualityModes;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPDeviceCapabilities class.
        /// </summary>
        /// <param name="node">Response node.</param>
        internal UPnPDeviceCapabilities(XmlNode node)
        {
            _playMedia = node.SelectSingleNode("PlayMedia").InnerText.Trim();
            _recMedia = node.SelectSingleNode("RecMedia").InnerText.Trim();
            _recQualityModes = node.SelectSingleNode("RecQualityModes").InnerText.Trim().Split(',');
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
            builder.AppendLine($"RecQualityModes:{string.Join(",", _recQualityModes)}");
            return builder.ToString();
        }
    }
}