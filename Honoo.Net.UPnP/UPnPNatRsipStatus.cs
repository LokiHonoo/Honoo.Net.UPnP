using System;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP NAT RSIP status.
    /// </summary>
    public sealed class UPnPNatRsipStatus
    {
        #region Members

        private readonly bool _natEnabled;
        private readonly bool _rsipAvailable;

        /// <summary>
        /// NAT enabled.
        /// </summary>
        public bool NatEnabled => _natEnabled;

        /// <summary>
        /// RSIP available.
        /// </summary>
        public bool RsipAvailable => _rsipAvailable;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the NatRsipStatus class.
        /// </summary>
        /// <param name="node">Response node.</param>
        internal UPnPNatRsipStatus(XmlNode node)
        {
            _natEnabled = Convert.ToBoolean(int.Parse(node.SelectSingleNode("NewNATEnabled").InnerText.Trim(), CultureInfo.InvariantCulture));
            _rsipAvailable = Convert.ToBoolean(int.Parse(node.SelectSingleNode("NewRSIPAvailable").InnerText.Trim(), CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// This method has been overridden. Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"NewNATEnabled:{(_natEnabled ? 1 : 0)}");
            builder.AppendLine($"NewRSIPAvailable:{(_rsipAvailable ? 1 : 0)}");
            return builder.ToString();
        }
    }
}