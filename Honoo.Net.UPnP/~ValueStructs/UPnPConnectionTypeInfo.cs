using System.Text;
using System.Xml.Linq;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP connection type info.
    /// </summary>
    public sealed class UPnPConnectionTypeInfo
    {
        #region Members

        private readonly string _connectionType;
        private readonly string _possibleConnectionTypes;

        /// <summary>
        /// Connection type.
        /// <br />This property accepts the following: "Unconfigured", "IP_Routed", "IP_Bridged".
        /// </summary>
        public string ConnectionType => _connectionType;

        /// <summary>
        /// Possible connection types. Value is CSV string.
        /// <br />This property accepts the following: "Unconfigured", "IP_Routed", "IP_Bridged".
        /// </summary>
        public string PossibleConnectionTypes => _possibleConnectionTypes;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPConnectionTypeInfo class.
        /// </summary>
        /// <param name="element">Response element.</param>
        internal UPnPConnectionTypeInfo(XElement element)
        {
            _connectionType = element.Element("NewConnectionType").Value.Trim();
            _possibleConnectionTypes = element.Element("NewPossibleConnectionTypes").Value.Trim();
        }

        /// <summary>
        /// This method has been overridden. Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"NewConnectionType:{_connectionType}");
            builder.AppendLine($"NewPossibleConnectionTypes:{_possibleConnectionTypes}");
            return builder.ToString();
        }
    }
}