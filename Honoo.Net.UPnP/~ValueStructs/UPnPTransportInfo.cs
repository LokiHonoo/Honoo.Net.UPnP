using System.Text;
using System.Xml.Linq;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP transport info.
    /// </summary>
    public sealed class UPnPTransportInfo
    {
        #region Members

        private readonly string _currentSpeed;
        private readonly string _currentTransportState;
        private readonly string _currentTransportStatus;

        /// <summary>
        /// Current transport play speed.
        /// </summary>
        public string CurrentSpeed => _currentSpeed;

        /// <summary>
        /// Current transport state.
        /// <br />This property accepts the following: "STOPPED", "PAUSED_PLAYBACK", "PAUSED_RECORDING", "PLAYING", "RECORDING", "TRANSITIONING", "NO_MEDIA_PRESENT", Vendor-defined.
        /// </summary>
        public string CurrentTransportState => _currentTransportState;

        /// <summary>
        /// Current transport status.
        /// <br />This property accepts the following: "OK", "ERROR_OCCURRED", Vendor-defined.
        /// </summary>
        public string CurrentTransportStatus => _currentTransportStatus;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPTransportInfo class.
        /// </summary>
        /// <param name="element">Response element.</param>
        internal UPnPTransportInfo(XElement element)
        {
            _currentTransportState = element.Element("CurrentTransportState").Value.Trim();
            _currentTransportStatus = element.Element("CurrentTransportStatus").Value.Trim();
            _currentSpeed = element.Element("CurrentSpeed").Value.Trim();
        }

        /// <summary>
        /// This method has been overridden. Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"CurrentTransportState:{_currentTransportState}");
            builder.AppendLine($"CurrentTransportStatus:{_currentTransportStatus}");
            builder.AppendLine($"CurrentSpeed:{_currentSpeed}");
            return builder.ToString();
        }
    }
}