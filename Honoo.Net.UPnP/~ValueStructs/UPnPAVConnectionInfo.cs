using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP AVTransport connection info.
    /// </summary>
    public sealed class UPnPAVConnectionInfo
    {
        #region Members

        private readonly int _avTransportID;
        private readonly string _direction;
        private readonly int _peerConnectionID;
        private readonly string _peerConnectionManager;
        private readonly string _protocolInfo;
        private readonly int _rcsID;
        private readonly string _status;

        /// <summary>
        /// It identifies a logical instance of the AVTransport service associated with a Connection.See [ref to Device Model] for more information.
        /// </summary>
        public int AVTransportID => _avTransportID;

        /// <summary>
        /// This state variable is introduced to provide type information for the "Direction" parameter in action PrepareForConnection.
        /// <br />This property accepts the following: "Output", "Input".
        /// </summary>
        public string Direction => _direction;

        /// <summary>
        /// It identifies the specific connection on that ConnectionManager service.
        /// </summary>
        public int PeerConnectionID => _peerConnectionID;

        /// <summary>
        /// This state variable is introduced to provide type information for the "PeerConnectionManager" parameter in actions PrepareForConnection and GetCurrentConnectionInfo.
        /// </summary>
        public string PeerConnectionManager => _peerConnectionManager;

        /// <summary>
        /// This state variable is introduced to provide type information for the "Protocol" parameter in actions PrepareForConnection and GetCurrentConnectionInfo.
        /// </summary>
        public string ProtocolInfo => _protocolInfo;

        /// <summary>
        /// It identifies a logical instance of the Rendering Control service associated with a Connection.See [ref to Device Model] for more information.
        /// </summary>
        public int RcsID => _rcsID;

        /// <summary>
        /// The current status of the Connection referred to by variable A_ARG_TYPE_ConnectionID. This status may change dynamically due to changes in the network.
        /// <br />This property accepts the following: "OK", "ContentFormatMismatch", "InsufficientBandwidth", "UnreliableChannel", "Unknown", Vendor-defined .
        /// </summary>
        public string Status => _status;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPAVConnectionInfo class.
        /// </summary>
        /// <param name="element">Response element.</param>
        internal UPnPAVConnectionInfo(XElement element)
        {
            _rcsID = int.Parse(element.Element("RcsID").Value.Trim(), CultureInfo.InvariantCulture);
            _avTransportID = int.Parse(element.Element("AVTransportID").Value.Trim(), CultureInfo.InvariantCulture);
            _protocolInfo = element.Element("ProtocolInfo").Value.Trim();
            _peerConnectionManager = element.Element("PeerConnectionManager").Value.Trim();
            _peerConnectionID = int.Parse(element.Element("PeerConnectionID").Value.Trim(), CultureInfo.InvariantCulture);
            _direction = element.Element("Direction").Value.Trim();
            _status = element.Element("Status").Value.Trim();
        }

        /// <summary>
        /// This method has been overridden. Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"RcsID:{_rcsID}");
            builder.AppendLine($"AVTransportID:{_avTransportID}");
            builder.AppendLine($"ProtocolInfo:{_protocolInfo}");
            builder.AppendLine($"PeerConnectionManager:{_peerConnectionManager}");
            builder.AppendLine($"PeerConnectionID:{_peerConnectionID}");
            builder.AppendLine($"Direction:{_direction}");
            builder.AppendLine($"Status:{_status}");
            return builder.ToString();
        }
    }
}