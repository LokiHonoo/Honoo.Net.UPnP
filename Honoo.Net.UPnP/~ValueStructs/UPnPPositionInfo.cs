using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP position info.
    /// </summary>
    public sealed class UPnPPositionInfo
    {
        #region Members

        private readonly int _absCount;
        private readonly string _absTime;
        private readonly int _relCount;
        private readonly string _relTime;
        private readonly uint _track;
        private readonly string _trackDuration;
        private readonly string _trackMetaData;
        private readonly string _trackURI;

        /// <summary>
        /// Absolute counter position.
        /// </summary>
        public int AbsCount => _absCount;

        /// <summary>
        /// Absolute time position.
        /// </summary>
        public string AbsTime => _absTime;

        /// <summary>
        /// Relative counter position.
        /// </summary>
        public int RelCount => _relCount;

        /// <summary>
        /// Relative time position.
        /// </summary>
        public string RelTime => _relTime;

        /// <summary>
        /// Current track.
        /// </summary>
        public uint Track => _track;

        /// <summary>
        /// Current track duration.
        /// </summary>
        public string TrackDuration => _trackDuration;

        /// <summary>
        /// Current track meta data.
        /// </summary>
        public string TrackMetaData => _trackMetaData;

        /// <summary>
        /// Current track uri.
        /// </summary>
        public string TrackURI => _trackURI;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPPositionInfo class.
        /// </summary>
        /// <param name="element">Response element.</param>
        internal UPnPPositionInfo(XElement element)
        {
            _track = uint.Parse(element.Element("Track").Value.Trim(), CultureInfo.InvariantCulture);
            _trackURI = element.Element("TrackURI").Value.Trim();
            _trackDuration = element.Element("TrackDuration").Value.Trim();
            _trackMetaData = element.Element("TrackMetaData").Value.Trim();
            _absTime = element.Element("AbsTime").Value.Trim();
            _absCount = int.Parse(element.Element("AbsCount").Value.Trim(), CultureInfo.InvariantCulture);
            _relTime = element.Element("RelTime").Value.Trim();
            _relCount = int.Parse(element.Element("RelCount").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// This method has been overridden. Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Track:{_track}");
            builder.AppendLine($"TrackURI:{_trackURI}");
            builder.AppendLine($"TrackDuration:{_trackDuration}");
            builder.AppendLine($"TrackMetaData:{_trackMetaData}");
            builder.AppendLine($"AbsTime:{_absTime}");
            builder.AppendLine($"AbsCount:{_absCount}");
            builder.AppendLine($"RelTime:{_relTime}");
            builder.AppendLine($"RelCount:{_relCount}");
            return builder.ToString();
        }
    }
}