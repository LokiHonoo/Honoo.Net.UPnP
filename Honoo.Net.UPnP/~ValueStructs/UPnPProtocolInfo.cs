using System.Text;
using System.Xml.Linq;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP protocol info.
    /// </summary>
    public sealed class UPnPProtocolInfo
    {
        #region Members

        private readonly string _sink;
        private readonly string _source;

        /// <summary>
        /// This variable contains a comma-separated list of information on protocols this ConnectionManager supports for “sinking” (receiving) data, in its current state. Value is CSV string.
        /// </summary>
        public string Sink => _sink;

        /// <summary>
        /// TThis variable contains a comma-separated list of information on protocols this ConnectionManager supports for “sourcing” (sending) data, in its current state. Value is CSV string.
        /// </summary>
        public string Source => _source;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPProtocolInfo class.
        /// </summary>
        /// <param name="element">Response element.</param>

        internal UPnPProtocolInfo(XElement element)
        {
            _source = element.Element("Source").Value.Trim();
            _sink = element.Element("Sink").Value.Trim();
        }

        /// <summary>
        /// This method has been overridden. Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Source:{_source}");
            builder.AppendLine($"Sink:{_sink}");
            return builder.ToString();
        }
    }
}