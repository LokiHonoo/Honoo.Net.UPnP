using System.Text;
using System.Xml;

namespace Honoo.Net.UPnP
{
    /// <summary>
    /// UPnP status info.
    /// </summary>
    public sealed class UPnPStatusInfo
    {
        #region Properties

        private readonly string _connectionStatus;
        private readonly string _lastConnectionError;
        private readonly uint _uptime;

        /// <summary>
        /// Connection status. This property accepts the following:
        /// "Unconfigured", "Connecting", "Authenticating", "PendingDisconnect", "Disconnecting", "Disconnected", "Connected".
        /// </summary>
        public string ConnectionStatus => _connectionStatus;

        /// <summary>
        /// Last connection error. This property accepts the following:
        /// <para/>"ERROR_NONE", "ERROR_UNKNOWN", "ERROR_ISP_TIME_OUT", "ERROR_COMMAND_ABORTED", "ERROR_NOT_ENABLED_FOR_INTERNET"
        /// <para/>"ERROR_BAD_PHONE_NUMBER", "ERROR_USER_DISCONNECT", "ERROR_ISP_DISCONNECT", "ERROR_IDLE_DISCONNECT", "ERROR_FORCED_DISCONNECT"
        /// <para/>"ERROR_SERVER_OUT_OF_RESOURCES", "ERROR_RESTRICTED_LOGON_HOURS", "ERROR_ACCOUNT_DISABLED", "ERROR_ACCOUNT_EXPIRED"
        /// <para/>"ERROR_PASSWORD_EXPIRED", "ERROR_AUTHENTICATION_FAILURE", "ERROR_NO_DIALTONE", "ERROR_NO_CARRIER", "ERROR_NO_ANSWER", "ERROR_LINE_BUSY"
        /// <para/>"ERROR_UNSUPPORTED_BITSPERSECOND", "ERROR_TOO_MANY_LINE_ERRORS", "ERROR_IP_CONFIGURATION"
        /// </summary>
        public string LastConnectionError => _lastConnectionError;

        /// <summary>
        /// Update time.
        /// </summary>
        public uint Uptime => _uptime;

        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the UPnPStatusInfo class.
        /// </summary>
        /// <param name="node">Response node.</param>

        internal UPnPStatusInfo(XmlNode node)
        {
            _connectionStatus = node.SelectSingleNode("NewConnectionStatus").InnerText.Trim();
            _lastConnectionError = node.SelectSingleNode("NewLastConnectionError").InnerText.Trim();
            _uptime = uint.Parse(node.SelectSingleNode("NewUptime").InnerText.Trim());
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