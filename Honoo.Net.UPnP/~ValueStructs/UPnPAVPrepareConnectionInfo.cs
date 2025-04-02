using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP AVTransport prepare connection info.
    /// </summary>
    public sealed class UPnPAVPrepareConnectionInfo
    {
        #region Members

        private readonly int _avTransportID;
        private readonly int _connectionID;
        private readonly int _rcsID;

        /// <summary>
        /// It identifies a logical instance of the AVTransport service associated with a Connection.See [ref to Device Model] for more information.
        /// </summary>
        public int AVTransportID => _avTransportID;

        /// <summary>
        /// It identifies the specific connection on that ConnectionManager service.
        /// </summary>
        public int ConnectionID => _connectionID;

        /// <summary>
        /// It identifies a logical instance of the Rendering Control service associated with a Connection.See [ref to Device Model] for more information.
        /// </summary>
        public int RcsID => _rcsID;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPAVPrepareConnectionInfo class.
        /// </summary>
        /// <param name="element">Response element.</param>
        internal UPnPAVPrepareConnectionInfo(XElement element)
        {
            _connectionID = int.Parse(element.Element("ConnectionID").Value.Trim(), CultureInfo.InvariantCulture);
            _avTransportID = int.Parse(element.Element("AVTransportID").Value.Trim(), CultureInfo.InvariantCulture);
            _rcsID = int.Parse(element.Element("RcsID").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// This method has been overridden. Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"ConnectionID:{_connectionID}");
            builder.AppendLine($"AVTransportID:{_avTransportID}");
            builder.AppendLine($"RcsID:{_rcsID}");
            return builder.ToString();
        }
    }
}