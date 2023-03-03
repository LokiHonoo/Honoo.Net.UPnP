using System.Text;
using System.Xml;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP position info.
    /// </summary>
    public sealed class UPnPPositionInfo
    {
        #region Properties

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

        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the UPnPPositionInfo class.
        /// </summary>
        /// <param name="node">Response node.</param>
        internal UPnPPositionInfo(XmlNode node)
        {
            _track = uint.Parse(node.SelectSingleNode("Track").InnerText.Trim());
            _trackURI = node.SelectSingleNode("TrackURI").InnerText.Trim();
            _trackDuration = node.SelectSingleNode("TrackDuration").InnerText.Trim();
            _trackMetaData = node.SelectSingleNode("TrackMetaData").InnerText.Trim();
            _absTime = node.SelectSingleNode("AbsTime").InnerText.Trim();
            _absCount = int.Parse(node.SelectSingleNode("AbsCount").InnerText.Trim());
            _relTime = node.SelectSingleNode("RelTime").InnerText.Trim();
            _relCount = int.Parse(node.SelectSingleNode("RelCount").InnerText.Trim());
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