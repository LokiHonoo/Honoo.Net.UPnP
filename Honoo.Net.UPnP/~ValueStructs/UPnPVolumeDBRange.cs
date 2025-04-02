using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP volume DB range.
    /// </summary>
    public sealed class UPnPVolumeDBRange
    {
        #region Members

        private readonly short _maxValue;
        private readonly short _minValue;

        /// <summary>
        /// It identify the range of valid values for the VolumeDB state variable in units of 1/256 decibels(dB).
        /// </summary>
        public short MaxValue => _maxValue;

        /// <summary>
        /// It identify the range of valid values for the VolumeDB state variable in units of 1/256 decibels(dB).
        /// </summary>
        public short MinValue => _minValue;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPVolumeDBRange class.
        /// </summary>
        /// <param name="element">Response element.</param>
        internal UPnPVolumeDBRange(XElement element)
        {
            _minValue = short.Parse(element.Element("MinValue").Value.Trim(), CultureInfo.InvariantCulture);
            _maxValue = short.Parse(element.Element("MaxValue").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// This method has been overridden. Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"MinValue:{_minValue}");
            builder.AppendLine($"MaxValue:{_maxValue}");
            return builder.ToString();
        }
    }
}