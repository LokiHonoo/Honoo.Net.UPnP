using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP link layer max bit rates.
    /// </summary>
    public sealed class UPnPLinkLayerMaxBitRates
    {
        #region Members

        private readonly uint _downstreamMaxBitRate;
        private readonly uint _upstreamMaxBitRate;

        /// <summary>
        /// This variable represents the maximum downstream bit rate available to this connection instance. This variable has a static value once a connection is setup.
        /// </summary>
        public uint DownstreamMaxBitRate => _downstreamMaxBitRate;

        /// <summary>
        /// This variable represents the maximum upstream bit rate available to this connection instance. This variable has a static value once a connection is setup.
        /// </summary>
        public uint UpstreamMaxBitRate => _upstreamMaxBitRate;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPLinkLayerMaxBitRates class.
        /// </summary>
        /// <param name="element">Response element.</param>

        internal UPnPLinkLayerMaxBitRates(XElement element)
        {
            _upstreamMaxBitRate = uint.Parse(element.Element("NewUpstreamMaxBitRate").Value.Trim(), CultureInfo.InvariantCulture);
            _downstreamMaxBitRate = uint.Parse(element.Element("NewDownstreamMaxBitRate").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// This method has been overridden. Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"NewUpstreamMaxBitRate:{_upstreamMaxBitRate}");
            builder.AppendLine($"NewDownstreamMaxBitRate:{_downstreamMaxBitRate}");
            return builder.ToString();
        }
    }
}