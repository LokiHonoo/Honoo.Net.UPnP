using System.Text;
using System.Xml;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP transport settings.
    /// </summary>
    public sealed class UPnPTransportSettings
    {
        #region Members

        private readonly string _playMode;
        private readonly string _recQualityMode;

        /// <summary>
        /// Current play mode.
        /// <br />This property accepts the following: "NORMAL", "SHUFFLE", "REPEAT_ONE", "REPEAT_ALL", "RANDOM", "DIRECT_1", "INTRO", Vendor-defined.
        /// </summary>
        public string PlayMode => _playMode;

        /// <summary>
        /// Current record quality mode.
        /// <br />This property accepts the following: "0:EP", "1:LP", "2:SP", "0:BASIC", "1:MEDIUM", "2:HIGH", "NOT_IMPLEMENTED", Vendor-defined.
        /// </summary>
        public string RecQualityMode => _recQualityMode;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPTransportSettings class.
        /// </summary>
        /// <param name="node">Response node.</param>
        internal UPnPTransportSettings(XmlNode node)
        {
            _playMode = node.SelectSingleNode("PlayMode").InnerText.Trim();
            _recQualityMode = node.SelectSingleNode("RecQualityMode").InnerText.Trim();
        }

        /// <summary>
        /// This method has been overridden. Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"PlayMode:{_playMode}");
            builder.AppendLine($"RecQualityMode:{_recQualityMode}");
            return builder.ToString();
        }
    }
}