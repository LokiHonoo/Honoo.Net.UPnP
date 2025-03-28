using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP connection type info.
    /// </summary>
    public sealed class UPnPConnectionTypeInfo
    {
        #region Members

        private readonly string _connectionType;
        private readonly string[] _possibleConnectionTypes;

        /// <summary>
        /// Connection type. This property accepts the following: "Unconfigured", "IP_Routed", "IP_Bridged".
        /// </summary>
        public string ConnectionType => _connectionType;

        /// <summary>
        /// Possible connection types. This property accepts the following: "Unconfigured", "IP_Routed", "IP_Bridged".
        /// </summary>
        public ICollection<string> PossibleConnectionTypes => _possibleConnectionTypes;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPConnectionTypeInfo class.
        /// </summary>
        /// <param name="node">Response node.</param>
        internal UPnPConnectionTypeInfo(XmlNode node)
        {
            _connectionType = node.SelectSingleNode("NewConnectionType").InnerText.Trim();
            _possibleConnectionTypes = node.SelectSingleNode("NewPossibleConnectionTypes").InnerText.Trim().Split(',');
        }

        /// <summary>
        /// This method has been overridden. Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"NewConnectionType:{_connectionType}");
            builder.AppendLine($"NewPossibleConnectionTypes:{string.Join(",", _possibleConnectionTypes)}");
            return builder.ToString();
        }
    }
}