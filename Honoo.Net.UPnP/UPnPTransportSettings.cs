using System.Text;
using System.Xml;

namespace Honoo.Net.UPnP
{
    /// <summary>
    /// UPnP transport settings.
    /// </summary>
    public sealed class UPnPTransportSettings
    {
        #region Properties

        private readonly string _playMode;
        private readonly string _recQualityMode;

        /// <summary>
        /// Current play mode. This property accepts the following:
        /// "NORMAL", "REPEAT_ONE", "REPEAT_ALL", "SHUFFLE", "SHUFFLE_NOREPEAT".
        /// </summary>
        public string PlayMode => _playMode;

        /// <summary>
        /// Current record quality mode.
        /// </summary>
        public string RecQualityMode => _recQualityMode;

        #endregion Properties

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