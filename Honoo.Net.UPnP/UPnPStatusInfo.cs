using System.Globalization;
using System.Text;
using System.Xml;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP status info.
    /// </summary>
    public sealed class UPnPStatusInfo
    {
        #region Members

        private readonly string _connectionStatus;
        private readonly string _lastConnectionError;
        private readonly uint _uptime;

        /// <summary>
        /// Connection status.
        /// <br />This property accepts the following: "Unconfigured", "Connecting", "Connected", "PendingDisconnect", "Disconnecting", "Disconnected", Vendor-defined.
        /// </summary>
        public string ConnectionStatus => _connectionStatus;

        /// <summary>
        /// Last connection error.
        /// <br />This property accepts the following: "ERROR_NONE", "ERROR_COMMAND_ABORTED", "ERROR_NOT_ENABLED_FOR_INTERNET", "ERROR_USER_DISCONNECT", "ERROR_ISP_DISCONNECT",
        /// <br />"ERROR_IDLE_DISCONNECT", "ERROR_FORCED_DISCONNECT", "ERROR_NO_CARRIER", "ERROR_IP_CONFIGURATION", "ERROR_UNKNOWN", Vendor-defined.
        /// </summary>
        public string LastConnectionError => _lastConnectionError;

        /// <summary>
        /// Update time.
        /// </summary>
        public uint Uptime => _uptime;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPStatusInfo class.
        /// </summary>
        /// <param name="node">Response node.</param>

        internal UPnPStatusInfo(XmlNode node)
        {
            _connectionStatus = node.SelectSingleNode("NewConnectionStatus").InnerText.Trim();
            _lastConnectionError = node.SelectSingleNode("NewLastConnectionError").InnerText.Trim();
            _uptime = uint.Parse(node.SelectSingleNode("NewUptime").InnerText.Trim(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// This method has been overridden. Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"NewConnectionStatus:{_connectionStatus}");
            builder.AppendLine($"NewLastConnectionError:{_lastConnectionError}");
            builder.AppendLine($"NewUptime:{_uptime}");
            return builder.ToString();
        }
    }
}